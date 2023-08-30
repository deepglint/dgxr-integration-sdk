package ws

import (
	"encoding/json"
	"fmt"
	"net/http"
	"time"

	"meta/global"

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

// [{"action":0,"key":0}]
type Action struct {
	Action int `json:"action"`
	Key    int `json:"key"`
}

func InitServer() {
	r := gin.Default()
	r.GET("/ws", handleWebSocket)
	err := r.Run(":8000")
	if err != nil {
		logrus.Fatal("Failed to start server: ", err)
	}
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
			var data []Action
			if err := json.Unmarshal(msg, &data); err != nil {
				logrus.Errorf("ws unmarshal fail, error: %v", err)
				continue
			}
			logrus.Info("receive message: ", data)
			recAction := map[int]int{}
			for _, v := range data {
				recAction[v.Action] = v.Key
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
		for _, v := range global.Sources {
			if data, err := v.LastData(); err != nil {
				logrus.Error(err)
			} else {
				if v.Xbox != nil {
					pose[fmt.Sprintf("%v", v.Xbox.ID)] = data.Objs
				}
			}
		}
		msg.Pose = pose
		message, _ := json.Marshal(msg)
		err = conn.WriteMessage(websocket.TextMessage, message)
		if err != nil {
			logrus.Errorf("Failed to send message to WebSocket: %v", err)
			break
		}
		// if _, ok := global.Games[games.Skiing]; ok && global.Games[games.Skiing].PersonID != "" {
		// 	if personSource, ok := global.Sources[global.Games[games.Skiing].PersonID]; ok {
		// 		obj, err := personSource.LastData()
		// 		if err != nil {
		// 			logrus.Errorf("Skiing person source error: %v", err)
		// 			return
		// 		}

		// 		msg := Message{
		// 			FrameId:  0,
		// 			PersonId: global.Games[games.Skiing].PersonID,
		// 			TsEngine: time.Now().UnixMilli(),
		// 			TsWS:     time.Now().UnixMilli(),
		// 			Pose: Pose{
		// 				KeyPoints: obj.Objs,
		// 			},
		// 		}
		// 		message, _ := json.Marshal(msg)
		// 		err = conn.WriteMessage(websocket.TextMessage, message)
		// 		if err != nil {
		// 			log.Println("Failed to send message to WebSocket:", err)
		// 			break
		// 		}
		// 	}
		// }
	}
}
