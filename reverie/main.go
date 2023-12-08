package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/url"
	"reverie/config"
	"reverie/db"
	"reverie/global"
	"reverie/server"
	"reverie/source/input/grpc"
	"time"

	"github.com/gorilla/websocket"
	"github.com/kardianos/service"
	"github.com/sirupsen/logrus"
)

type program struct{}

func (p *program) Start(s service.Service) error {
	go p.run()
	return nil
}

func (p *program) Stop(s service.Service) error {
	logrus.Infoln("Service is stopping...")
	return nil
}

func main() {
	prg := &program{}
	prg.run()
	// // 获取当前可执行文件的路径
	// exePath, _ := os.Executable()
	// exeDir := filepath.Dir(exePath)
	// if err := os.Chdir(exeDir); err != nil {
	// 	logrus.Fatal(err)
	// }

	// svcConfig := &service.Config{
	// 	Name:        "Alpha-DGMeta",
	// 	DisplayName: "alpha meta Service",
	// 	Description: "Integrated meta space action recognition, 3dpos, virtual controller and other services",
	// }

	// prg := &program{}
	// s, err := service.New(prg, svcConfig)
	// if err != nil {
	// 	logrus.Fatal(err)
	// }

	// if len(os.Args) > 1 {
	// 	err = service.Control(s, os.Args[1])
	// 	if err != nil {
	// 		logrus.Fatal(err)
	// 	}
	// 	return
	// }

	// err = s.Run()
	// if err != nil {
	// 	logrus.Fatal(err)
	// }
}

func (p *program) run() {
	config.InitConfig("./config")
	db.InitDB()
	go global.CheckLicense()
	go server.InitHttp()
	global.XboxDevice = global.NewXboxPool(10)
	go global.UpdateTemplate()
	go Client("192.168.30.147:16666", "", "test")
	defer global.XboxDevice.CloseAllXbox()
	grpc.Grpc()
}

func Client(address, path, clientName string) {
	for {
		time.Sleep(1 * time.Second)
		u := url.URL{Scheme: "ws", Host: address, Path: path}
		log.Printf("connecting to %s", u.String())
		c, _, err := websocket.DefaultDialer.Dial(u.String(), nil)
		if err != nil {
			log.Println("dial:", err)
			continue
		}
		defer c.Close()
		for v := range global.Head {
			b, _ := json.Marshal(v)
			log.Println(string(b))
			err2 := c.WriteMessage(websocket.TextMessage, b)
			if err2 != nil {
				fmt.Println(err2)
				break
			}
		}
		fmt.Println("send over")
	}
}
