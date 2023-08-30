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
	Games  *Games
	Log    *Log
}

type Games struct {
	Skiing *Skiing
}

type Skiing struct {
	TiltAngle     float64
	ShoulderWidth float64
	MinElbowAngle float64
	MaxElbowAngle float64
	JumpKneeAngle float64
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

	games := &Games{
		Skiing: &Skiing{
			TiltAngle:     viper.GetFloat64("games.skiing.tiltAngle"),
			ShoulderWidth: viper.GetFloat64("games.skiing.shoulderWidth"),
			MinElbowAngle: viper.GetFloat64("games.skiing.minElbowAngle"),
			MaxElbowAngle: viper.GetFloat64("games.skiing.maxElbowAngle"),
			JumpKneeAngle: viper.GetFloat64("games.skiing.jumpKneeAngle"),
		},
	}

	return &Config{
		Rules:  rules,
		Action: map[int]int{},
		Source: *source,
		Games:  games,
		Log: &Log{
			Level: viper.GetString("logLevel"),
		},
	}
}
