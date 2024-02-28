package action

import (
	"reverie/action/rule"
	"reverie/global"
	"reverie/model/config"
	"reverie/model/source"

	"github.com/sirupsen/logrus"
)

type Action int
type ActionFunc func(*source.Source) bool

const (
	RightHandDrawCircle  Action = 1     //左手画圈
	LeftHandDrawCircle   Action = 2     //右手画圈
	Kick                 Action = 10    //踢腿
	CombineHandsStraight Action = 17    //双手伸直合并
	ThrowBoulder         Action = 18    //举手投掷巨物
	SlowRun              Action = 19    //慢跑
	FastRun              Action = 20    //快跑
	Butterfly            Action = 21    //蝶泳
	Freestyle            Action = 22    //自由泳
	KeepRaisingHand      Action = 23    //持续举手
	Applaud              Action = 24    //拍掌
	Jump                 Action = 25    //起跳
	DeepSquat            Action = 26    //下蹲
	RaiseOnHand          Action = 10000 //举单手
	RaiseBothHand        Action = 10001 //举双手
	ArmFlat              Action = 10002 //手臂平展
	ArmFlatIsL           Action = 10003 //手臂平展为 L
	ArmVerticalIsL       Action = 10004 //手臂垂直为 L

	SlideLeft      Action = 2001 // 左滑
	SlideRight     Action = 2002 // 右滑
	SlideUp        Action = 2003 // 上滑
	SlideDown      Action = 2004 // 下滑
	HandsAway      Action = 2005 // 双手远离
	HandsClose     Action = 2006 // 双手靠近
	Waving         Action = 2007 // 挥手
	ArmToForward   Action = 2008 // 手臂向前
	ArmToBack      Action = 2009 // 手臂向后
	ArmToLeft      Action = 2010 // 手臂向左
	ArmToRight     Action = 2011 // 手臂向右
	BendBothElbows Action = 2012 // 弯双肘
	HandsCross     Action = 2013 // 双手交叉

	PoseA       Action = 3001 // 姿势 A
	PoseB       Action = 3002 // 姿势 B
	PoseC       Action = 3003 // 姿势 C
	PoseD       Action = 3004 // 姿势 D
	LeanToLeft  Action = 3005 // 左倾斜
	LeanToRight Action = 3006 // 右倾斜
	Stand       Action = 3007 // 站立

	SmallSquat Action = 4001 // 浅蹲
)

func (a Action) String() string {
	switch a {
	case RightHandDrawCircle:
		return "RightHandDrawCircle"
	case LeftHandDrawCircle:
		return "LeftHandDrawCircle"
	case Kick:
		return "Kick"
	case CombineHandsStraight:
		return "CombineHandsStraight"
	case ThrowBoulder:
		return "ThrowBoulder"
	case SlowRun:
		return "SlowRun"
	case FastRun:
		return "FastRun"
	case Butterfly:
		return "Butterfly"
	case Freestyle:
		return "Freestyle"
	case KeepRaisingHand:
		return "KeepRaisingHand"
	case Applaud:
		return "Applaud"
	case Jump:
		return "Jump"
	case DeepSquat:
		return "DeepSquat"
	case RaiseOnHand:
		return "RaiseOnHand"
	case RaiseBothHand:
		return "RaiseBothHand"
	case ArmFlat:
		return "ArmFlat"
	case ArmFlatIsL:
		return "ArmFlatIsL"
	case ArmVerticalIsL:
		return "ArmVerticalIsL"
	case SlideLeft:
		return "SlideLeft"
	case SlideRight:
		return "SlideRight"
	case SlideUp:
		return "SlideUp"
	case SlideDown:
		return "SlideDown"
	case HandsAway:
		return "HandsAway"
	case HandsClose:
		return "HandsClose"
	case Waving:
		return "Waving"
	case ArmToForward:
		return "ArmToForward"
	case ArmToBack:
		return "ArmToBack"
	case ArmToLeft:
		return "ArmToLeft"
	case ArmToRight:
		return "ArmToRight"
	case BendBothElbows:
		return "BendBothElbows"
	case HandsCross:
		return "HandsCross"
	case PoseA:
		return "PoseA"
	case PoseB:
		return "PoseB"
	case PoseC:
		return "PoseC"
	case PoseD:
		return "PoseD"
	case LeanToLeft:
		return "LeanToLeft"
	case LeanToRight:
		return "LeanToRight"
	case Stand:
		return "Stand"
	case SmallSquat:
		return "SmallSquat"
	default:
		return "Unknown"
	}
}

type ActionRegistry struct {
	registry map[Action]ActionFunc
}

func (ar *ActionRegistry) Register(name Action, an ActionFunc) {
	ar.registry[name] = an
}

func (fr *ActionRegistry) Run(name Action, pos *source.Source) bool {
	if fn, exists := fr.registry[name]; exists {
		return fn(pos)
	}
	return false
}

var (
	Registry *ActionRegistry
)

func init() {
	Registry = &ActionRegistry{
		registry: make(map[Action]ActionFunc),
	}

	Registry.Register(PoseA, rule.A)
	Registry.Register(PoseB, rule.B)
	Registry.Register(PoseC, rule.C)
	Registry.Register(PoseD, rule.D)
	// Registry.Register(, rule.RaiseHandRight)
	Registry.Register(Waving, rule.Greet)
	Registry.Register(SlideLeft, rule.SlideLeft)
	Registry.Register(SlideRight, rule.SlideRight)
	// Registry.Register(SlideUp, rule.SlideUp)
	// Registry.Register(SlideDown, rule.SlideDown)
	// Registry.Register(SmallSquat, rule.Squat)
	Registry.Register(SmallSquat, rule.DafuWongSquat)
	Registry.Register(LeanToRight, rule.RightTilt)
	Registry.Register(LeanToLeft, rule.LeftTilt)
	Registry.Register(BendBothElbows, rule.ElbowBend)
	Registry.Register(Stand, rule.Stand)
	// Registry.Register(HandsCross, rule.HandsCross)
	// Registry.Register(HandsAway, rule.HandsAway)
	// Registry.Register(HandsClose, rule.HandsClose)
}

func ActionToXbox() {
	global.Sources.Range(func(key, value interface{}) bool {
		poser := value.(*source.Source)
		go func(pos *source.Source) {
			for key, actionData := range global.Config.Action {
				// 遍历每一个注册进来的动作
				if _, ok := Registry.registry[Action(key)]; ok {
					if Registry.Run(Action(key), pos) {
						logrus.Infof("rule action: %s", Action(key).String())
						go pos.Xbox.SetXbox(actionData)
					}
				} else {
					go TemplateMatch(pos, key, actionData)
				}
			}
		}(poser)
		return true
	})
}

func RuleToXbox(pos *source.Source) {
	for k, v := range global.Config.Action {
		// 收到数据入库
		if Registry.Run(Action(k), pos) {
			logrus.Infof("rule action: %s", Action(k).String())
			go pos.Xbox.SetXbox(v)
		}
	}
}

func ModelToXbox(pos *source.Source, action int32) {
	if v, ok := global.Config.Action[int(action)]; ok {

		if body, err := pos.LastData(); err == nil {
			switch action {
			case 21:
				// 状态定义 false
				// 当手的关节点低于胯部的时候设置为false，高于左肩位置设置为true，当为false且高于左边肩膀的时候计数一次
				if body.Objs[source.LeftHand][2]-0.2-body.Objs[source.LeftHip][2] < 0 {
					pos.Butterfly = false
				}
				leftStatue := body.Objs[source.LeftHand][2] - body.Objs[source.LeftShoulder][2]
				if leftStatue > 0 && !pos.Butterfly {
					// 按下按键并且计数
					logrus.Infof("model action: %s", Action(action).String())
					go pos.Xbox.SetXbox(v)
				}
				if leftStatue > 0 {
					pos.Butterfly = true
				}
				return
			case 22:
				// if body.Objs[source.LeftHand][2]-body.Objs[source.LeftHip][2] < 0 {
				// 	pos.FreeStyle = false
				// }
				leftStatue := body.Objs[source.LeftHand][2] - body.Objs[source.LeftShoulder][2]
				if leftStatue < 0 {
					pos.FreeStyle = false
				}
				if leftStatue > 0 && !pos.FreeStyle {
					// 按下按键并且计数
					logrus.Infof("sum model action: %s", Action(action).String())
					go pos.Xbox.SetXbox(v)
					pos.FreeStyle = true
				}
				if leftStatue > 0 {
					pos.FreeStyle = true
				}
				return
			case 20:
				// 低于跨的的1/3的时候，设置为false,左边腿高于右边腿和跨的的1/3的时候设置为true，且当为false的时候，计数+1
				// 快跑
				hight := (body.Objs[source.RightHip][2] - body.Objs[source.RightKnee][2]) / 3
				if body.Objs[source.LeftKnee][2]-hight-body.Objs[source.RightKnee][2] < 0 {
					pos.FastRun = false
				}
				if body.Objs[source.LeftKnee][2]-hight-body.Objs[source.RightKnee][2] > 0 && !pos.FastRun {
					logrus.Infof("model action: %s", Action(action).String())
					go pos.Xbox.SetXbox(v)
				}
				if body.Objs[source.LeftKnee][2]-hight-body.Objs[source.RightKnee][2] > 0 {
					pos.FastRun = true
				}
				return
				// default:
				// 	pos.FastRun = false
				// 	pos.Butterfly = false
				// 	pos.FreeStyle = false
			}
			logrus.Infof("model action: %s", Action(action).String())
			go pos.Xbox.SetXbox(v)
		}
	}
}

func TemplateMatch(pos *source.Source, k int, action config.ActionData) {
	if val, ok := global.GetTemplatesByID(k); ok {
		var disScore float64
		pathLen := 100
		temNormP3d := KeyNodePos(val.Tem.NormP3d, val.KeyNode)
		// 增加单帧字段，根据数据库字段判断是否是单帧，单帧就匹配 for 遍历每一帧进行计算匹配
		if val.SingleFrame == 1 {
			// 单帧
			if pos, err := pos.LastData(); err != nil {
				logrus.Error(err)
			} else {
				input := [][][]float64{}
				input = append(input, pos.Objs)
				normP3d := global.NormalizeSeqPose3D(input)
				keyNormP3d := KeyNodePos(normP3d, val.KeyNode)

				for _, tem := range temNormP3d {
					distMat := global.ComputeDistMatrix([][][]float64{tem}, keyNormP3d)
					disScore = distMat.At(0, 0)
					if disScore < float64(val.Score) {
						break
					}
				}
			}
		} else {
			// 多帧
			pos := pos.All()
			input := [][][]float64{}
			is := len(pos) - len(val.Tem.NormP3d)
			if is < 0 {
				return
			}
			for i := is; i < len(pos); i++ {
				input = append(input, pos[i].Objs)
			}
			normP3d := global.NormalizeSeqPose3D(input)
			keyNormP3d := KeyNodePos(normP3d, val.KeyNode)
			distMat := global.ComputeDistMatrix(temNormP3d, keyNormP3d)
			atrousDMat := global.ComputeAccumulatedCostMatrix(distMat)
			_, pathLen, disScore = global.FindBestPath(atrousDMat, distMat)
		}
		if disScore < float64(val.Score) && ((pathLen+1) > int(float32(len(val.Tem.NormP3d))*val.PathLen) || pathLen == 100) {
			pos.ActionWindow.Add(k)
			// if pos.ActionWindow.MaxCount() == k {
			logrus.Infof("rule action: %s, %v", Action(k).String(), disScore, pathLen+1, int(float32(len(val.Tem.NormP3d))*val.PathLen))
			go pos.Xbox.SetXbox(action)
			// }
		}
	}
}

func KeyNodePos(inputPos [][][]float64, keyNode []int) (outputPos [][][]float64) {
	for _, val := range inputPos {
		pos := [][]float64{}
		for _, v := range keyNode {
			pos = append(pos, val[v])
		}
		outputPos = append(outputPos, pos)
	}
	return
}
