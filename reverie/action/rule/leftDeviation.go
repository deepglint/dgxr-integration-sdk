package rule

import (
	"fmt"
	"math"

	"meta/global"
	"meta/model/source"
)

// LeftDeviation
func ElbowBend(data *source.Source) bool {
	obj, err := data.LastData()
	if err != nil {
		return false
	}
	angleLeft := data.CalculateAngle(obj.Objs[source.LeftShoulder], obj.Objs[source.LeftElbow], obj.Objs[source.LeftWrist])
	angleRight := data.CalculateAngle(obj.Objs[source.RightShoulder], obj.Objs[source.RightElbow], obj.Objs[source.RightWrist])
	fmt.Println(angleLeft, angleRight, global.Config.Rules.ElbowBend.MinAngle, global.Config.Rules.ElbowBend.MaxAngle)
	if angleLeft > global.Config.Rules.ElbowBend.MinAngle && angleRight > global.Config.Rules.ElbowBend.MinAngle && angleLeft < global.Config.Rules.ElbowBend.MaxAngle && angleRight < global.Config.Rules.ElbowBend.MaxAngle && math.Abs(obj.Objs[5][1]-obj.Objs[22][1]) > 0.25 && math.Abs(obj.Objs[5][1]-obj.Objs[23][1]) > 0.25 {
		return true
	}
	return false
}

// 正常人默认左肩膀和右肩膀就是水平的
func LeftTilt(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		LeftShoulder := source.Point{X: float64(pose.Objs[source.LeftShoulder][0]), Y: 0, Z: float64(pose.Objs[source.LeftShoulder][2])}
		RightShoulder := source.Point{X: float64(pose.Objs[source.RightShoulder][0]), Y: 0, Z: float64(pose.Objs[source.RightShoulder][2])}
		// 计算线段 AB 和 CD 之间的夹角

		originalLeftShoulder := source.Point{X: -1, Y: 0, Z: 1.5}
		originalRightShoulder := source.Point{X: 1, Y: 0, Z: 1.5}

		vectorAB := source.Vector(originalLeftShoulder, originalRightShoulder)
		vectorCD := source.Vector(LeftShoulder, RightShoulder)
		angle := source.AngleBetweenVectors(vectorAB, vectorCD)

		// 将弧度转换为角度
		degree := angle * 180 / math.Pi

		if math.Abs(LeftShoulder.X-RightShoulder.X) > global.Config.Rules.LeftRightTilt.ShoulderWidth && degree > global.Config.Rules.LeftRightTilt.TiltAngle && LeftShoulder.Z < RightShoulder.Z {
			return true
		}

	}
	return false
}

// 正常人默认左肩膀和右肩膀就是水平的
func RightTilt(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		LeftShoulder := source.Point{X: float64(pose.Objs[source.LeftShoulder][0]), Y: 0, Z: float64(pose.Objs[source.LeftShoulder][2])}
		RightShoulder := source.Point{X: float64(pose.Objs[source.RightShoulder][0]), Y: 0, Z: float64(pose.Objs[source.RightShoulder][2])}
		// 计算线段 AB 和 CD 之间的夹角

		originalLeftShoulder := source.Point{X: -1, Y: 0, Z: 1.5}
		originalRightShoulder := source.Point{X: 1, Y: 0, Z: 1.5}

		vectorAB := source.Vector(originalLeftShoulder, originalRightShoulder)
		vectorCD := source.Vector(LeftShoulder, RightShoulder)
		angle := source.AngleBetweenVectors(vectorAB, vectorCD)

		// 将弧度转换为角度
		degree := angle * 180 / math.Pi

		if math.Abs(LeftShoulder.X-RightShoulder.X) > global.Config.Rules.LeftRightTilt.ShoulderWidth && degree > global.Config.Rules.LeftRightTilt.TiltAngle && LeftShoulder.Z > RightShoulder.Z {
			return true
		}

	}
	return false
}

// 正常人默认左肩膀和右肩膀就是水平的
func Squat(data *source.Source) bool {
	if pose, err := data.LastData(); err != nil {
		return false
	} else {
		LeftKneeAngle := data.CalculateAngle(pose.Objs[source.LeftHip], pose.Objs[source.LeftKnee], pose.Objs[source.LeftAnkle])
		RightKneeAngle := data.CalculateAngle(pose.Objs[source.RightHip], pose.Objs[source.RightKnee], pose.Objs[source.RightAnkle])
		if LeftKneeAngle > global.Config.Rules.Squat.KneeAngle && RightKneeAngle > global.Config.Rules.Squat.KneeAngle {
			return true
		}
	}
	return false
}
