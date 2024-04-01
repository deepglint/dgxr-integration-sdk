package global

import (
	"fmt"
	"io/ioutil"
	"net/http"
	"regexp"
	"reverie/util"
	"strconv"
	"time"

	"github.com/sirupsen/logrus"
)

type HostInfo struct {
	Name   string
	Code   int
	Detail string
	Ip     string
}

func Alert() {
	n := 0
	alertStatus := false
	for {
		time.Sleep(30 * time.Second)
		if (len(Config.Space.Devices.Host.Ip)) >= 1 {
			info := PaseHost991(Config.Space.Devices.Host.Ip[0])
			if info.Code == 0 {
				n = 0
				if alertStatus {
					alert := util.Alert{
						Name:     info.Name,
						Space:    Config.Space.Name,
						Detail:   info.Detail,
						Status:   false,
						Describe: fmt.Sprintf("IP地址：%s，详情请访问 http://%s:9400/vars/*t_latency_x_merge_frames_qps*", Config.Space.Devices.Host.Ip[0], Config.Space.Devices.Host.Ip[0]),
					}
					s := util.LarkToMarkDown(alert)
					util.LarkAlert(Config.Space.AlertAddress, s, alert)
					alertStatus = false
				}
			} else {
				n++
				if n > 5 {
					alert := util.Alert{
						Name:     info.Name,
						Space:    Config.Space.Name,
						Detail:   info.Detail,
						Describe: fmt.Sprintf("IP地址：%s，详情请访问 http://%s:9400/vars/*t_latency_x_merge_frames_qps*", Config.Space.Devices.Host.Ip[0], Config.Space.Devices.Host.Ip[0]),
						Status:   true,
					}
					s := util.LarkToMarkDown(alert)
					util.LarkAlert(Config.Space.AlertAddress, s, alert)
					n = 0
					alertStatus = true
				}
			}
		}

	}
}

func PaseHost991(Ip string) HostInfo {
	r, err := http.Get(fmt.Sprintf("http://%s:9400/vars/*t_latency_x_merge_frames_qps*", Ip))
	if err != nil {
		logrus.Error("get 991 err:", err)
		return HostInfo{Code: 404, Name: "设备异常，具体原因未知！"}
	}
	defer r.Body.Close()
	if r.StatusCode != 200 {
		// TODO 991访问失败或则服务不通，尝试重试 5 次
		return HostInfo{Code: r.StatusCode, Name: "融合服务器异常", Detail: "融合服务器状态码异常", Ip: Ip}
	}
	body, _ := ioutil.ReadAll(r.Body)

	re := regexp.MustCompile(`<span id="[^"]*">(\d+)</span>`)

	match := re.FindStringSubmatch(string(body))
	if len(match) > 1 {
		if i, err := strconv.Atoi(match[1]); err == nil {
			if i != 30 {
				// 融合帧小于 30，
				return HostInfo{Code: r.StatusCode, Name: "融合帧异常", Detail: "融合服务器帧数小于 30 帧，目前帧数：" + match[1], Ip: Ip}
			}
		}
	}
	return HostInfo{Code: 0, Name: "设备一切正常！"}
}
