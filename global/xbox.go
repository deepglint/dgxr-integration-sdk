package global

import (
	box "meta/model/xbox"
	"meta/source/output/xbox"

	"sync"
	"time"

	"github.com/sirupsen/logrus"
)

var (
	Emu        *xbox.Emulator
	XboxDevice *box.XboxPool
)

func init() {

	if Emu == nil {
		Emulator, err := xbox.Open(func(vibration xbox.Vibration) {
			//device.Vibrate(vibration.LargeMotor, vibration.SmallMotor)
		})
		if err != nil {
			panic(err)
		}
		Emu = Emulator
	}
}

func Close() {
	if Emu != nil {
		Emu.Close()
	}
}

func CloseXboxPool() {
	for _, v := range XboxDevice.Pool {
		v.Xbox.Close()
	}
}

func NewXboxPool(size int) *box.XboxPool {
	Pool := []*box.Xbox{}
	for i := 0; i < size; i++ {
		time.Sleep(10 * time.Millisecond)
		con, err := Emu.Connect()
		if err != nil {
			logrus.Errorf("new xbox pool err: ", err.Error())
			continue
		}
		Pool = append(Pool, &box.Xbox{ID: i, Xbox: con, Mu: sync.Mutex{}})
	}
	PoolInstance := &box.XboxPool{Pool: Pool, Mu: sync.Mutex{}}
	return PoolInstance
}
