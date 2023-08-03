package rule

import (
	"meta/common/models/sources"
)

func RaiseHandRight(pos sources.SourceData) bool {
	if pos.Objs[23][2]-pos.Objs[21][2] > 0.15 {
		return true
	}
	return false
}
