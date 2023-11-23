package ws

import (
	"encoding/json"
	"fmt"
	"net/http"
	"time"

	"reverie/global"
	"reverie/model/config"
	"reverie/model/source"

	"github.com/gin-gonic/gin"
	"github.com/gorilla/websocket"
	"github.com/sirupsen/logrus"
)

type PoseData struct {
	Ts   int64                  `json:"ts"`
	Pose map[string][][]float64 `json:"pose"`
}

type MessageData struct {
	Game     string `json:"game"`
	PersonID string `json:"personId"`
}

// 临时message
type Pose struct {
	KeyPoints [][]float64 `json:"keyPoints"`
}

type Message struct {
	FrameId  int    `json:"frameId"`
	PersonId string `json:"personId"`
	TsEngine int64  `json:"tsEngine"`
	TsWS     int64  `json:"tsWs"`
	Pose     Pose   `json:"pose"`
}

type Action struct {
	Action int `json:"action"`
	Key    int `json:"key"`
	Type   int `json:"type"`
}

func InitWsServer(r *gin.Engine) {
	// ws := r.Group("ws", middleware.License())
	ws := r.Group("/ws")
	ws.GET("", handleWebSocket)
}

var (
	upgrader = websocket.Upgrader{
		CheckOrigin: func(r *http.Request) bool {
			return true
		},
	}
)

func handleWebSocket(c *gin.Context) {
	conn, err := upgrader.Upgrade(c.Writer, c.Request, nil)
	if err != nil {
		logrus.Errorf("Failed to upgrade connection to WebSocket: %v", err)
		return
	}
	defer conn.Close()
	go func() {
		for {
			_, msg, err := conn.ReadMessage()
			if err != nil {
				logrus.Errorf("ws read message error: %v", err)
				return
			}
			fmt.Println(string(msg))
			var data []Action
			if err := json.Unmarshal(msg, &data); err != nil {
				logrus.Errorf("ws unmarshal fail, error: %v", err)
				continue
			}

			logrus.Info("receive message: ", data)
			recAction := map[int]config.ActionData{}
			for _, v := range data {
				recAction[v.Action] = config.ActionData{
					Type:  v.Type,
					Value: v.Key,
				}
			}
			global.Config.Action = recAction
		}
	}()

	ticker := time.NewTicker(30 * time.Millisecond)
	defer ticker.Stop()
	for {
		// 等待定时器触发的事件
		<-ticker.C
		msg := PoseData{
			Ts: time.Now().UnixMilli(),
		}
		pose := map[string][][]float64{}
		global.Sources.Range(func(key, value interface{}) bool {
			pos := value.(*source.Source)
			if data, err := pos.LastData(); err != nil {
				logrus.Error(err)
			} else {
				if pos.Xbox != nil {
					pose[fmt.Sprintf("%v", pos.Xbox.ID)] = data.Objs
				}
			}
			return true
		})
		msg.Pose = pose
		message, _ := json.Marshal(msg)
		err = conn.WriteMessage(websocket.TextMessage, message)
		if err != nil {
			logrus.Errorf("Failed to send message to WebSocket: %v", err)
			break
		}
	}
}
