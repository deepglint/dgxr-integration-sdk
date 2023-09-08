package rule

import (
	"math"
	"meta/global"
	"meta/model/source"
)

func Greet(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		// 	手腕一定的角度
		// - 当前帧x 在右肩膀里面，
		// - 高度和肩膀差不多
		// - 当前帧比上一帧x 方向的位移在一定的值
		calculate := data.CalculateAverageABC(int(source.RightWrist), int(source.RightElbow), int(source.RightShoulder))
		move := math.Abs(pose.Objs[source.RightHand][0] + pose.Objs[source.RightHand][2] - pose.Objs[source.RightShoulder][0] - pose.Objs[source.RightShoulder][2])
		if calculate > global.Config.Rules.Greet.ElbowAngle && move > global.Config.Rules.Greet.HandMoveDistanceXZ {
			return true
		}
	}
	return false
}
