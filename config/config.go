package config

import (
	"meta/global"
	"meta/model/config"

	logs "github.com/sirupsen/logrus"
	"github.com/spf13/viper"
)

func init() {
	viper.AllowEmptyEnv(true)
	viper.AutomaticEnv()
}

func InitConfig(path string) {
	viper.AddConfigPath(path)
	viper.SetConfigType("yaml")
	viper.SetConfigName("config")
	// log lever
	_ = viper.BindEnv("logLevel", "LOG_LEVEL")
	if err := viper.ReadInConfig(); err != nil {
		logs.Fatal(err)
	}

	switch viper.GetString("logLevel") {
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

	viper.WatchConfig()

	logs.Infoln("---------meta config list--------")
	for _, key := range viper.AllKeys() {
		logs.Infoln(key, ":", viper.Get(key))
	}
	logs.Infoln("----------------------------------")

	global.Config = config.NewConfig()
}
