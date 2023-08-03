package dll

import (
	"syscall"
	"unsafe"
)

var dll = syscall.NewLazyDLL("devices/vjoy/joystick/dll/vJoyInterface.dll")

var (
	procGetvJoyVersion            = dll.NewProc("GetvJoyVersion")
	procvJoyEnabled               = dll.NewProc("vJoyEnabled")
	procGetvJoyProductString      = dll.NewProc("GetvJoyProductString")
	procGetvJoyManufacturerString = dll.NewProc("GetvJoyManufacturerString")
	procGetvJoySerialNumberString = dll.NewProc("GetvJoySerialNumberString")
	procGetVJDButtonNumber        = dll.NewProc("GetVJDButtonNumber")
	procGetVJDDiscPovNumber       = dll.NewProc("GetVJDDiscPovNumber")
	procGetVJDContPovNumber       = dll.NewProc("GetVJDContPovNumber")
	procGetVJDAxisExist           = dll.NewProc("GetVJDAxisExist")
	procGetVJDAxisMax             = dll.NewProc("GetVJDAxisMax")
	procGetVJDAxisMin             = dll.NewProc("GetVJDAxisMin")
	procAcquireVJD                = dll.NewProc("AcquireVJD")
	procRelinquishVJD             = dll.NewProc("RelinquishVJD")
	procUpdateVJD                 = dll.NewProc("UpdateVJD")
	procGetVJDStatus              = dll.NewProc("GetVJDStatus")
	procResetVJD                  = dll.NewProc("ResetVJD")
	procResetAll                  = dll.NewProc("ResetAll")
	procResetButtons              = dll.NewProc("ResetButtons")
	procResetPovs                 = dll.NewProc("ResetPovs")
	procSetAxis                   = dll.NewProc("SetAxis")
	procSetBtn                    = dll.NewProc("SetBtn")
	procSetDiscPov                = dll.NewProc("SetDiscPov")
	procSetContPov                = dll.NewProc("SetContPov")
)

func makeString(s uintptr) string {
	n := 0
	for p := s; *(*uint16)(unsafe.Pointer(p)) != 0; {
		p += 2
		n++
	}
	buf := make([]uint16, n)
	for p, i := s, 0; i < n; {
		buf[i] = *(*uint16)(unsafe.Pointer(p))
		p += 2
		i++
	}
	return syscall.UTF16ToString(buf)
}
