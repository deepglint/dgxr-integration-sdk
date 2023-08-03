package conductor

// import (
// 	"math"

// 	"meta/global"

// 	"github.com/sirupsen/logrus"
// )

// func Conductor(personId string) {
// 	/*
// 		右手超过右肩膀的时候（绝对值相差5cm）且角度达到阈值判断为右手上
// 		右手x到达左肩膀的x相差5cm的且肘关节角度在一定范围内的时候判断为右手向左
// 	*/
// 	if personId != "" {
// 		if personSource, ok := global.Sources[personId]; ok {
// 			obj, err := personSource.LastData()
// 			if err != nil {
// 				logrus.Errorf("Skiing person source error: %v", err)
// 				return
// 			}

// 			// 值<0.2
// 			f := personSource.CalculateAngle(obj.Objs[6], obj.Objs[8], obj.Objs[10])
// 			if math.Abs((obj.Objs[23][2]+obj.Objs[23][1]+obj.Objs[23][0])-(obj.Objs[6][2]+obj.Objs[6][1]+obj.Objs[6][0])) < 0.2 && f > 120 {

// 				logrus.Info("右手向上挥了")
// 				global.VJoy.Button(1)
// 			}

// 			// 120 度以上
// 			distance := math.Abs((obj.Objs[23][0]) - (obj.Objs[5][0]))
// 			if distance > -0.3 && distance < 0.15 {
// 				global.VJoy.Button(2)
// 				logrus.Info("右手向上挥了")
// 			}
// 		}
// 	}
// }
