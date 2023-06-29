package apps

import (
	"encoding/json"
	"log"
	"meta/model"
	"meta/servers/grpc/proto"
	"meta/servers/ws"
	"time"
)

func ActionWsConductor() {
	go ws.Client(wsAddress, "", "client1")
	time.Sleep(2 * time.Second)
	for {
		if ws.ClientMap["client1"] != nil {
			_, message, err := ws.ClientMap["client1"].ReadMessage()
			if err != nil {
				log.Println("read:", err)
				go ws.Client(wsAddress, "", "client1")
				time.Sleep(10 * time.Second)
				continue
			}
			data := Message{}
			err = json.Unmarshal(message, &data)
			if err != nil {
				continue
			}
			obj := proto.Data{
				Objs: make([]*proto.DataKeypoints, 25),
			}
			obj.Objs[5] = &proto.DataKeypoints{Value: data.A}
			obj.Objs[7] = &proto.DataKeypoints{Value: data.B}
			obj.Objs[9] = &proto.DataKeypoints{Value: data.C}
			obj.Objs[22] = &proto.DataKeypoints{Value: data.D}
			obj.Objs[6] = &proto.DataKeypoints{Value: data.E}
			obj.Objs[8] = &proto.DataKeypoints{Value: data.F}
			obj.Objs[10] = &proto.DataKeypoints{Value: data.G}
			obj.Objs[23] = &proto.DataKeypoints{Value: data.H}
			model.FrameQ.Enqueue(obj)
			model.FrameQ.Conductor(&obj)
		} else {
			go ws.Client(wsAddress, "", "client1")
			time.Sleep(10 * time.Second)
		}
	}
}
