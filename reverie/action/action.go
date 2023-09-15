package action

import (
	"meta/action/rule"
	"meta/global"
	"meta/model/source"

	"github.com/sirupsen/logrus"
)

type Action int
type ActionFunc func(*source.Source) bool

const (
	A         Action = 0
	B         Action = 1
	C         Action = 2
	D         Action = 3
	Greet     Action = 4 // 招呼
	LeftSlide Action = 5 // 左边滑
	Squat     Action = 6 // 下蹲
	RightTilt Action = 7 // 右倾斜
	LeftTilt  Action = 8 // 左倾斜
	ElbowBend Action = 9 // 弯双肘

	HandUp = 23 //举手

	SlowRun   = 19 // 慢跑
	FastRun   = 20 // 快跑
	CheerUp   = 24 // 欢呼
	JumpUp    = 25 // 起跳
	SquatDown = 26 // 下蹲
)

func (a Action) String() string {
	switch a {
	case A:
		return "A"
	case B:
		return "B"
	case C:
		return "C"
	case D:
		return "D"
	case Greet:
		return "Greet"
	case LeftSlide:
		return "LeftSlide"
	case HandUp:
		return "HandUp"
	case SlowRun:
		return "SlowRun"
	case FastRun:
		return "FastRun"
	case CheerUp:
		return "CheerUp"
	case JumpUp:
		return "JumpUp"
	case SquatDown:
		return "SquatDown"
	case Squat:
		return "Squat"
	case RightTilt:
		return "RightTilt"
	case LeftTilt:
		return "LeftTilt"
	case ElbowBend:
		return "ElbowBend"
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
	logrus.Errorf("Action %s is not registered.\n", name)
	return false
}

var (
	Registry *ActionRegistry
)

func init() {
	Registry = &ActionRegistry{
		registry: make(map[Action]ActionFunc),
	}
	Registry.Register(A, rule.A)
	Registry.Register(B, rule.B)
	Registry.Register(C, rule.C)
	Registry.Register(D, rule.D)
	Registry.Register(HandUp, rule.RaiseHandRight)
	Registry.Register(Greet, rule.Greet)
	Registry.Register(LeftSlide, rule.LeftSlide)
	Registry.Register(Squat, rule.Squat)
	Registry.Register(RightTilt, rule.RightTilt)
	Registry.Register(LeftTilt, rule.LeftTilt)
	Registry.Register(ElbowBend, rule.ElbowBend)
}

func RuleToXbox(pos *source.Source) {
	for k, v := range global.Config.Action {
		if Registry.Run(Action(k), pos) {
			go pos.Xbox.SetButton(v)
		}
	}
}

func ModelToXbox(pos *source.Source, action int32) {
	if v, ok := global.Config.Action[int(action)]; ok {
		go pos.Xbox.SetButton(v)
	}
}
