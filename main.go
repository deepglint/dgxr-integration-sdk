package main

import (
	"meta/config"
	"meta/global"
	"meta/servers/grpc"
	"meta/servers/ws"
)

func main() {
	config.InitConfig("./config")
	global.InitSources()
	// TODO vjoy
	global.InitVjoy()
	go games.InitGames()
	go ws.InitServer()
	grpc.Grpc()
}
