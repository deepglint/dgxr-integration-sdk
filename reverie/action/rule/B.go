package rule

import (
	"math"
	"meta/global"
	"meta/model/source"
)

func B(data *source.Source) bool {
	if pos, err := data.LastData(); err != nil {
		return false
	} else {
		// 右脚比左脚高20cm 以上，且 x 坐标 y 坐标相差一定的范围，且右手和头部x，y，z 在一定的距离范围内
		// 右脚比左脚高度差
		LeftAndRightHeight := math.Abs(pos.Objs[source.RightTiptoe][2] - pos.Objs[source.LeftTiptoe][2])

		// 左脚和右脚在地面二位坐标上的距离
		LeftAndRightXY := math.Abs(pos.Objs[source.RightTiptoe][0]-pos.Objs[source.LeftTiptoe][0]) + math.Abs(pos.Objs[source.RightTiptoe][1]-pos.Objs[source.LeftTiptoe][1])

		// 右手和头部x，y，z 在一定的距离范围内
		RightHandAndHead := math.Abs(pos.Objs[source.RightHand][0]-pos.Objs[source.HeadTop][0]) + math.Abs(pos.Objs[source.RightHand][1]-pos.Objs[source.HeadTop][1]) + math.Abs(pos.Objs[source.RightHand][2]-pos.Objs[source.HeadTop][2])
		if LeftAndRightHeight > global.Config.Rules.B.FeetHeight && LeftAndRightXY < global.Config.Rules.B.LeftAndRightFeetXY && RightHandAndHead < global.Config.Rules.B.RightHandThanHeadXYZ {
			return true
		}
		return false
	}
}
