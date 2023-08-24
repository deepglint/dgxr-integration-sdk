package main

import (
	"fmt"

	"meta/config"
	"meta/global"
	"meta/source/input/grpc"
	"meta/source/output/ws"

	"github.com/kardianos/service"
)

type program struct{}

func (p *program) Start(s service.Service) error {
	go p.run()
	return nil
}

func (p *program) Stop(s service.Service) error {

	fmt.Println("Service is stopping...")
	return nil
}

func main() {
	prg := &program{}
	prg.run()
	// // 获取当前可执行文件的路径
	// exePath, _ := os.Executable()
	// exeDir := filepath.Dir(exePath)
	// if err := os.Chdir(exeDir); err != nil {
	// 	logs.Fatal(err)
	// }

	// svcConfig := &service.Config{
	// 	Name:        "Alpha-DGMeta",
	// 	DisplayName: "alpha meta Service",
	// 	Description: "Integrated meta space action recognition, 3dpos, virtual controller and other services",
	// }

	// prg := &program{}
	// s, err := service.New(prg, svcConfig)
	// if err != nil {
	// 	logs.Fatal(err)
	// }

	// if len(os.Args) > 1 {
	// 	err = service.Control(s, os.Args[1])
	// 	if err != nil {
	// 		logs.Fatal(err)
	// 	}
	// 	return
	// }

	// err = s.Run()
	// if err != nil {
	// 	logs.Fatal(err)
	// }
}

func (p *program) run() {
	config.InitConfig("./config")
	global.InitSources()
	go ws.InitServer()
	grpc.Grpc()
}
