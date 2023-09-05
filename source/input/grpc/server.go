package grpc

import (
	"context"
	"fmt"
	"log"
	"net"
	"os"
	"time"

	"meta/action"
	"meta/global"
	sources "meta/model/source"
	pb "meta/source/input/grpc/proto"

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
	// message := ReqDataInfo{
	// 	ReqInfo:  req,
	// 	RecvTime: time.Now(),
	// }
	// if data, err := json.Marshal(message); err == nil {
	// 	go saveRequest(data)
	// }
	if len(req.Result) == 0 {
		for id, val := range global.Sources {
			if val.Xbox != nil {
				global.XboxDevice.ReturnDevice(val.Xbox)
			}
			delete(global.Sources, id)
		}
		global.Sources = map[string]*sources.Source{}
	}
	for k, v := range req.Result {
		if k == "999001" && len(v.ThreeDim) > 0 {
			// 删除离开的人员
			for id, val := range global.Sources {
				if _, ok := v.ThreeDim[id]; !ok {
					if val.Xbox != nil {
						global.XboxDevice.ReturnDevice(val.Xbox)
					}
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
					if value.Xbox == nil {
						value.Xbox = global.XboxDevice.GetDevice()
					}
					value.Enqueue(obj)
				} else {
					source := *sources.InitSource(global.Config.Source.Cap)
					source.Xbox = global.XboxDevice.GetDevice()
					source.Enqueue(obj)
					global.Sources[id] = &source
				}
				// TODO 按按键
				action.RuleToXbox(global.Sources[id])
				for _, v := range data.RecActions {
					if v.Confidence > 0.7 {
						action.ModelToXbox(global.Sources[id], v.Action)
					}
				}
			}
		} else {
			for id, val := range global.Sources {
				if val.Xbox != nil {
					global.XboxDevice.ReturnDevice(val.Xbox)
				}
				delete(global.Sources, id)
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
	listenAddress := fmt.Sprintf("%s:%s", global.Config.Source.Grpc.Host, global.Config.Source.Grpc.Port)
	lis, err := net.Listen("tcp", listenAddress)
	if err != nil {
		log.Fatalf("无法监听端口：%v", err)
	}
	s := grpc.NewServer()

	pb.RegisterThreeDimSkelServer(s, &server{})
	log.Printf("开始监听 %s", listenAddress)
	if err := s.Serve(lis); err != nil {
		log.Fatalf("无法启动服务器：%v", err)
	}
}
