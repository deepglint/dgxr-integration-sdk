package rule

import (
	"math"
	"reverie/global"
	"reverie/model/source"
)

func A(data *source.Source) bool {
	if pos, err := data.LastData(); err != nil {
		return false
	} else {
		// 两个脚的距离大于胯部距离且两个手超过头顶
		leftHandHightHead := pos.Objs[source.LeftHand][2] - pos.Objs[source.HeadTop][2]
		rightHandHightHead := pos.Objs[source.RightHand][2] - pos.Objs[source.HeadTop][2]
		tipDistance := math.Abs(pos.Objs[source.RightTiptoe][0] - pos.Objs[source.LeftTiptoe][0])
		hidDistance := math.Abs(pos.Objs[source.LeftHip][0] - pos.Objs[source.RightHip][0])
		if leftHandHightHead > global.Config.Rules.A.HandHightHead && rightHandHightHead > global.Config.Rules.A.HandHightHead && tipDistance-hidDistance > global.Config.Rules.A.FeetThanHip {
			return true
		}
		return false
	}
}
