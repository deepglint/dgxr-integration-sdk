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

type LeftRightSlide struct {
	HandMoveDistanceX float64
	HandToShoulderXZ  float64
}

type UpSlide struct {
	HandMoveDistanceY float64
	HandMoveDistanceZ float64
	ElbowAngle        float64
	HandToShoulderZ   float64
}

type DownSlide struct {
	HandMoveDistanceX float64
	HandMoveDistanceY float64
	HandMoveDistanceZ float64
	ElbowAngle        float64
	HandToHipZ        float64
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
type Jump struct {
	KneeHeight float64
}

type HandsCross struct {
	LeftMinCalculate  float64
	LeftMaxCalculate  float64
	RightMinCalculate float64
	RightMaxCalculate float64
}

type Rules struct {
	A              A
	B              B
	C              C
	D              D
	Greet          Greet
	RaiseRightHand RaiseRightHand
	LeftRightSlide LeftRightSlide
	UpSlide        UpSlide
	DownSlide      DownSlide
	LeftRightTilt  LeftRightTilt
	ElbowBend      ElbowBend
	Squat          Squat
	Jump           Jump
	HandsCross     HandsCross
}
