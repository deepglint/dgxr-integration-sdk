package config

import (
	"github.com/spf13/viper"
)

func init() {
	viper.AllowEmptyEnv(true)
	viper.AutomaticEnv()
}

type ActionData struct {
	Type  int `json:"type"`
	Value int `json:"value"`
}

type Config struct {
	Source Source
	Space  Space
	Action map[int]ActionData
	Rules  Rules
	Log    *Log
}

type Grpc struct {
	Host string
	Port string
}

type Source struct {
	Cap  int
	Grpc *Grpc
}

type Space struct {
	XDirection string
	YDirection string
}

type Log struct {
	Level string
}

func NewConfig() *Config {

	grpc := &Grpc{
		Host: viper.GetString("source.grpc.host"),
		Port: viper.GetString("source.grpc.port"),
	}

	source := &Source{
		Cap:  viper.GetInt("source.cap"),
		Grpc: grpc,
	}

	space := Space{
		XDirection: viper.GetString("space.xDirection"),
		YDirection: viper.GetString("space.yDirection"),
	}

	rules := Rules{}
	// 绑定配置到结构体
	if err := viper.UnmarshalKey("rules", &rules); err != nil {
		panic(err)
	}

	return &Config{
		Rules:  rules,
		Action: map[int]ActionData{},
		Source: *source,
		Space:  space,
		Log: &Log{
			Level: viper.GetString("logLevel"),
		},
	}
}
