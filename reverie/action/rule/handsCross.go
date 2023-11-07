package rule

import (
	"math"
	"reverie/global"
	"reverie/model/source"
)

func HandsCross(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		// 双手的肘的角度
		// 双手高度高于胯部
		// 双手低于肩膀
		// 双手的之间的XY距离的绝对值小于双肩膀的距离绝对值
		leftCalculate := data.CalculateAngle(pose.Objs[source.LeftShoulder], pose.Objs[source.LeftElbow], pose.Objs[source.LeftWrist])
		rightCalculate := data.CalculateAngle(pose.Objs[source.RightShoulder], pose.Objs[source.RightElbow], pose.Objs[source.RightWrist])
		HandHight := (pose.Objs[source.LeftHand][2] + pose.Objs[source.RightHand][2]) / 2
		HandShoulderDistance := math.Abs(pose.Objs[source.LeftHand][0]+pose.Objs[source.LeftHand][1]-pose.Objs[source.RightHand][0]-pose.Objs[source.RightHand][1]) - math.Abs((pose.Objs[source.LeftShoulder][0]+pose.Objs[source.LeftShoulder][1])-pose.Objs[source.RightShoulder][0]-pose.Objs[source.RightShoulder][1])
		if leftCalculate > global.Config.Rules.HandsCross.LeftMinCalculate && leftCalculate < global.Config.Rules.HandsCross.LeftMaxCalculate && rightCalculate < global.Config.Rules.HandsCross.RightMaxCalculate && rightCalculate > global.Config.Rules.HandsCross.RightMaxCalculate && HandShoulderDistance < 0 && HandHight > pose.Objs[source.LeftHip][2] && HandHight < pose.Objs[source.LeftShoulder][2] {
			return true
		}
	}
	return false
}
