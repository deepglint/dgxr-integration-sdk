package rule

import (
	"math"
	"reverie/global"
	"reverie/model/source"
)

func SlideLeft(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		// leftHip

		// - 1s 相比上面 20 帧的动作，手部的位移
		// - 手高度在左肩膀位置相差一定的大小
		angle := CalculateRotateAngle(pose.Objs[source.LeftHip], pose.Objs[source.RightHip])
		if angle > 0 && angle < 45 {
			averageHandX := data.AverageAX(int(source.RightHand))
			moveDistance := math.Abs(averageHandX - pose.Objs[source.RightHand][0])
			HandToShoulderX := math.Abs(pose.Objs[source.RightHand][0] - pose.Objs[source.LeftShoulder][0])
			HandToShoulderZ := math.Abs(pose.Objs[source.RightHand][2] - pose.Objs[source.LeftShoulder][2])
			if moveDistance > global.Config.Rules.LeftRightSlide.HandMoveDistanceX && HandToShoulderX < global.Config.Rules.LeftRightSlide.HandToShoulderXZ && HandToShoulderZ < global.Config.Rules.LeftRightSlide.HandToShoulderXZ {
				return true
			}
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
		angle := CalculateRotateAngle(pose.Objs[source.LeftHip], pose.Objs[source.RightHip])
		if angle > 0 && angle < 45 {
			averageHandX := data.AverageAX(int(source.LeftHand))
			moveDistance := math.Abs(averageHandX - pose.Objs[source.LeftHand][0])
			HandToShoulderX := math.Abs(pose.Objs[source.LeftHand][0] - pose.Objs[source.RightShoulder][0])
			HandToShoulderZ := math.Abs(pose.Objs[source.LeftHand][2] - pose.Objs[source.RightShoulder][2])
			if moveDistance > global.Config.Rules.LeftRightSlide.HandMoveDistanceX && HandToShoulderX < global.Config.Rules.LeftRightSlide.HandToShoulderXZ && HandToShoulderZ < global.Config.Rules.LeftRightSlide.HandToShoulderXZ {
				return true
			}
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
		angle := CalculateRotateAngle(pose.Objs[source.LeftHip], pose.Objs[source.RightHip])
		if angle > 0 && angle < 45 {
			averageHandY := data.AverageAY(int(source.LeftHand))
			averageHandZ := data.AverageAZ(int(source.LeftHand))
			moveHandY := math.Abs(averageHandY - pose.Objs[source.LeftHand][1])
			moveHandZ := math.Abs(averageHandZ - pose.Objs[source.LeftHand][2])
			elbowCalculate := data.CalculateAngle(pose.Objs[source.LeftShoulder], pose.Objs[source.LeftElbow], pose.Objs[source.LeftWrist])
			HandShoulderHight := pose.Objs[source.LeftHand][2] - pose.Objs[source.LeftShoulder][2]
			if moveHandY > global.Config.Rules.UpSlide.HandMoveDistanceY && moveHandZ > global.Config.Rules.UpSlide.HandMoveDistanceZ && elbowCalculate > global.Config.Rules.UpSlide.ElbowAngle && HandShoulderHight > global.Config.Rules.UpSlide.HandToShoulderZ {
				return true
			}
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
		averageHandX := data.AverageAX(int(source.RightHand))
		averageHandY := data.AverageAY(int(source.RightHand))
		averageHandZ := data.AverageAZ(int(source.RightHand))
		moveHandX := math.Abs(averageHandX - pose.Objs[source.RightHand][0])

		moveHandY := math.Abs(averageHandY - pose.Objs[source.RightHand][1])
		moveHandZ := math.Abs(averageHandZ - pose.Objs[source.RightHand][2])
		elbowCalculate := data.CalculateAngle(pose.Objs[source.RightShoulder], pose.Objs[source.RightElbow], pose.Objs[source.RightWrist])
		HandHipHight := pose.Objs[source.RightHand][2] - pose.Objs[source.RightHip][2]
		if moveHandX < global.Config.Rules.DownSlide.HandMoveDistanceX && moveHandY > global.Config.Rules.DownSlide.HandMoveDistanceY && moveHandZ > global.Config.Rules.DownSlide.HandMoveDistanceZ && elbowCalculate < global.Config.Rules.DownSlide.ElbowAngle && HandHipHight < global.Config.Rules.DownSlide.HandToHipZ {
			return true
		}
	}
	return false
}

func CalculateRotateAngle(a, b []float64) float64 {
	AB_x := b[0] - a[0]
	AB_y := b[1] - a[1]

	i_x, i_y := 1.0, 0.0

	dotProduct := AB_x*i_x + AB_y*i_y

	modAB := math.Sqrt(AB_x*AB_x + AB_y*AB_y)

	modI := math.Sqrt(i_x*i_x + i_y*i_y)

	cosTheta := dotProduct / (modAB * modI)

	theta := math.Acos(cosTheta)

	angleDegrees := theta * 180 / math.Pi
	return angleDegrees
}
