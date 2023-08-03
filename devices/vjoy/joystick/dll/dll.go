package dll

// written for VJD Version 2.0.4 Alpha Release – June 2014

import (
	"log"
	"unsafe"
)

// constants used for GetVJDStatus()
const (
	VJD_STAT_OWN  = iota // The vJoy Device is owned by this application.
	VJD_STAT_FREE        // The vJoy Device is NOT owned by any application (including this one).
	VJD_STAT_BUSY        // The vJoy Device is owned by another application. It cannot be acquired by this application.
	VJD_STAT_MISS        // The vJoy Device is missing. It either does not exist or the driver is down.
	VJD_STAT_UNKN        // Unknown
)

const (
	HID_USAGE_X   = 0x30
	HID_USAGE_Y   = 0x31
	HID_USAGE_Z   = 0x32
	HID_USAGE_RX  = 0x33
	HID_USAGE_RY  = 0x34
	HID_USAGE_RZ  = 0x35
	HID_USAGE_SL0 = 0x36
	HID_USAGE_SL1 = 0x37
	HID_USAGE_WHL = 0x38
	HID_USAGE_POV = 0x39
)

type JOYSTICK_POSITION struct {
	Device     byte // Index of device. 1-based.
	Throttle   int32
	Rudder     int32
	Aileron    int32
	AxisX      int32
	AxisY      int32
	AxisZ      int32
	AxisXRot   int32
	AxisYRot   int32
	AxisZRot   int32
	Slider     int32
	Dial       int32
	Wheel      int32
	AxisVX     int32
	AxisVY     int32
	AxisVZ     int32
	AxisVBRX   int32
	AxisVBRY   int32
	AxisVBRZ   int32
	Buttons    int32  // 32 buttons: 0x00000001 means button1 is pressed, 0x80000000 -> button32 is pressed
	Hats       uint32 // Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
	HatsEx1    uint32 // Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
	HatsEx2    uint32 // Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
	HatsEx3    uint32 // Lower 4 bits: HAT switch or 16-bit of continuous HAT switch
	ButtonsEx1 int32  // Buttons 33-64
	ButtonsEx2 int32  // Buttons 65-96
	ButtonsEx3 int32  // Buttons 97-128
}

// Load loads the vJoyInterface dll. It can be used to check
// if functions in this library are available. Other functions
// will panic if the DLL could not be loaded.
func Load() error {
	return dll.Load()
}

// GetVersion returns the version number of the installed vJoy.
// To be used only after vJoyEnabled()
func GetvJoyVersion() uint16 {
	r, _, _ := procGetvJoyVersion.Call()
	return uint16(r)
}

// Returns wether vJoy version 2.x is installed and enabled.
func VJoyEnabled() bool {
	r, _, _ := procGetvJoyVersion.Call()
	return r != 0
}

func GetvJoyProductString() string {
	r, _, _ := procGetvJoyProductString.Call()
	return makeString(r)
}

func GetvJoyManufacturerString() string {
	r, _, _ := procGetvJoyManufacturerString.Call()
	return makeString(r)
}

func GetvJoySerialNumberString() string {
	r, _, _ := procGetvJoySerialNumberString.Call()
	return makeString(r)
}

// Get the number of buttons defined in the specified VDJ
func GetVJDButtonNumber(rID uint) int {
	r, _, _ := procGetVJDButtonNumber.Call(uintptr(rID))
	return int(r)
}

// Get the number of descrete-type POV hats defined in the specified VDJ
func GetVJDDiscPovNumber(rID uint) int {
	r, _, _ := procGetVJDDiscPovNumber.Call(uintptr(rID))
	return int(r)
}

// Get the number of descrete-type POV hats defined in the specified VDJ
func GetVJDContPovNumber(rID uint) int {
	r, _, _ := procGetVJDContPovNumber.Call(uintptr(rID))
	return int(r)
}

// Test if given axis defined in the specified VDJ
func GetVJDAxisExist(rID uint, axis uint) bool {
	r, _, _ := procGetVJDAxisExist.Call(uintptr(rID), uintptr(axis))
	return r != 0
}

// Get logical Maximum value for a given axis defined in the specified VDJ
func GetVJDAxisMax(rID uint, axis uint, max *int32) bool {
	r, _, _ := procGetVJDAxisMax.Call(uintptr(rID), uintptr(axis), uintptr(unsafe.Pointer(max)))
	return r != 0
}

// Get logical Minimum value for a given axis defined in the specified VDJ
func GetVJDAxisMin(rID uint, axis uint, min *int32) bool {
	r, _, _ := procGetVJDAxisMin.Call(uintptr(rID), uintptr(axis), uintptr(unsafe.Pointer(min)))
	return r != 0
}

// Acquire the specified vJoy Device.
func AcquireVJD(rID uint) bool {
	r, _, _ := procAcquireVJD.Call(uintptr(rID))
	return r != 0
}

// Relinquish the specified vJoy Device.
func RelinquishVJD(rID uint) bool {
	r, _, _ := procRelinquishVJD.Call(uintptr(rID))
	return r != 0
}

// Update the position data of the specified vJoy Device.
func UpdateVJD(rID uint, data *JOYSTICK_POSITION) bool {
	r, _, _ := procUpdateVJD.Call(uintptr(rID), uintptr(unsafe.Pointer(data)))
	return r != 0
}

// Get the status of the specified vJoy Device.
func GetVJDStatus(rID uint) int {
	r, _, _ := procGetVJDStatus.Call(uintptr(rID))
	return int(r)
}

// Reset all controls to predefined values in the specified VDJ
func ResetVJD(rID uint) bool {
	r, _, _ := procResetVJD.Call(uintptr(rID))
	return r != 0
}

// Reset all controls to predefined values in all VDJ
func ResetAll() bool {
	r, _, _ := procResetAll.Call()
	return r != 0
}

// Reset all buttons (To 0) in the specified VDJ
func ResetButtons(rID uint) bool {
	r, _, _ := procResetButtons.Call(uintptr(rID))
	return r != 0
}

// Reset all POV Switches (To -1) in the specified VDJ
func ResetPovs(rID uint) bool {
	r, _, _ := procResetPovs.Call(uintptr(rID))
	return r != 0
}

// Write Value to a given axis defined in the specified VDJ
func SetAxis(value int, rID, axis uint) bool {
	r, _, _ := procSetAxis.Call(uintptr(value), uintptr(rID), uintptr(axis))
	return r != 0
}

// Write Value to a given button defined in the specified VDJ
func SetBtn(value bool, rID uint, btn byte) bool {
	var vi uintptr
	if value {
		vi = 1
	} else {
		vi = 0
	}
	r, _, _ := procSetBtn.Call(uintptr(vi), uintptr(rID), uintptr(btn))
	return r != 0
}

// Write Value to a given descrete POV defined in the specified VDJ
// pov should be in range 1..4
// value can be in the range: -1..3. 0: North, 1: East, 2: South, 3: West
// -1 means Neutral (Nothing pressed).
func SetDiscPov(value int, rID uint, pov byte) bool {
	r, _, _ := procSetDiscPov.Call(uintptr(value), uintptr(rID), uintptr(pov))
	return r != 0
}

// Write Value to a given continuous POV defined in the specified VDJ.
// pov should be in range 1..4
// value can be in the range: -1..35999. It is measured in units of one-hundredth a degree.
// -1 means Neutral (Nothing pressed).
func SetContPov(value uint32, rID uint, pov byte) bool {
	r, _, _ := procSetContPov.Call(uintptr(value), uintptr(rID), uintptr(pov))
	return r != 0
}

func init() {
	if dll.Load() == nil {
		ver := GetvJoyVersion()
		if ver != 516 {
			log.Println("Warning: untested vJoy version:", ver, "!= 516")
		}
	}
}
