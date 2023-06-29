package ws

import (
	"encoding/json"
	"log"
	"meta/global"
	"net/http"

	"github.com/gin-gonic/gin"
	"github.com/gorilla/websocket"
)

func InitServer() {

	r := gin.Default()

	// WebSocket 路由
	r.GET("/ws", handleWebSocket)
	// 启动服务
	err := r.Run(":8000")
	if err != nil {
		log.Fatal("Failed to start server: ", err)
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
		log.Println("Failed to upgrade connection to WebSocket:", err)
		return
	}

	// 读取客户端发送的消息
	for msg := range global.ActionChan {
		// _, message, err := conn.ReadMessage()
		// if err != nil {
		// 	log.Println("Failed to read message from WebSocket:", err)
		// 	break
		// }

		// log.Println("Received message:", string(message))
		message, _ := json.Marshal(msg)
		// 回复客户端消息
		err = conn.WriteMessage(websocket.TextMessage, message)
		if err != nil {
			log.Println("Failed to send message to WebSocket:", err)
			break
		}
	}

	// 关闭连接
	conn.Close()
}
