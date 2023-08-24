package rule

import (
	"fmt"
	"meta/model/source"
)

func C(data *source.Source) bool {
	if pos, err := data.LastData(); err != nil {
		return false
	} else {
		// 左手高于头部一定的距离，且左右手X 都在头部的右边

		// 左手高于头部一定的距离
		LeftHandAndHead := pos.Objs[source.LeftHand][2] - pos.Objs[source.HeadTop][2]

		fmt.Println("C------", LeftHandAndHead)

		if LeftHandAndHead > 0.2 && pos.Objs[source.LeftHand][0]-pos.Objs[source.HeadTop][0] > 0 && pos.Objs[source.RightHand][0]-pos.Objs[source.HeadTop][0] > 0 {
			fmt.Println("C")
			return true
		}
		return false
	}
}
