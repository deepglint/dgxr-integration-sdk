package main

import (
	"meta/global"
	"meta/model"
	"meta/servers/grpc"
	"meta/servers/ws"
)

func main() {
	q := model.InitQueue(30)
	model.FrameQ = q
	global.ActionChan = make(chan global.Action, 10)
	go ws.InitServer()
	grpc.Grpc()
	// apps.ActionWs2()
	// apps.ActionWsConductor()
}
