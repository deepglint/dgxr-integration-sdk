package model

import (
	"fmt"
	"log"
	"math"
	"meta/global"
	"meta/servers/grpc/proto"
	"time"
)

type JointType int

const (
	Nose JointType = iota
	LeftEye
	RightEye
	LeftEar
	RightEar
	LeftShoulder
	RightShoulder
	LeftElbow
	RightElbow
	LeftWrist
	RightWrist
	LeftHip
	RightHip
	LeftKnee
	RightKnee
	LeftAnkle
	RightAnkle
	LeftTiptoe
	RightTiptoe
	LeftHeel
	RightHeel
	HeadTop
	LeftHand
	RightHand
	Chain
)

var FrameQ *Queue

// TODO 待封装更改根据uid计算每个人的动作，暂时不考虑锁的问题
type Queue struct {
	items []proto.Data
	cap   int
}

func InitQueue(cap int) *Queue {
	return &Queue{
		items: []proto.Data{},
		cap:   cap,
	}
}

func (q *Queue) Enqueue(item proto.Data) {
	if len(q.items) >= q.cap {
		q.items = q.items[1:]
	}
	q.items = append(q.items, item)
}

func (q *Queue) Dequeue() (proto.Data, error) {
	if len(q.items) == 0 {
		return proto.Data{}, fmt.Errorf("Queue is empty")
	}
	// 取出队列的第一个数据
	item := q.items[0]
	// 删除队列的第一个数据
	q.items = q.items[1:]
	return item, nil
}

func (q *Queue) Size() int {
	return len(q.items)
}

func (q *Queue) CalculateAverageABC(a, b, c int) float64 {
	var sum float64
	for _, value := range q.items {
		sum += CalculateAngle(value.Objs[a].Value, value.Objs[b].Value, value.Objs[c].Value)
	}
	return sum / float64(len(q.items))
}

func (q *Queue) AverageAX(a int) float64 {
	var sum float64
	for _, value := range q.items {
		sum += float64(value.Objs[a].Value[0])
	}
	return sum / float64(len(q.items))
}

// 返回任意两个点旋转角度
func (q *Queue) CalculateAverageAB(a, b int) float64 {
	var sum float64
	for _, value := range q.items {
		sum += CalculateRotateAngle(value.Objs[a].Value, value.Objs[b].Value)
	}
	return sum / float64(len(q.items))
}

// 判断是否举左手
func (q *Queue) IsLeftHandUp() bool {
	return AThanB(q.items[0].Objs[int(LeftHand)].Value[2], q.items[0].Objs[int(HeadTop)].Value[2])
}

// 计算两个点的中心点的平均值
func (q *Queue) CalculateAverageA(a, b int) float64 {
	var sum float64
	for _, value := range q.items {
		center := CalculateCenterPoint(value.Objs[a].Value, value.Objs[b].Value)
		sum += center[0] + center[1] + center[2]
	}
	return sum / float64(len(q.items))
}

// 计算两个点的x，y中心点的平均值
func (q *Queue) CalculateAverageAXY(a, b int) float64 {
	var sum float64
	for _, value := range q.items {
		center := CalculateCenterPoint(value.Objs[a].Value, value.Objs[b].Value)
		sum += center[0] + center[1]
	}
	return sum / float64(len(q.items))
}

// TODO，放到队列中的一个变量，队列中放每个人的action，每个人的id，每个人的分组，是否监控等
var action = false

// TODO 待增加时间维度的判断
func (q *Queue) Jump(obj *proto.Data, frameId, PersonId string, ts int64) {
	event := "idle"
	if q.Size() < 10 {

	} else {
		// _, err := q.Dequeue()
		// if err != nil {
		// 	// 没有检测到数据
		// }

		// 判断是否在移动
		move := q.CalculateAverageAXY(11, 12)
		center := CalculateCenterPoint(obj.Objs[11].Value, obj.Objs[12].Value)

		l := q.CalculateAverageABC(11, 13, 15)
		r := q.CalculateAverageABC(12, 14, 16)
		m := (l + r) / 2

		cl := CalculateAngle(obj.Objs[11].Value, obj.Objs[13].Value, obj.Objs[15].Value)
		cr := CalculateAngle(obj.Objs[12].Value, obj.Objs[14].Value, obj.Objs[16].Value)
		cm := (cl + cr) / 2

		// TODO kinect 单位是毫米，因此是否移动改为80，灵境值为m，因此改为0.08m
		if math.Abs(cr-cl) > 40 || math.Abs((center[0]+center[1])-move) > 0.08 {
			return
		}

		cx := CalculateRotateAngle(obj.Objs[11].Value, obj.Objs[12].Value)
		if (cm - m) > 20 {
			event = "squat"
			action = true

		} else if action == true && cm < 36 { // TODO更改26的值，可以更改延迟时间，26，
			event = "squat"
			action = false
		}

		keyPoint := [][]float32{}
		for _, v := range obj.Objs {
			keyPoint = append(keyPoint, v.Value[:3])
		}

		if len(global.ActionChan) < 8 {
			global.ActionChan <- global.Action{
				FrameId:  frameId,
				PersonId: PersonId,
				TsEngine: ts,
				TsWS:     time.Now().UnixMilli(),
				Pose: global.Pose{
					KeyPoints: keyPoint,
					Direction: int(cx), //TODO 待计算
					Action:    event,
				},
			}
		}
	}
}

// TODO 待增加时间维度的判断
func (q *Queue) Conductor(obj *proto.Data) {

	if q.Size() < 10 {

	} else {
		cl := CalculateAngle(obj.Objs[6].Value, obj.Objs[8].Value, obj.Objs[10].Value)
		if float64(obj.Objs[23].Value[2]-obj.Objs[6].Value[2]) > -20 && cl > 130 {
			log.Println("右手向上挥了", cl)
		}
		// 左手伸直
		left := CalculateAngle(obj.Objs[5].Value, obj.Objs[7].Value, obj.Objs[9].Value)
		// 左手位置
		/*
			1.速度太快无法识别
			2.向左和向前无法识别（增加手部
			3.增加左大臂和身体夹角在某个范围内
		*/
		leftShoulder := CalculateAngle(obj.Objs[7].Value, obj.Objs[5].Value, obj.Objs[6].Value)
		moveLeftHand := float64(obj.Objs[22].Value[0]) - q.AverageAX(22)
		// ((left < 80) || left > 160)
		if moveLeftHand > 30 && leftShoulder > 50 && leftShoulder < 75 {
			log.Println("左手向前指了", left, moveLeftHand, leftShoulder)
		}
	}
}
