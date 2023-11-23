package db

import (
	"fmt"
	"reverie/global"
	"reverie/model"

	_ "github.com/mattn/go-sqlite3"
	log "github.com/sirupsen/logrus"
	"github.com/spf13/viper"
	"xorm.io/xorm"
	xormLog "xorm.io/xorm/log"
)

func InitDB() {
	if global.Config != nil {
		if engine, err := xorm.NewEngine("sqlite3", fmt.Sprintf("%s/reverie.db?_busy_timeout=300000", "./")); err != nil {
			log.Fatalf("init db failed, err:%v", err)
		} else {
			engine.ShowSQL(true)
			switch viper.GetString(global.Config.Log.Level) {
			case "debug":
				engine.Logger().SetLevel(xormLog.LOG_DEBUG)
			case "info":
				engine.Logger().SetLevel(xormLog.LOG_INFO)
			case "warn":
				engine.Logger().SetLevel(xormLog.LOG_WARNING)
			case "error":
				engine.Logger().SetLevel(xormLog.LOG_ERR)
			default:
				engine.Logger().SetLevel(xormLog.LOG_INFO)
			}
			global.DbEngine = engine
			initTables()
		}
	} else {
		log.Fatal("init pg failed, try to init config first")
	}
}

func initTables() {
	err := global.DbEngine.Sync2(
		new(model.Template),
	)
	if err != nil {
		log.Fatalf("init tables failed, %v", err)
		return
	}
}
