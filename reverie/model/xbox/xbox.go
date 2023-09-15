package xbox

import (
	"meta/source/output/xbox"
	"sync"
	"time"
)

type Xbox struct {
	ID     int
	IsUsed bool
	Mu     sync.Mutex
	Xbox   *xbox.Controller
}

type XboxPool struct {
	Pool []*Xbox
	Mu   sync.Mutex
}

func (xp *XboxPool) GetDevice() *Xbox {
	if xp == nil {
		return nil
	}
	xp.Mu.Lock()
	defer xp.Mu.Unlock()
	for _, v := range xp.Pool {
		if !v.IsUsed {
			v.IsUsed = true
			return v
		}
	}
	return nil
}

func (xp *XboxPool) ReturnDevice(device *Xbox) {
	if xp == nil {
		return
	}
	xp.Mu.Lock()
	defer xp.Mu.Unlock()
	device.IsUsed = false
}

// 按压所有手柄
func (xp *XboxPool) PressAllXbox() {
	if xp == nil {
		return
	}
	for _, v := range xp.Pool {
		report := xbox.Report{}
		report.SetButton(true, 1)
		v.Xbox.Send(&report)
	}
}

// 关闭所有手柄
func (xp *XboxPool) CloseAllXbox() {
	if xp == nil {
		return
	}
	for _, v := range xp.Pool {
		v.Xbox.Close()
		v = nil
	}
}

// 按键
func (xp *Xbox) SetButton(button int) {
	if xp == nil {
		return
	}
	if !xp.Mu.TryLock() {
		return
	}
	defer xp.Mu.Unlock()
	report := xbox.Report{}
	if button < 16 {
		report.SetButton(true, button)
	} else {
		switch button {
		case 16:
			// LeftStickUp
			report.SetStick(false, 0, 32767)
		case 17:
			// LeftStickDown
			report.SetStick(false, 0, -32768)
		case 18:
			// LeftStickLeft
			report.SetStick(false, -32768, 0)
		case 19:
			// LeftStickRight
			report.SetStick(false, 32767, 0)
		case 20:
			// RightStickUp
			report.SetStick(true, 0, 32767)
		case 21:
			// RightStickDown
			report.SetStick(true, 0, -32768)
		case 22:
			// RightStickLeft
			report.SetStick(true, -32768, 0)
		case 23:
			// RightStickRight
			report.SetStick(true, 32767, 0)
		case 24:
			// LeftTrigger
			report.SetTrigger(false, 250)
		case 25:
			// RightTrigger
			report.SetTrigger(true, 250)
		}
	}
	xp.Xbox.Send(&report)
	time.Sleep(10 * time.Millisecond)
	report = xbox.Report{}
	xp.Xbox.Send(&report)
}
