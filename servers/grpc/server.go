package grpc

import (
	"context"
	"encoding/json"

	"meta/model"
	pb "meta/servers/grpc/proto"

	"log"
	"net"
	"os"
	"time"

	"google.golang.org/grpc"
)

var person string

// 实现 ThreeDimSkelServer 接口
type server struct {
	pb.UnimplementedThreeDimSkelServer
}

// 保存请求入参
func saveRequest(data []byte) {
	// 打开文件，如果文件不存在则创建，以追加模式写入，设置权限为 0644
	file, err := os.OpenFile("./request_data.json", os.O_WRONLY|os.O_CREATE|os.O_APPEND, 0644)
	if err != nil {
		log.Println("create file fail", err.Error())
		return
	}
	defer file.Close()
	// 检查文件是否为空
	fileInfo, _ := file.Stat()
	if fileInfo.Size() > 0 {
		// 如果文件不为空，则在写入数据之前先添加换行符
		_, _ = file.WriteString("\n")
	}
	// 写入数据
	_, err = file.Write(data)
	if err != nil {
		log.Println("save request fail", err.Error())
		return
	}
}

type ReqDataInfo struct {
	ReqInfo  *pb.Request `json:"reqInfo"`
	RecvTime time.Time   `json:"recvTime"`
}

// 实现 SendThreeDimSkelData 方法
func (s *server) SendThreeDimSkelData(ctx context.Context, req *pb.Request) (*pb.Response, error) {
	// 处理接收到的请求
	message := ReqDataInfo{
		ReqInfo:  req,
		RecvTime: time.Now(),
	}
	if data, err := json.Marshal(message); err == nil {
		go saveRequest(data)
	}

	for k, v := range req.Result {
		if k == "999001" && len(v.ThreeDim) > 0 {
			if person == "" {
				// TODO判断每一个人是否举手，如果举手则作为识别人员
				for k, _ := range v.ThreeDim {
					person = k
					break
				}
			}
			if _, ok := v.ThreeDim[person]; ok {
				model.FrameQ.Enqueue(*v.ThreeDim[person])
				go model.FrameQ.Jump(v.ThreeDim[person], req.FrameId, person, time.Now().UnixMilli())
			} else {
				person = ""
				// TODO未识别到操作人员
			}
		}
	}

	response := &pb.Response{
		MsgCode: 0,
		Message: "success",
	}

	return response, nil
}

func Grpc() {
	// go pb.Client1()
	// 监听的地址和端口
	listenAddress := "0.0.0.0:50051" // 替换为实际的监听地址和端口

	// 创建 gRPC 服务器
	lis, err := net.Listen("tcp", listenAddress)
	if err != nil {
		log.Fatalf("无法监听端口：%v", err)
	}

	// 创建 gRPC 服务器实例
	s := grpc.NewServer()

	// 注册服务
	pb.RegisterThreeDimSkelServer(s, &server{})

	log.Printf("开始监听 %s", listenAddress)
	// 启动服务器
	if err := s.Serve(lis); err != nil {
		log.Fatalf("无法启动服务器：%v", err)
	}
}
