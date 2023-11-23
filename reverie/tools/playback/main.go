package main

import (
	"bufio"
	"context"
	"encoding/json"
	"fmt"
	"os"
	"time"

	pb "reverie/source/input/grpc/proto"

	"github.com/sirupsen/logrus"
	"google.golang.org/grpc"
)

type ReqDataInfo struct {
	ReqInfo  *pb.Request `json:"reqInfo"`
	RecvTime time.Time   `json:"recvTime"`
}

func main() {
	//解析保存的grpc数据进行分析
	Grpc()
	// 通过grpc client进行推送

}

func Grpc() {
	time.Sleep(1 * time.Second)
	// gRPC 服务器地址
	serverAddress := "127.0.0.1:50051" // 替换为实际的服务器地址

	// 创建与服务器的连接
	conn, err := grpc.Dial(serverAddress, grpc.WithInsecure())
	if err != nil {
		logrus.Fatal("无法连接到服务器：", err)
	}
	defer conn.Close()

	// 创建 gRPC 客户端
	client := pb.NewThreeDimSkelClient(conn)
	// 打开文件
	file, err := os.Open("./walk.txt") // 替换为你要读取的文件路径
	if err != nil {
		logrus.Fatal(err)
	}
	defer file.Close()

	// 创建一个 Scanner 对象来按行读取文件
	scanner := bufio.NewScanner(file)
	n := 0
	// 循环读取每一行
	for scanner.Scan() {
		n++
		// time.Sleep(1 * time.Second)
		time.Sleep(30 * time.Millisecond)
		line := scanner.Text()
		data := &ReqDataInfo{}
		err = json.Unmarshal([]byte(line), data)
		if err != nil {
			// logrus.Info("解析失败", err, n, data)
		}
		if data.ReqInfo == nil || data.ReqInfo.FrameId == "" {
			continue
		}
		// 发送请求到服务器
		sentMsg := &pb.Request{
			FrameId:   data.ReqInfo.FrameId,
			Result:    data.ReqInfo.Result,
			TimeStamp: data.ReqInfo.TimeStamp,
		}
		fmt.Println(data.ReqInfo.FrameId)
		_, err := client.SendThreeDimSkelData(context.Background(), sentMsg)
		if err != nil {
			logrus.Fatal("发送请求失败：", err)
		}
	}

	// 检查是否有错误发生
	if err := scanner.Err(); err != nil {
		logrus.Fatal(err)
	}

}
