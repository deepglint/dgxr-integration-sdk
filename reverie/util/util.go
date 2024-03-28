package util

import (
	"io/ioutil"
	"net/http"
)

const ConfigPath = "./"

func Get(url string) (data []byte, err error) {
	resp, err := http.Get(url)
	if err != nil {
		return
	}
	defer resp.Body.Close()

	data, err = ioutil.ReadAll(resp.Body)
	if err != nil {
		return
	}
	return
}

func UnifyCoordinate(xDirection, yDirection string, pose []float32) []float64 {
	switch {
	case xDirection == "left" && yDirection == "up":
		return []float64{-float64(pose[0]), float64(pose[1]), float64(pose[2])}
	case xDirection == "left" && yDirection == "down":
		return []float64{-float64(pose[0]), -float64(pose[1]), float64(pose[2])}
	case xDirection == "right" && yDirection == "up":
		return []float64{float64(pose[0]), float64(pose[1]), float64(pose[2])}
	case xDirection == "right" && yDirection == "down":
		return []float64{float64(pose[0]), -float64(pose[1]), float64(pose[2])}
	case xDirection == "up" && yDirection == "left":
		return []float64{float64(pose[1]), -float64(pose[0]), float64(pose[2])}
	case xDirection == "up" && yDirection == "right":
		return []float64{-float64(pose[1]), float64(pose[0]), float64(pose[2])}
	case xDirection == "down" && yDirection == "left":
		return []float64{-float64(pose[1]), -float64(pose[0]), float64(pose[2])}
	case xDirection == "down" && yDirection == "right":
		return []float64{float64(pose[1]), -float64(pose[0]), float64(pose[2])}
	default:
		// 默认情况，假设 x 和 y 方向都为右上方向
		return []float64{float64(pose[0]), float64(pose[1]), float64(pose[2])}
	}
}
