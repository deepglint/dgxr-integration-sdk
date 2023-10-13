package rule

import (
	"reverie/global"
	"reverie/model/source"
)

func D(data *source.Source) bool {
	if pos, err := data.LastData(); err != nil {
		return false
	} else {
		// 做 D 的动作的时候，不区分面相前面还是后面还是左右哦
		// 手和脚的 高度在一定范围内且头和胯部的高度再一定范围内
		// 手和脚在三维空间中的距离
		HandAndTipToeDistance := (source.XYZSum(pos.Objs[source.LeftHand]) + source.XYZSum(pos.Objs[source.RightHand])) - (source.XYZSum(pos.Objs[source.LeftTiptoe]) + source.XYZSum(pos.Objs[source.RightTiptoe]))
		// 头和胯部的高度再一定范围内
		HeadAndHipDistance := pos.Objs[source.HeadTop][2] - pos.Objs[source.LeftHip][2]
		if HandAndTipToeDistance < global.Config.Rules.D.HandAndTipToeDistance && HeadAndHipDistance < global.Config.Rules.D.HeadAndHipDistanceZ {
			return true
		}

		return false
	}
}
