package config

// A:
//
//	  handHightHead: 0 #手高于头的距离，建议 0～0.1
//	  feetThanHip: 0   #双脚大于胯部的距离，建议 0～0.1
//	B:
//	  feetHeight: 0.2 #双脚之间的高度差，建议 0.2
//	  leftAndRightFeetXY: 0.2 #左右脚之间的XY 上的位移，建议 0.2
//	  rightHandThanHeadXYZ: 0.3 #右手和头的三维空间距离，建议 0.3
//	C:
//	  leftHandThanHeadZ: 0.2 #左手和头的高度距离，建议 0.2
//	D:
//	  handAndTipToeDistance: 0.2 #双手和双脚三维空间的距离，建议 0.2
//	  headAndHipDistanceZ: 0.2 #头和臀部在Z 轴上的空间距离，建议 0.2
//	greet:
//	  elbowAngle: 100 #手肘的角度，建议 100
//	  handMoveDistanceXZ: 0.15 #手在 XZ 二维平面的移动距离，建议 0.15
//	raiseRightHand:
//	  handThanHeadZ: 0.1 #手（Z方向）高于头的距离，建议 0.1～0.2
//	leftSlide:
//	  handMoveDistanceX: 0 #在X轴的移动距离，建议 0.2
//	  HandToShoulderXZ: 0.05 #手和肩膀在XZ上的绝对值距离，建议 0.05
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

type Rules struct {
	A              A
	B              B
	C              C
	D              D
	Greet          Greet
	RaiseRightHand RaiseRightHand
	LeftSlide      LeftSlide
}
