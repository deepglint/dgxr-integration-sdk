package action

import (
	"reverie/action/rule"
	"reverie/global"
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
	Registry.Register(SlideUp, rule.SlideUp)
	Registry.Register(SlideDown, rule.SlideDown)
	Registry.Register(SmallSquat, rule.Squat)
	Registry.Register(LeanToRight, rule.RightTilt)
	Registry.Register(LeanToLeft, rule.LeftTilt)
	Registry.Register(BendBothElbows, rule.ElbowBend)
	Registry.Register(Stand, rule.Stand)
	Registry.Register(HandsCross, rule.HandsCross)
}

func RuleToXbox(pos *source.Source) {
	for k, v := range global.Config.Action {
		if Registry.Run(Action(k), pos) {
			logrus.Infof("rule action: %s", Action(k).String())
			go pos.Xbox.SetXbox(v)
		}
	}
}

func ModelToXbox(pos *source.Source, action int32) {
	if v, ok := global.Config.Action[int(action)]; ok {
		logrus.Infof("model action: %s", Action(action).String())
		go pos.Xbox.SetXbox(v)
	}
}
