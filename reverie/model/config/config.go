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
	Name         string
	Devices      Devices
	AlertAddress string
	XDirection   string
	YDirection   string
}

type Devices struct {
	Cam  Cam
	Host Host
}

type Cam struct {
	Ip []string
}

type Host struct {
	Ip []string
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
	// name: 公司 9 楼小灵境
	// devices:
	//   cam:
	// 	ip:
	// 	- 192.168.103.51
	// 	- 192.168.103.52
	// 	- 192.168.103.53
	// 	- 192.168.103.54
	// 	- 192.168.103.55
	// 	- 192.168.103.56
	// 	- 192.168.103.57
	// 	- 192.168.103.58
	//   host:
	// 	ip:
	// 	- 192.168.103.61
	// alert: https://open.feishu.cn/open-api
	space := Space{
		Name: viper.GetString("space.name"),
		Devices: Devices{
			Cam:  Cam{Ip: viper.GetStringSlice("space.devices.cam.ip")},
			Host: Host{Ip: viper.GetStringSlice("space.devices.host.ip")},
		},
		AlertAddress: viper.GetString("space.alert"),
		XDirection:   viper.GetString("space.xDirection"),
		YDirection:   viper.GetString("space.yDirection"),
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
