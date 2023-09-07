package config

type A struct {
	HandHightHead float64
	FeetThanHip   float64
}

type B struct {
	FeetHeight           float64
	LeftAndRightFeetXY   float64
	RightHandThanHeadXYZ float64
}

type C struct {
	LeftHandThanHeadZ float64
}

type D struct {
	HandAndTipToeDistance float64
	HeadAndHipDistanceZ   float64
}

type Greet struct {
	ElbowAngle         float64
	HandMoveDistanceXZ float64
}

type RaiseRightHand struct {
	HandThanHeadZ float64
}

type LeftSlide struct {
	HandMoveDistanceX float64
	HandToShoulderXZ  float64
}

type LeftRightTilt struct {
	TiltAngle     float64
	ShoulderWidth float64
}

type ElbowBend struct {
	MinAngle float64
	MaxAngle float64
}

type Squat struct {
	KneeAngle float64
}

type Rules struct {
	A              A
	B              B
	C              C
	D              D
	Greet          Greet
	RaiseRightHand RaiseRightHand
	LeftSlide      LeftSlide
	LeftRightTilt  LeftRightTilt
	ElbowBend      ElbowBend
	Squat          Squat
}
