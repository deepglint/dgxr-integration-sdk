package rule

import "reverie/model/source"

func RaiseHandRight(data *source.Source) bool {
	if pos, err := data.LastData(); err != nil {
		return false
	} else if pos.Objs[23][2]-pos.Objs[21][2] > 0.15 {
		return true
	}
	return false
}
