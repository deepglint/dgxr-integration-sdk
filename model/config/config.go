package config

import (
	"github.com/spf13/viper"
)

func init() {
	viper.AllowEmptyEnv(true)
	viper.AutomaticEnv()
}

type Config struct {
	Source Source
	Action map[int]int
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

	rules := Rules{}
	// 绑定配置到结构体
	if err := viper.UnmarshalKey("rules", &rules); err != nil {
		panic(err)
	}

	return &Config{
		Rules:  rules,
		Action: map[int]int{},
		Source: *source,
		Log: &Log{
			Level: viper.GetString("logLevel"),
		},
	}
}
