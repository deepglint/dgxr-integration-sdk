package rule

import (
	"math"
	"meta/global"
	"meta/model/source"
)

func LeftSlide(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		// - 1s 相比上面 20 帧的动作，手部的位移
		// - 手高度在左肩膀位置相差一定的大小
		averageHandX := data.AverageAX(int(source.RightHand))
		moveDistance := averageHandX - pose.Objs[source.RightHand][0]
		HandToShoulderX := math.Abs(pose.Objs[source.RightHand][0] - pose.Objs[source.LeftShoulder][0])
		HandToShoulderZ := math.Abs(pose.Objs[source.RightHand][2] - pose.Objs[source.LeftShoulder][2])
		if moveDistance > global.Config.Rules.LeftSlide.HandMoveDistanceX && HandToShoulderX < global.Config.Rules.LeftSlide.HandToShoulderXZ && HandToShoulderZ < global.Config.Rules.LeftSlide.HandToShoulderXZ {
			return true
		}
	}
	return false
}
