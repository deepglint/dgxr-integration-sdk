package ws

import (
	"encoding/json"
	"meta/common/models/games"
	"meta/common/models/sources"
	"meta/global"
	"net/http"
	"time"

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
			var data MessageData
			if err := json.Unmarshal(msg, &data); err != nil {
				logrus.Errorf("ws unmarshal fail, error: %v", err)
				continue
			}
			logrus.Info("receive message: ", data.PersonID)
			// 增加 id 和游戏 name 的校验，相同则跳过
			// 后续增加锁
			// 增加 设置新的 id 后，游戏的状态设置为初始化状态
			gameData := global.Games.GetGames(games.GameName(data.Game))
			if gameData != nil {
				if gameData.PersonID == data.PersonID {
					continue
				}
			}
			global.Games.SetGames(games.GameName(data.Game), &global.GameData{
				Game:           games.GameName(data.Game),
				PersonID:       data.PersonID,
				StartEnable:    false,
				OriginalSource: sources.SourceData{},
				Jump:           false,
			})
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
		for k, v := range global.Sources {
			if data, err := v.LastData(); err != nil {
				logrus.Error(err)
			} else {
				pose[k] = data.Objs
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
