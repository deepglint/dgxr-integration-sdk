package source

import (
	"math"
)

// 计算两点之间的向量表示
func Vector(p1, p2 Point) Point {
	return Point{p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z}
}

// 计算向量的点积
func DotProduct(v1, v2 Point) float64 {
	return v1.X*v2.X + v1.Y*v2.Y + v1.Z*v2.Z
}

// 计算向量的模长
func Magnitude(v Point) float64 {
	return math.Sqrt(v.X*v.X + v.Y*v.Y + v.Z*v.Z)
}

// 计算两个向量的夹角（弧度）
func AngleBetweenVectors(v1, v2 Point) float64 {
	dot := DotProduct(v1, v2)
	mag1 := Magnitude(v1)
	mag2 := Magnitude(v2)

	// 使用反余弦函数计算夹角的弧度值
	radian := math.Acos(dot / (mag1 * mag2))
	return radian
}

func XYZSum(a []float64) float64 {
	if len(a) != 3 {
		return 0
	}
	return a[0] + a[1] + a[2]
}
