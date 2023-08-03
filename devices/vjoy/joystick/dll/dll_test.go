package dll

import (
	"testing"
)

func TestFunctions(t *testing.T) {
	GetvJoyVersion()
	VJoyEnabled()
	GetvJoyProductString()
	GetvJoyManufacturerString()
	GetvJoySerialNumberString()
	rid := uint(1)
	GetVJDButtonNumber(rid)
	GetVJDContPovNumber(rid)
	GetVJDStatus(rid)
	ResetAll()
	if AcquireVJD(rid) {
		t.Log("vjd acquired")
		ResetVJD(rid)
		ResetButtons(rid)
		ResetPovs(rid)
		SetBtn(true, rid, 1)
		SetDiscPov(5, rid, 1)
		SetContPov(5, rid, 1)
		RelinquishVJD(rid)
	} else {
		t.Log("no vjd")
	}
}
