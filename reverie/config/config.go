package config

import (
	"io"
	"os"
	"reverie/global"
	"reverie/model/config"

	logs "github.com/sirupsen/logrus"
	"github.com/spf13/viper"
	"gopkg.in/natefinch/lumberjack.v2"
)

func init() {
	viper.AllowEmptyEnv(true)
	viper.AutomaticEnv()
}

func InitConfig(path string) {
	viper.AddConfigPath(path)
	viper.SetConfigType("yaml")
	viper.SetConfigName("config")

	if err := viper.ReadInConfig(); err != nil {
		logs.Fatal(err)
	}

	switch viper.GetString("log.level") {
	case "debug":
		logs.SetLevel(logs.DebugLevel)
	case "info":
		logs.SetLevel(logs.InfoLevel)
	case "warn":
		logs.SetLevel(logs.WarnLevel)
	case "error":
		logs.SetLevel(logs.ErrorLevel)
	default:
		logs.SetLevel(logs.InfoLevel)
	}
	logger := &lumberjack.Logger{
		Filename:   "./logrus.log",
		MaxSize:    500,
		MaxBackups: 3,
		MaxAge:     30,
		Compress:   true,
	}
	logs.SetOutput(io.MultiWriter(os.Stdout, logger))
	logs.Trace("trace")
	// logs.SetOutput(logger, os.Stdout) // logrus 设置日志的输出方式
	viper.WatchConfig()

	logs.Infoln("---------meta config list--------")
	for _, key := range viper.AllKeys() {
		logs.Infoln(key, ":", viper.Get(key))
	}
	logs.Infoln("----------------------------------")

	global.Config = config.NewConfig()
}
