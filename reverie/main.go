package main

import (
	"reverie/config"
	"reverie/db"
	"reverie/global"
	"reverie/server"
	"reverie/source/input/grpc"

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
	// 获取当前可执行文件的路径
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
	defer global.XboxDevice.CloseAllXbox()
	grpc.Grpc()
}
