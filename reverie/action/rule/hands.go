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
		if leftCalculate > global.Config.Rules.HandsCross.LeftMinCalculate && leftCalculate < global.Config.Rules.HandsCross.LeftMaxCalculate && rightCalculate < global.Config.Rules.HandsCross.RightMaxCalculate && rightCalculate > global.Config.Rules.HandsCross.RightMinCalculate && HandShoulderDistance < 0 && HandHight > pose.Objs[source.LeftHip][2] && HandHight < pose.Objs[source.LeftShoulder][2] {
			return true
		}
	}
	return false
}

func HandsAway(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		/*
			- 左手x 超过左肩膀0.2
			- 右手 x 超过右肩膀0.2
			- 左手1s x 的移动距离超过 0.3
			- 右手1s 移动的距离也超过 0.3
		*/
		shoulderLeftHandDistance := pose.Objs[source.LeftShoulder][0] - pose.Objs[source.LeftHand][0]
		shoulderRightHandDistance := pose.Objs[source.RightHand][0] - pose.Objs[source.RightShoulder][0]
		leftMoveDistance := data.AverageAX(int(source.LeftHand)) - pose.Objs[source.LeftHand][0]
		rightMoveDistance := pose.Objs[source.RightHand][0] - data.AverageAX(int(source.RightHand))
		if shoulderLeftHandDistance > 0.3 && shoulderRightHandDistance > 0.3 && leftMoveDistance > 0.15 && rightMoveDistance > 0.15 {
			return true
		}

	}
	return false
}

func HandsClose(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		/*
			- 双手 xyz abs 距离不超过 0.05
			- 双手的 x在左边肩膀和右边肩膀 x 之间
			- 双手 z 大于胯部，小于肩部
			- 左手x 1s 内移动（end-ave>0.1)
			- 右手x 1s内移动（end-ave<0.1）
		*/
		handDistance := math.Abs(pose.Objs[source.LeftHand][0]-pose.Objs[source.RightHand][0]) + math.Abs(pose.Objs[source.LeftHand][1]-pose.Objs[source.RightHand][1]) + math.Abs(pose.Objs[source.LeftHand][2]-pose.Objs[source.RightHand][2])
		leftMoveDistance := pose.Objs[source.LeftHand][0] - data.AverageAX(int(source.LeftHand))
		rightMoveDistance := data.AverageAX(int(source.RightHand)) - pose.Objs[source.RightHand][0]
		if handDistance < 0.10 && leftMoveDistance > 0.10 && rightMoveDistance > 0.10 && pose.Objs[source.LeftHand][0] > pose.Objs[source.LeftShoulder][0] && pose.Objs[source.RightHand][0] < pose.Objs[source.RightShoulder][0] && pose.Objs[source.LeftHand][2] > pose.Objs[source.LeftHip][2] && pose.Objs[source.LeftHand][2] < pose.Objs[source.LeftShoulder][2] && pose.Objs[source.RightHand][2] > pose.Objs[source.RightHip][2] && pose.Objs[source.RightHand][2] < pose.Objs[source.RightShoulder][2] {
			return true
		}
	}
	return false
}
