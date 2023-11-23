package main

import (
	"bufio"
	"bytes"
	"encoding/json"
	"fmt"
	"net/http"
	"os"
	"time"

	pb "reverie/source/input/grpc/proto"

	"github.com/sirupsen/logrus"
)

type ReqDataInfo struct {
	ReqInfo  *pb.Request `json:"reqInfo"`
	RecvTime time.Time   `json:"recvTime"`
}

func main() {
	slidRight := ReqData{
		Id:          2002,
		SingleFrame: 0,
		KeyNode:     []int{5, 6, 7, 9, 11, 12, 13, 14, 21, 22},
		Name:        "SlidRight",
		Score:       0.2,
		PathLen:     0.5,
	}
	build("./slidRight.json", slidRight)

	slidUp := ReqData{
		Id:          2003,
		KeyNode:     []int{5, 6, 7, 9, 11, 12, 13, 14, 21, 22},
		SingleFrame: 0,
		Name:        "SlidUp",
		Score:       0.2,
		PathLen:     0.5,
	}
	build("./slidUp.json", slidUp)

	slidDown := ReqData{
		Id:          2004,
		SingleFrame: 0,
		KeyNode:     []int{5, 6, 8, 10, 11, 12, 13, 14, 21, 23},
		Name:        "SlidDown",
		Score:       0.25,
		PathLen:     0.6,
	}
	build("./slidDown.json", slidDown)

	handAlway := ReqData{
		Id:          2005,
		SingleFrame: 0,
		KeyNode:     []int{5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 22, 23},
		Name:        "HandAlway",
		Score:       0.15,
		PathLen:     0.7,
	}
	build("./handAlway.json", handAlway)

	handClose := ReqData{
		Id:          2006,
		SingleFrame: 0,
		KeyNode:     []int{5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 22, 23},
		Name:        "HandClose",
		Score:       0.15,
		PathLen:     0.5,
	}
	build("./handClose.json", handClose)

	close := ReqData{
		Id:          2013,
		SingleFrame: 1,
		KeyNode:     []int{5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 22, 23},
		Name:        "HandsCross",
		Score:       0.25,
		PathLen:     0.4,
	}
	build("./close.json", close)

	LeftArmToForward := ReqData{
		Id:          2008,
		SingleFrame: 1,
		KeyNode:     []int{5, 6, 7, 9, 11, 12, 13, 14, 21, 22},
		Name:        "ArmToForward",
		Score:       0.2,
		PathLen:     0.4,
	}
	build("./前指.json", LeftArmToForward)

	RightArmToBack := ReqData{
		Id:          2009,
		SingleFrame: 1,
		KeyNode:     []int{5, 6, 8, 10, 11, 12, 13, 14, 21, 23},
		Name:        "ArmToBack",
		Score:       0.15,
		PathLen:     0.4,
	}
	build("./hou.json", RightArmToBack)

	ArmToLeft := ReqData{
		Id:          2010,
		SingleFrame: 1,
		KeyNode:     []int{5, 6, 7, 9, 11, 12, 13, 14, 21, 22},
		Name:        "ArmToLeft",
		Score:       0.2,
		PathLen:     0.4,
	}
	build("./zuo.json", ArmToLeft)

	ArmToRight := ReqData{
		Id:          2011,
		SingleFrame: 1,
		KeyNode:     []int{5, 6, 8, 10, 11, 12, 13, 14, 21, 23},
		Name:        "ArmToRight",
		Score:       0.2,
		PathLen:     0.4,
	}
	build("./you.json", ArmToRight)
}

func build(fileName string, data ReqData) {
	// 打开文件
	file, err := os.Open(fileName) // 替换为你要读取的文件路径
	if err != nil {
		logrus.Fatal(err)
	}
	defer file.Close()

	// 创建一个 Scanner 对象来按行读取文件
	scanner := bufio.NewScanner(file)
	n := 0
	posArrs := [][][]float32{}
	// 循环读取每一行
	for scanner.Scan() {
		n++
		line := scanner.Text()
		data := &ReqDataInfo{}
		err = json.Unmarshal([]byte(line), data)
		if err != nil {
			// logrus.Info("解析失败", err, n, data)
		}
		if data.ReqInfo == nil || data.ReqInfo.FrameId == "" {
			logrus.Error("解析失败", err, n, data)
			continue
		}
		posArr := [][]float32{}
		for k, v := range data.ReqInfo.Result {
			if k != "999001" || len(v.ThreeDim) < 0 {
				continue
			}
			// 数据添加到数据源
			for _, data := range v.ThreeDim {
				for _, pos := range data.Objs {
					fmt.Println(pos.Value[:3])
					posArr = append(posArr, pos.Value[:3])
				}
				posArrs = append(posArrs, posArr)
				break
			}
		}
	}
	PostHttp(posArrs, data)
}

type ReqData struct {
	Id          int           `json:"id"`
	Name        string        `json:"name"`
	Pos         [][][]float32 `json:"pos"`
	Score       float32       `json:"score"`
	PathLen     float32       `json:"pathLen"`
	SingleFrame int           `json:"singleFrame"`
	KeyNode     []int         `json:"keyNode"`
}

func PostHttp(pos [][][]float32, data ReqData) {
	data.Pos = pos
	// 发送POST请求
	url := "http://10.211.55.3:8000/template/"
	jsonData, err := json.Marshal(data)
	if err != nil {
		logrus.Fatal(err)
	}

	resp, err := http.Post(url, "application/json", bytes.NewBuffer(jsonData))
	if err != nil {
		logrus.Fatal(err)
	}
	defer resp.Body.Close()
	if resp.StatusCode != 201 {
		logrus.Error("请求失败", resp.StatusCode)
	}
}
