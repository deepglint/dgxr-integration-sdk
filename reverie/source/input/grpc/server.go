package grpc

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"net"
	"os"
	"sync"
	"time"

	"reverie/action"
	"reverie/global"
	sources "reverie/model/source"
	pb "reverie/source/input/grpc/proto"
	"reverie/util"

	"github.com/sirupsen/logrus"
	"github.com/spf13/viper"
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
	if viper.GetBool("record") {
		message := ReqDataInfo{
			ReqInfo:  req,
			RecvTime: time.Now(),
		}
		if data, err := json.Marshal(message); err == nil {
			go saveRequest(data)
		}
	}
	sourcesMap := make(map[string]*sources.Source)
	global.Sources.Range(func(key, value interface{}) bool {
		id := key.(string)
		val := value.(*sources.Source)
		sourcesMap[id] = val
		return true
	})
	if len(req.Result) == 0 {
		// Convert global.Sources to a map type

		for id, val := range sourcesMap {
			if val.Xbox != nil {
				global.XboxDevice.ReturnDevice(val.Xbox)
			}
			global.Sources.Delete(id)
		}
		global.Sources = sync.Map{}
	}
	for k, v := range req.Result {
		if k == "999001" && len(v.ThreeDim) > 0 {
			// 删除离开的人员
			for id, val := range sourcesMap {
				if _, ok := v.ThreeDim[id]; !ok {
					if val.Xbox != nil {
						global.XboxDevice.ReturnDevice(val.Xbox)
					}
					global.Sources.Delete(id)
				}
			}
			// 数据添加到数据源
			for id, data := range v.ThreeDim {
				obj := sources.SourceData{
					Objs: [][]float64{},
				}
				for k, v := range data.Objs {
					unifyValue := util.UnifyCoordinate(global.Config.Space.XDirection, global.Config.Space.YDirection, v.Value)
					if v.Value[0] == 0 && v.Value[1] == 0 && v.Value[2] == 0 {
						if value, ok := sourcesMap[id]; ok {
							if sd, err := value.LastData(); err == nil {
								obj.Objs = append(obj.Objs, sd.Objs[k])
							}
						} else {
							obj.Objs = append(obj.Objs, unifyValue)
						}
					} else {
						obj.Objs = append(obj.Objs, unifyValue)
					}
				}
				if value, ok := sourcesMap[id]; ok {
					if value.Xbox == nil {
						value.Xbox = global.XboxDevice.GetDevice()
					}
					value.Enqueue(obj)
				} else {
					source := *sources.InitSource(global.Config.Source.Cap)
					source.Xbox = global.XboxDevice.GetDevice()
					source.Enqueue(obj)
					global.Sources.Store(id, &source)
				}
				temMap := map[int32]bool{}
				for _, v := range data.RecActions {
					if v.Confidence > 0.6 {
						temMap[v.Action] = true
						logrus.Debugf("model action %v: %s", v.Action, action.Action(v.Action).String())
						if val, ok := global.Sources.Load(id); ok {
							pos := val.(*sources.Source)
							action.ModelToXbox(pos, v.Action)
						}
					}
				}
				// TODO 单独判断是否含有这几个动作在data.RecActions，如果没有设置为false
				if _, ok := temMap[20]; !ok {
					if val, ok := global.Sources.Load(id); ok {
						pos := val.(*sources.Source)
						pos.FastRun = false
					}
				}
				if _, ok := temMap[21]; !ok {
					if val, ok := global.Sources.Load(id); ok {
						pos := val.(*sources.Source)
						pos.Butterfly = false
					}
				}
				if _, ok := temMap[22]; !ok {
					if val, ok := global.Sources.Load(id); ok {
						pos := val.(*sources.Source)
						pos.FreeStyle = false
					}
				}
			}
			go action.ActionToXbox()
		} else {
			for id, val := range sourcesMap {
				if val.Xbox != nil {
					global.XboxDevice.ReturnDevice(val.Xbox)
				}
				global.Sources.Delete(id)
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
