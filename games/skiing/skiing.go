package skiing

import (
	"math"

	"meta/global"
	games "meta/model/game"
	sources "meta/model/source"

	"github.com/sirupsen/logrus"
)

func StartGame(source *sources.Source) bool {
	obj, err := source.LastData()
	if err != nil {
		logrus.Errorf("startGame error: %v", err)
		return false
	}
	angleLeft := source.CalculateAngle(obj.Objs[sources.LeftShoulder], obj.Objs[sources.LeftElbow], obj.Objs[sources.LeftWrist])
	angleRight := source.CalculateAngle(obj.Objs[sources.RightShoulder], obj.Objs[sources.RightElbow], obj.Objs[sources.RightWrist])
	if angleLeft > global.Config.Games.Skiing.MinElbowAngle && angleRight > global.Config.Games.Skiing.MinElbowAngle && angleLeft < global.Config.Games.Skiing.MaxElbowAngle && angleRight < global.Config.Games.Skiing.MaxElbowAngle && obj.Objs[5][1]-obj.Objs[22][1] > 0.25 && obj.Objs[5][1]-obj.Objs[23][1] > 0.25 {
		global.Games.SetGamesSource(games.Skiing, obj)
		// TODO vjoy
		// global.Button(1)
		return true
	}
	return false
}

func Skiing(personId string) {
	if personId != "" {
		if personSource, ok := global.Sources[personId]; ok {
			obj, err := personSource.LastData()
			if err != nil {
				logrus.Errorf("Skiing person source error: %v", err)
				return
			}

			LeftShoulder := sources.Point{X: float64(obj.Objs[sources.LeftShoulder][0]), Y: 0, Z: float64(obj.Objs[sources.LeftShoulder][2])}
			RightShoulder := sources.Point{X: float64(obj.Objs[sources.RightShoulder][0]), Y: 0, Z: float64(obj.Objs[sources.RightShoulder][2])}
			source := global.Games.GetGames(games.Skiing)
			// 水平越水平，切肩膀宽度宽设置为起始位置
			if math.Abs(LeftShoulder.Z-RightShoulder.Z) < math.Abs(source.OriginalSource.Objs[sources.LeftShoulder][2]-source.OriginalSource.Objs[sources.RightShoulder][2]) {
				// global.Games[games.Skiing].OriginalSource = obj
				global.Games.SetGamesSource(games.Skiing, obj)

			}

			originalLeftShoulder := sources.Point{X: source.OriginalSource.Objs[sources.LeftShoulder][0], Y: 0, Z: source.OriginalSource.Objs[sources.LeftShoulder][2]}
			originalRightShoulder := sources.Point{X: source.OriginalSource.Objs[sources.RightShoulder][0], Y: 0, Z: source.OriginalSource.Objs[sources.RightShoulder][2]}

			// 计算线段 AB 和 CD 之间的夹角
			vectorAB := sources.Vector(originalLeftShoulder, originalRightShoulder)
			vectorCD := sources.Vector(LeftShoulder, RightShoulder)
			angle := sources.AngleBetweenVectors(vectorAB, vectorCD)

			// 将弧度转换为角度
			degree := angle * 180 / math.Pi

			if LeftShoulder.X-RightShoulder.X > global.Config.Games.Skiing.ShoulderWidth && degree > global.Config.Games.Skiing.TiltAngle {
				if LeftShoulder.Z < RightShoulder.Z {
					// TODO vjoy
					// if global.VJoy != nil {
					// 	global.VJoy.Axis(joystick.AxisX).Setf(-1)
					// 	global.VJoy.Update()
					// }
					logrus.Info("========left===========")
				} else {
					// TODO vjoy
					// if global.VJoy != nil {
					// 	global.VJoy.Axis(joystick.AxisX).Setf(1)
					// 	global.VJoy.Update()
					// }
					logrus.Info("========right===========")
				}
			} else {
				// TODO vjoy
				// if global.VJoy != nil {
				// 	global.VJoy.Axis(joystick.AxisX).Setf(0)
				// 	global.VJoy.Update()
				// }
				LeftKneeAngle := personSource.CalculateAngle(obj.Objs[sources.LeftHip], obj.Objs[sources.LeftKnee], obj.Objs[sources.LeftAnkle])
				RightKneeAngle := personSource.CalculateAngle(obj.Objs[sources.RightHip], obj.Objs[sources.RightKnee], obj.Objs[sources.RightAnkle])
				if LeftKneeAngle > global.Config.Games.Skiing.JumpKneeAngle && RightKneeAngle > global.Config.Games.Skiing.JumpKneeAngle {
					global.Games.SetGamesJump(games.Skiing, true)
				} else {
					if source.Jump {
						// TODO vjoy
						// global.Button(2)
						logrus.Info("========jump===========")
						global.Games.SetGamesJump(games.Skiing, false)
					}
				}
			}
		}
	}
}

// 向量点积
func dot(v1, v2 sources.Point) float64 {
	return v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z
}

// 向量长度
func length(v sources.Point) float64 {
	return math.Sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z)
}
