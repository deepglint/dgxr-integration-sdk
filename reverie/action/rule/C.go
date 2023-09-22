package rule

import (
	"reverie/global"
	"reverie/model/source"
)

func C(data *source.Source) bool {
	if pos, err := data.LastData(); err != nil {
		return false
	} else {
		// 左手高于头部一定的距离，且左右手X 都在头部的右边

		// 左手高于头部一定的距离
		LeftHandAndHead := pos.Objs[source.LeftHand][2] - pos.Objs[source.HeadTop][2]
		if LeftHandAndHead > global.Config.Rules.C.LeftHandThanHeadZ && pos.Objs[source.LeftHand][0]-pos.Objs[source.HeadTop][0] > 0 && pos.Objs[source.RightHand][0]-pos.Objs[source.HeadTop][0] > 0 {
			return true
		}
		return false
	}
}
