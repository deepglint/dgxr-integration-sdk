package server

import (
	"reverie/source/output/ws"

	"github.com/gin-gonic/gin"
	"github.com/sirupsen/logrus"
)

func InitHttp() {
	r := gin.Default()
	InitLicense(r)
	ws.InitWsServer(r)
	err := r.Run(":8000")
	if err != nil {
		logrus.Fatal("Failed to start server: ", err)
	}
}
