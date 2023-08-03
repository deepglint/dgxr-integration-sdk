package grpc

import (
	"context"
	"encoding/json"
	"fmt"
	"math"

	"meta/common/models/sources"
	"meta/global"
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
	if len(req.Result) == 0 {
		global.Sources = map[string]*sources.Source{}
	}
	for k, v := range req.Result {
		if k == "999001" && len(v.ThreeDim) > 0 {
			// 删除离开的人员
			for id, _ := range global.Sources {
				if _, ok := v.ThreeDim[id]; !ok {
					delete(global.Sources, id)
				}
			}
			// 数据添加到数据源
			for id, data := range v.ThreeDim {
				obj := sources.SourceData{
					Objs: [][]float64{},
				}
				for _, v := range data.Objs {
					obj.Objs = append(obj.Objs, []float64{float64(v.Value[0]), float64(v.Value[1]), float64(v.Value[2])})
				}
				if value, ok := global.Sources[id]; ok {
					value.Enqueue(obj)
				} else {
					source := *sources.InitSource(global.Config.Source.Cap)
					source.Enqueue(obj)
					global.Sources[id] = &source
				}
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
	// 监听的地址和端口
	listenAddress := fmt.Sprintf("%s:%s", global.Config.Source.Grpc.Host, global.Config.Source.Grpc.Port) // 替换为实际的监听地址和端口

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

// 高斯滤波器函数
func gaussianFilter(data []float64, sigma float64) []float64 {
	size := len(data)
	filteredData := make([]float64, size)

	// 计算高斯核权重
	kernel := make([]float64, 2*size-1)
	sum := 0.0
	for i := range kernel {
		x := float64(i - size + 1)
		kernel[i] = math.Exp(-x*x/(2*sigma*sigma)) / (math.Sqrt(2*math.Pi) * sigma)
		sum += kernel[i]
	}

	// 归一化高斯核权重
	for i := range kernel {
		kernel[i] /= sum
	}

	// 应用高斯滤波器
	for i := range data {
		for j, k := 0, size-1; j < len(kernel); j, k = j+1, k-1 {
			if i+k >= 0 && i+k < size {
				filteredData[i] += data[i+k] * kernel[j]
			}
		}
	}

	return filteredData
}
