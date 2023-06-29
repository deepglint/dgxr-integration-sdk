package ws

import (
	"log"
	"net/url"
	"os"
	"os/signal"

	"github.com/gorilla/websocket"
)

var ClientMap = make(map[string]*websocket.Conn)

func Client(address, path, clientName string) {
	interrupt := make(chan os.Signal, 1)
	signal.Notify(interrupt, os.Interrupt)

	u := url.URL{Scheme: "ws", Host: address, Path: path}
	log.Printf("connecting to %s", u.String())

	c, _, err := websocket.DefaultDialer.Dial(u.String(), nil)
	if err != nil {
		log.Println("dial:", err)
		return
	}
	defer c.Close()
	ClientMap[clientName] = c
	defer delete(ClientMap, clientName)

	// 等待中断信号
	<-interrupt
	log.Println("a ...any key to exit")
}
