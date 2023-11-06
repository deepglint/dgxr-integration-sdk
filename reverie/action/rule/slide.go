package rule

import (
	"fmt"
	"math"
	"reverie/global"
	"reverie/model/source"
)

func SlideLeft(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		// - 1s 相比上面 20 帧的动作，手部的位移
		// - 手高度在左肩膀位置相差一定的大小
		averageHandX := data.AverageAX(int(source.RightHand))
		moveDistance := averageHandX - pose.Objs[source.RightHand][0]
		HandToShoulderX := math.Abs(pose.Objs[source.RightHand][0] - pose.Objs[source.LeftShoulder][0])
		HandToShoulderZ := math.Abs(pose.Objs[source.RightHand][2] - pose.Objs[source.LeftShoulder][2])
		fmt.Println("右手移动", moveDistance, "右手到左肩膀X", HandToShoulderX, "右手到左肩膀Z", HandToShoulderZ)
		if moveDistance > global.Config.Rules.LeftRightSlide.HandMoveDistanceX && HandToShoulderX < global.Config.Rules.LeftRightSlide.HandToShoulderXZ && HandToShoulderZ < global.Config.Rules.LeftRightSlide.HandToShoulderXZ {
			return true
		}
	}
	return false
}

func SlideRight(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		// - 1s 相比上面 20 帧的动作，手部的位移
		// - 手高度在左肩膀位置相差一定的大小
		averageHandX := data.AverageAX(int(source.LeftHand))
		moveDistance := averageHandX - pose.Objs[source.LeftHand][0]
		HandToShoulderX := math.Abs(pose.Objs[source.LeftHand][0] - pose.Objs[source.RightShoulder][0])
		HandToShoulderZ := math.Abs(pose.Objs[source.LeftHand][2] - pose.Objs[source.RightShoulder][2])
		fmt.Println("左手移动", moveDistance, "左手到右肩膀X", HandToShoulderX, "左手到右肩膀Z", HandToShoulderZ)
		if moveDistance > global.Config.Rules.LeftRightSlide.HandMoveDistanceX && HandToShoulderX < global.Config.Rules.LeftRightSlide.HandToShoulderXZ && HandToShoulderZ < global.Config.Rules.LeftRightSlide.HandToShoulderXZ {
			return true
		}
	}
	return false
}

func SlideUp(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		// - 1s 相比上面 20 帧的动作，Y 轴的手部的位移
		// - 1s 相比上面 20 帧的动作，Z 轴的手部的位移
		// 左手肘部的角度在一定的范围内
		// 左手X高于左肩膀位置一定的大小
		averageHandY := data.AverageAY(int(source.LeftHand))
		averageHandZ := data.AverageAZ(int(source.LeftHand))
		moveHandY := averageHandY - pose.Objs[source.LeftHand][1]
		moveHandZ := averageHandZ - pose.Objs[source.LeftHand][2]
		elbowCalculate := data.CalculateAngle(pose.Objs[source.LeftShoulder], pose.Objs[source.LeftElbow], pose.Objs[source.LeftWrist])
		HandShoulderHight := pose.Objs[source.LeftHand][2] - pose.Objs[source.LeftShoulder][2]
		fmt.Println("Y 手移动", moveHandY, "Z 手移动", moveHandZ, "肘弯曲", elbowCalculate, "手高于肩膀", HandShoulderHight)
		if moveHandY > global.Config.Rules.UpSlide.HandMoveDistanceY && moveHandZ > global.Config.Rules.UpSlide.HandMoveDistanceZ && elbowCalculate > global.Config.Rules.UpSlide.ElbowAngle && HandShoulderHight > global.Config.Rules.UpSlide.HandToShoulderZ {
			return true
		}
	}
	return false
}

func SlideDown(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		// 1s 相比上面 20 帧的动作，Y 轴的手部的位移
		// 1s 相比上面 20 帧的动作，Z 轴的手部的位移
		// 右手肘部的角度在一定的范围内
		// 右手X低于胯部一定的大小
		averageHandY := data.AverageAY(int(source.RightHand))
		averageHandZ := data.AverageAZ(int(source.RightHand))
		moveHandY := averageHandY - pose.Objs[source.RightHand][1]
		moveHandZ := averageHandZ - pose.Objs[source.RightHand][2]
		elbowCalculate := data.CalculateAngle(pose.Objs[source.RightShoulder], pose.Objs[source.RightElbow], pose.Objs[source.RightWrist])
		HandHipHight := pose.Objs[source.RightHand][2] - pose.Objs[source.RightHip][2]

		fmt.Println("Y 手移动", moveHandY, "Z 手移动", moveHandZ, "肘弯曲", elbowCalculate, "手低于胯部", HandHipHight)
		if moveHandY > global.Config.Rules.DownSlide.HandMoveDistanceY && moveHandZ < global.Config.Rules.DownSlide.HandMoveDistanceZ && elbowCalculate < global.Config.Rules.DownSlide.ElbowAngle && HandHipHight < global.Config.Rules.DownSlide.HandToHipZ {
			return true
		}
	}
	return false
}
