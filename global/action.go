package global

import (
	"meta/action/rule"
	"meta/model/source"
	"time"

	"github.com/sirupsen/logrus"
)

type Action int
type ActionFunc func(*source.Source) bool

const (
	A      Action = 0
	B      Action = 1
	C      Action = 1
	D      Action = 3
	HandUp        = 23 //举手

	SlowRun   = 19 // 慢跑
	FastRun   = 20 // 快跑
	CheerUp   = 24 // 欢呼
	JumpUp    = 25 // 起跳
	SquatDown = 26 // 下蹲
)

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
	// result := registry.Run(A, pos)
	// fmt.Println("ActionA result:", result)
}

func Test() {
	for {
		// TODO 待优化放到收到 pos 的时候调用这个方法
		time.Sleep(10 * time.Millisecond)

	}
}

func RuleToXbox(pos *source.Source) {
	for k, v := range Config.Action {
		if Registry.Run(Action(k), pos) {
			go pos.Xbox.SetButton(v)
		}
	}
}

func ModelToXbox(pos *source.Source, action int32) {
	if v, ok := Config.Action[int(action)]; ok {
		go pos.Xbox.SetButton(v)
	}
}
