package joystick

import (
	"errors"
	"meta/devices/vjoy/joystick/dll"
)

var (
	ErrDeviceAlreadyOwned = errors.New("vJoy Device already open by this application")
	ErrDeviceBusy         = errors.New("vJoy Device is owned by another application")
	ErrDeviceMissing      = errors.New("vJoy Device is missing: either does not exist or the driver is down")
	ErrDeviceUnknown      = errors.New("Unknown vJoy Device error")
	ErrReset              = errors.New("Reset failed")
	ErrUpdate             = errors.New("UpdateVJD failed")
	ErrUnknownName        = errors.New("Unknown name (axis, button or pov)")
)

// Available check if the vjoy.dll was successfully loaded and is enabled.
// Other functions in this library will likely panic if Available returns
// false.
func Available() bool {
	err := dll.Load()
	return err == nil && dll.VJoyEnabled()
}

// Version returns the version number of the installed vJoy.
func Version() uint {
	return uint(dll.GetvJoyVersion())
}

func ProductString() string {
	return dll.GetvJoyProductString()
}

func ManufacturerString() string {
	return dll.GetvJoyManufacturerString()
}

func SerialNumberString() string {
	return dll.GetvJoySerialNumberString()
}

// ResetAll resets all VJD devices
func ResetAll() error {
	if dll.ResetAll() {
		return nil
	}
	return ErrReset
}

type AxisName uint

const (
	AxisX AxisName = iota
	AxisY
	AxisZ
	AxisRX
	AxisRY
	AxisRZ
	Slider0
	Slider1
	MaxAxis

	MaxButton = 128
)

func axisNumber(a AxisName) uint {
	switch a {
	case AxisX:
		return dll.HID_USAGE_X
	case AxisY:
		return dll.HID_USAGE_Y
	case AxisZ:
		return dll.HID_USAGE_Z
	case AxisRX:
		return dll.HID_USAGE_RX
	case AxisRY:
		return dll.HID_USAGE_RY
	case AxisRZ:
		return dll.HID_USAGE_RZ
	case Slider0:
		return dll.HID_USAGE_SL0
	case Slider1:
		return dll.HID_USAGE_SL1
	}
	return 0
}

type Axis struct {
	p        *int32
	exists   bool
	min, max int32
}

func (a *Axis) Exists() bool { return a.exists }

func (a *Axis) Setu(val int) { *(a.p) = int32(val & 0x7fff) } // 0..0x7fff
func (a *Axis) Seti(val int) { a.Setu(val - 0x4000) }         // -0x4000..0x3fff

// Setc is same as Setu, but truncates values to permitted bounds
func (a *Axis) Setc(val int) {
	switch {
	case val < 0:
		val = 0
	case 0x7fff < val:
		val = 0x7fff
	}
	a.Setu(val)
}

func (a *Axis) Setf(val float32) {
	var v int
	if val < 0 {
		val += 1
		if val < 0 {
			val = 0
		}
		v = int(val * 0x4000)
	} else {
		if val > 1 {
			val = 1
		}
		v = 0x4000 + int(val*0x3fff)
	}
	a.Setu(v)
}

func (a *Axis) Setuf(val float32) { a.Setc(int(val * 0x7fff)) } // 0..1

type Button struct {
	p      *int32
	mask   int32
	exists bool
}

func (b *Button) Exists() bool {
	return b.exists
}

func (b *Button) Set(val bool) {
	if val {
		*(b.p) |= b.mask
	} else {
		*(b.p) &= ^b.mask
	}
}

type HatState int

const (
	HatN   HatState = 0
	HatE   HatState = 1
	HatS   HatState = 2
	HatW   HatState = 3
	HatOff HatState = -1
)

type Hat interface {
	Exists() bool
	SetDiscrete(HatState)
	SetDegp(int) // set value in degree-percents (-1: off, 0-360000: direction)
}

type continuousHat struct {
	p      *uint32
	shift  uint
	exists bool
}

func (h *continuousHat) Exists() bool {
	return h.exists
}

func (h *continuousHat) SetDiscrete(s HatState) {
	mask := uint32(0xffff) << h.shift
	val := int(s)
	if val > 0 {
		val *= 9000
	}
	*(h.p) = (*(h.p) & mask) | (uint32(val) << h.shift)
}

func (h *continuousHat) SetDegp(val int) {
	mask := uint32(0xffff) << h.shift
	*(h.p) = (*(h.p) & mask) | (uint32(val) << h.shift)
}

type discreteHat struct {
	p      *uint32
	shift  uint
	exists bool
}

func (h *discreteHat) Exists() bool {
	return h.exists
}

func (h *discreteHat) SetDiscrete(val HatState) {
	mask := uint32(0xf) << h.shift
	vsh := (uint32(val) & 0xf) << h.shift
	*(h.p) = (*(h.p) & ^mask) | vsh
}

func (h *discreteHat) SetDegp(val int) {
	if val > 0 {
		val = ((val + 4500) / 9000) % 4
	}
	mask := uint32(0xffff) << h.shift
	*(h.p) = (*(h.p) & mask) | (uint32(val) << h.shift)
}

// Device represents an open vJoy device
type Device struct {
	rid           uint                  // rid Device opened with
	st            dll.JOYSTICK_POSITION // state for Update()
	noi           int32
	nou           uint32
	axes          []*Axis
	buttons       []*Button
	hats          []Hat
	invalidaxis   *Axis
	invalidbutton *Button
	invalidhat    Hat
}

// Acquire opens a Device for use in the application.
func Acquire(rid uint) (*Device, error) {
	d := &Device{rid: rid}
	err := d.init()
	if err != nil {
		return nil, err
	}
	d.Reset()
	d.Update()
	return d, nil
}

// Relinquish closes an acquired device
func (d *Device) Relinquish() {
	dll.RelinquishVJD(d.rid)
}

// reset Device: Axes centered, Buttons and Hats off.
func (d *Device) Reset() {
	const center = 0x4000
	d.st.Throttle = center
	d.st.Rudder = center
	d.st.Aileron = center
	d.st.AxisX = center
	d.st.AxisY = center
	d.st.AxisZ = center
	d.st.AxisXRot = center
	d.st.AxisYRot = center
	d.st.AxisZRot = center
	d.st.Slider = center
	d.st.Dial = center
	d.st.Wheel = center
	d.st.AxisVX = center
	d.st.AxisVY = center
	d.st.AxisVZ = center
	d.st.AxisVBRX = center
	d.st.AxisVBRY = center
	d.st.AxisVBRZ = center
	d.st.Buttons = 0
	const hatoff = 0xffffffff
	d.st.Hats = hatoff
	d.st.HatsEx1 = hatoff
	d.st.HatsEx2 = hatoff
	d.st.HatsEx3 = hatoff
}

// Axis returns the given Axis to update
// cached values for this device, to be submitted
// by Update()
func (d *Device) Axis(n AxisName) *Axis {
	if int(n) < len(d.axes) {
		return d.axes[n]
	}
	return d.invalidaxis
}

// return Button number n
func (d *Device) Button(n uint) *Button {
	if int(n) < len(d.buttons) {
		return d.buttons[n]
	}
	return d.invalidbutton
}

// return Hat number n
// hats aren't yet supported by vjoy
func (d *Device) Hat(n int) Hat {
	if 0 <= n && n < len(d.hats) {
		return d.hats[n]
	}
	return d.invalidhat
}

func (d *Device) Axes() []*Axis      { return d.axes }
func (d *Device) Buttons() []*Button { return d.buttons }
func (d *Device) Hats() []Hat        { return d.hats }

// Update VJD with values changed with in
// Button, Hat and Axis objects.
func (d *Device) Update() error {
	if dll.UpdateVJD(d.rid, &d.st) {
		return nil
	}
	return ErrUpdate
}

func (d *Device) init() error {
	d.st.Device = byte(d.rid)
	var err error
	switch dll.GetVJDStatus(d.rid) {
	case dll.VJD_STAT_OWN:
		return ErrDeviceAlreadyOwned
	case dll.VJD_STAT_FREE:
		// no op
	case dll.VJD_STAT_BUSY:
		err = ErrDeviceBusy
	case dll.VJD_STAT_MISS:
		err = ErrDeviceMissing
	default:
		//case VJD_STAT_UNKN:
		err = ErrDeviceUnknown
	}
	if !dll.AcquireVJD(d.rid) {
		return err
	}

	d.invalidaxis = &Axis{p: &d.noi, exists: false}
	d.invalidbutton = &Button{p: &d.noi, exists: false}
	d.invalidhat = &discreteHat{p: &d.nou, exists: false}

	d.axes = make([]*Axis, MaxAxis)
	for i := AxisName(0); i < MaxAxis; i++ {
		axn := axisNumber(i)
		a := &Axis{exists: dll.GetVJDAxisExist(d.rid, axn)}
		switch i {
		case AxisX:
			a.p = &d.st.AxisX
		case AxisY:
			a.p = &d.st.AxisY
		case AxisZ:
			a.p = &d.st.AxisZ
		case AxisRX:
			a.p = &d.st.AxisXRot
		case AxisRY:
			a.p = &d.st.AxisYRot
		case AxisRZ:
			a.p = &d.st.AxisZRot
		case Slider0:
			a.p = &d.st.Slider
		case Slider1:
			a.p = &d.st.Dial
		default:
			a.p = &d.noi // unused
		}
		if a.exists {
			dll.GetVJDAxisMin(d.rid, axn, &a.min)
			dll.GetVJDAxisMax(d.rid, axn, &a.max)
		}
		d.axes[i] = a
	}
	n := dll.GetVJDButtonNumber(d.rid)
	for i := 0; i < n; i++ {
		p, mask := d.button(i)
		if p != nil {
			d.buttons = append(d.buttons, &Button{p: p, exists: true, mask: mask})
		} else {
			d.buttons = append(d.buttons, d.invalidbutton)
		}
	}

	n = dll.GetVJDDiscPovNumber(d.rid)
	for i := 0; i < n; i++ {
		p, shift := d.dhat(i)
		if p != nil {
			d.hats = append(d.hats, &discreteHat{p: p, exists: true, shift: shift})
		}
	}

	n = dll.GetVJDContPovNumber(d.rid)
	for i := 0; i < n; i++ {
		p, shift := d.chat(i)
		if p != nil {
			d.hats = append(d.hats, &continuousHat{p: p, exists: true, shift: shift})
		}
	}

	return nil
}

func (d *Device) button(i int) (p *int32, mask int32) {
	switch i / 32 {
	case 0:
		p = &d.st.Buttons
	case 1:
		p = &d.st.ButtonsEx1
	case 2:
		p = &d.st.ButtonsEx2
	case 3:
		p = &d.st.ButtonsEx3
	}
	mask = 1 << uint(i%32)
	return
}

func (d *Device) dhat(i int) (p *uint32, shift uint) {
	p = &d.st.Hats
	shift = uint(i) * 4
	return
}

func (d *Device) chat(i int) (p *uint32, shift uint) {
	shift = uint(i%2) * 16
	switch i / 2 {
	case 0:
		p = &d.st.Hats
	case 1:
		p = &d.st.HatsEx1
	case 2:
		p = &d.st.HatsEx2
	case 3:
		p = &d.st.HatsEx3
	}
	return
}
