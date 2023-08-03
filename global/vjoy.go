package global

import (
	"meta/devices/vjoy/joystick"
	"time"

	"github.com/sirupsen/logrus"
)

var (
	// TODO vjoy
	VJoy *joystick.Device
)

func InitVjoy() {
	go func() {
		for {

			if VJoy == nil {
				device, err := joystick.Acquire(1)
				if err != nil {
					logrus.Error("vjoy acquire error", err)
				} else {
					VJoy = device
					logrus.Info("vjoy init success")
				}
			}
			time.Sleep(1 * time.Second)
		}

	}()
}

func Button(x uint) {
	go func() {
		defer func() {
			if err := recover(); err != nil {
				logrus.Error("vjoy button error", err)
			}
		}()
		if VJoy != nil {
			logrus.Info("vjoy button", x)
			VJoy.Button(x).Set(true)
			VJoy.Update()
			time.Sleep(100 * time.Millisecond)
			VJoy.Button(x).Set(false)
			VJoy.Update()
		}
	}()

}
