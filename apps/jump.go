package apps

import (
	"encoding/json"
	"log"
	"meta/model"
	"meta/servers/grpc/proto"
	"meta/servers/ws"
	"time"
)

type Message struct {
	A []float32 `json:"A"`
	B []float32 `json:"B"`
	C []float32 `json:"C"`
	D []float32 `json:"D"`
	E []float32 `json:"E"`
	F []float32 `json:"F"`
	G []float32 `json:"G"`
	H []float32 `json:"H"`
}

var wsAddress = "192.168.30.125:7878"

func ActionWs() {
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
				log.Println(err)
				continue
			}
			obj := proto.Data{
				Objs: make([]*proto.DataKeypoints, 25),
			}
			obj.Objs[11] = &proto.DataKeypoints{Value: data.A}
			obj.Objs[13] = &proto.DataKeypoints{Value: data.B}
			obj.Objs[15] = &proto.DataKeypoints{Value: data.C}
			obj.Objs[12] = &proto.DataKeypoints{Value: data.D}
			obj.Objs[14] = &proto.DataKeypoints{Value: data.E}
			obj.Objs[16] = &proto.DataKeypoints{Value: data.F}
			model.FrameQ.Enqueue(obj)
		} else {
			go ws.Client(wsAddress, "", "client1")
			time.Sleep(10 * time.Second)
		}
	}
}
