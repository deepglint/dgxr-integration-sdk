package controllers

// import (
// 	"fmt"
// 	"math"
// 	"meta/common/rule"
// 	"meta/global"
// 	"time"
// )

// /*
// 拿到数据源
// 根据配置文件获取规则检测的规则类型
// 执行具体的规则逻辑
// */
// var action = false

// func Jump() {
// 	// obj *proto.Data, frameId, PersonId string, ts int64
// 	// obj = glo

// 	personId := []string{}
// 	// 判断是否举手
// 	for id, v := range global.Sources {
// 		obj, err := v.LastData()
// 		if err != nil {
// 			continue
// 		}
// 		if ok := rule.RaiseHandRight(&obj); ok {
// 			personId = append(personId, id)
// 		}
// 	}

// 	if personId != nil {
// 		personSource := global.Sources[personId[0]]
// 		obj, err := personSource.LastData()
// 		if err != nil {
// 			return
// 		}
// 		move := personSource.CalculateAverageAXY(11, 12)
// 		center := personSource.CalculateCenterPoint(obj.Objs[11].Value, obj.Objs[12].Value)
// 		l := personSource.CalculateAverageABC(11, 13, 15)
// 		r := personSource.CalculateAverageABC(12, 14, 16)
// 		m := (l + r) / 2
// 		cl := personSource.CalculateAngle(obj.Objs[11].Value, obj.Objs[13].Value, obj.Objs[15].Value)
// 		cr := personSource.CalculateAngle(obj.Objs[12].Value, obj.Objs[14].Value, obj.Objs[16].Value)
// 		cm := (cl + cr) / 2
// 		cx := 0.0
// 		// TODO kinect 单位是毫米，因此是否移动改为80，灵境值为m，因此改为0.08m
// 		if math.Abs(cr-cl) > 40 || math.Abs((center[0]+center[1])-move) > 0.08 {

// 			fmt.Println("不要移动", cr-cl, math.Abs((center[0]+center[1])-move))

// 		} else {
// 			// TODO对接vjoy
// 			cx = personSource.CalculateRotateAngle(obj.Objs[11].Value, obj.Objs[12].Value)
// 			if (cm - m) > 20 {
// 				// event = "squat"
// 				// TODO对接vjoy
// 				action = true
// 			} else if action == true && cm < 36 { // TODO更改26的值，可以更改延迟时间，26，
// 				// TODO对接vjoy
// 				action = false
// 			}
// 		}
// 		// TODO 推送数据给ws
// 		if len(global.ActionChan) < 8 {
// 			keyPoint := [][]float32{}
// 			for _, v := range obj.Objs {
// 				keyPoint = append(keyPoint, v.Value[:3])
// 			}

// 			global.ActionChan <- global.Action{
// 				PersonId: personId[0],
// 				TsEngine: time.Now().UnixMilli(),
// 				TsWS:     time.Now().UnixMilli(),
// 				Pose: global.Pose{
// 					KeyPoints: keyPoint,
// 					Direction: int(cx), //TODO 待计算
// 				},
// 			}
// 		}
// 	}
// 	//

// 	// l := q.CalculateAverageABC(11, 13, 15)
// 	// r := q.CalculateAverageABC(12, 14, 16)
// 	// m := (l + r) / 2

// 	// cl := CalculateAngle(obj.Objs[11].Value, obj.Objs[13].Value, obj.Objs[15].Value)
// 	// cr := CalculateAngle(obj.Objs[12].Value, obj.Objs[14].Value, obj.Objs[16].Value)
// 	// cm := (cl + cr) / 2

// 	// cx := 0.0
// 	// // TODO kinect 单位是毫米，因此是否移动改为80，灵境值为m，因此改为0.08m
// 	// if math.Abs(cr-cl) > 40 || math.Abs((center[0]+center[1])-move) > 0.08 {
// 	// 	fmt.Println("不要移动", cr-cl, math.Abs((center[0]+center[1])-move))

// 	// } else {
// 	// 	cx = CalculateRotateAngle(obj.Objs[11].Value, obj.Objs[12].Value)
// 	// 	if (cm - m) > 20 {
// 	// 		event = "squat"

// 	// 		action = true

// 	// 	} else if action == true && cm < 36 { // TODO更改26的值，可以更改延迟时间，26，
// 	// 		event = "jump"

// 	// 		action = false
// 	// 	}
// 	// }

// 	// keyPoint := [][]float32{}

// 	// keyPoint = append(keyPoint, Average(obj.Objs[11].Value[:3], obj.Objs[12].Value[:3]))
// 	// keyPoint = append(keyPoint, Average(obj.Objs[0].Value[:3], Average(obj.Objs[11].Value[:3], obj.Objs[12].Value[:3])))
// 	// keyPoint = append(keyPoint, obj.Objs[0].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[21].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[5].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[7].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[9].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[22].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[6].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[8].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[10].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[23].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[11].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[13].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[15].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[17].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[12].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[14].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[16].Value[:3])
// 	// keyPoint = append(keyPoint, obj.Objs[18].Value[:3])
// 	// keyPoint = append(keyPoint, Average(obj.Objs[5].Value[:3], obj.Objs[6].Value[:3]))

// 	// for _, v := range obj.Objs {
// 	// 	keyPoint = append(keyPoint, v.Value[:3])
// 	// }

// }
