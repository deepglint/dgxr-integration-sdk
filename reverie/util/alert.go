package util

import (
	"encoding/json"
	"net/http"
	"strings"
	"time"

	log "github.com/sirupsen/logrus"
)

var loc *time.Location

type larkRequest struct {
	MsgType string `json:"msg_type"`
	Card    Card   `json:"card"`
}
type Config struct {
	WideScreenMode bool `json:"wide_screen_mode"`
}
type Title struct {
	Content string `json:"content"`
	Tag     string `json:"tag"`
}
type Header struct {
	Template string `json:"template"`
	Title    Title  `json:"title"`
}
type URLVal struct {
	URL        string `json:"url" default:"https://www.feishu.com"`
	AndroidURL string `json:"android_url" default:"https://developer.android.com/"`
	IosURL     string `json:"ios_url" default:"lark://msgcard/unsupported_action"`
	PcURL      string `json:"pc_url" default:"https://www.feishu.com"`
}
type Href struct {
	URLVal URLVal `json:"urlVal"`
}
type ZhCn struct {
	Tag     string `json:"tag"`
	Content string `json:"content"`
	Href    Href   `json:"href"`
}
type I18NElements struct {
	ZhCn []ZhCn `json:"zh_cn"`
}
type Card struct {
	MsgType      string       `json:"msg_type"`
	Config       Config       `json:"config"`
	Header       Header       `json:"header"`
	I18NElements I18NElements `json:"i18n_elements"`
}

func init() {
	loc, _ = time.LoadLocation("Asia/Shanghai")
}

type Alert struct {
	Name     string
	Space    string
	Detail   string
	Status   bool
	Describe string
}

// LarkToMarkDown 飞书平台告警
func LarkToMarkDown(alert Alert) string {
	var alertInfo = ""
	alertInfo += "**场地**: " + alert.Space
	alertInfo += "\n**时间**: " + time.Now().Format("2006-01-02 15:04:05")
	alertInfo += "\n**详情**: " + alert.Detail
	alertInfo += "\n**描述**: " + alert.Describe
	alertInfo += "\n<at id=all></at>"
	return alertInfo
}

func LarkAlert(address string, markdownText string, alert Alert) {
	larkReq := larkRequest{}
	title := ""
	if alert.Status {
		title += "告警:" + alert.Name
		larkReq.Card.Header.Template = "red"
	} else {
		title += "恢复:" + alert.Name
		larkReq.Card.Header.Template = "green"
	}
	larkReq.Card.Header.Title.Content = title
	larkReq.MsgType = "interactive"
	larkReq.Card.Config.WideScreenMode = true
	larkReq.Card.MsgType = "interactive"
	larkReq.Card.Header.Title.Tag = "plain_text"

	zh := ZhCn{}
	zh.Tag = "markdown"
	zh.Href.URLVal.URL = "https://www.feishu.com"
	zh.Href.URLVal.AndroidURL = "https://developer.android.com/"
	zh.Href.URLVal.IosURL = "lark://msgcard/unsupported_action"
	zh.Href.URLVal.PcURL = "https://www.feishu.com"

	zh.Content = markdownText
	larkReq.Card.I18NElements.ZhCn = append(larkReq.Card.I18NElements.ZhCn, zh)

	sendMsg(address, larkReq)

	log.Info("alert-center send to lark")
}

func sendMsg(apiUrl string, msg larkRequest) {
	contentType := "application/json"
	sendData, _ := json.Marshal(msg)

	result, err := http.Post(apiUrl, contentType, strings.NewReader(string(sendData)))
	if err != nil {
		log.Error("An feiBook API error has returned", err)
		return
	}
	defer result.Body.Close()
}
