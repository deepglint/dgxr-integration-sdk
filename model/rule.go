package model

import (
	"math"
)

// CalculateAngle 计算三点之间的夹角
func CalculateAngle(a, b, c []float32) float64 {
	if len(a) < 3 || len(b) < 3 || len(c) < 3 {
		return 0
	}
	ab := []float64{float64(b[0] - a[0]), float64(b[1] - a[1]), float64(b[2] - a[2])}
	bc := []float64{float64(c[0] - b[0]), float64(c[1] - b[1]), float64(c[2] - b[2])}

	dotProduct := ab[0]*bc[0] + ab[1]*bc[1] + ab[2]*bc[2]
	magnitudeAB := math.Sqrt(ab[0]*ab[0] + ab[1]*ab[1] + ab[2]*ab[2])
	magnitudeBC := math.Sqrt(bc[0]*bc[0] + bc[1]*bc[1] + bc[2]*bc[2])

	cosine := dotProduct / (magnitudeAB * magnitudeBC)
	radians := math.Acos(cosine)
	degrees := radians * 180 / math.Pi
	return degrees
}

type Point struct {
	X, Y, Z float64
}

// 计算线段的旋转角度
func CalculateRotateAngle(a, b []float32) float64 {
	// 计算线段AB的方向向量
	directionVector := Point{
		X: float64(b[0] - a[0]),
		Y: float64(b[1] - a[1]),
		Z: float64(b[2] - a[2]),
	}

	// 计算线段AB的长度
	lengthAB := math.Sqrt(math.Pow(directionVector.X, 2) + math.Pow(directionVector.Y, 2) + math.Pow(directionVector.Z, 2))

	// 将方向向量AB标准化
	unitVector := Point{
		X: directionVector.X / lengthAB,
		Y: directionVector.Y / lengthAB,
		Z: directionVector.Z / lengthAB,
	}

	// 定义参考向量
	refVector := Point{1.0, 0.0, 0.0} // 假设为X轴正方向的单位向量

	// 计算旋转角度（弧度）
	angle := math.Acos(dotProduct(unitVector, refVector))

	// 将角度转换为度数
	angleDegrees := angle * 180 / math.Pi

	return angleDegrees
}

// 计算两个向量的点积
func dotProduct(vector1, vector2 Point) float64 {
	return vector1.X*vector2.X + vector1.Y*vector2.Y + vector1.Z*vector2.Z
}

// 判断a是否大于b
func AThanB(a, b float32) bool {
	return a > b
}

// 计算两个点之间的中心点
func CalculateCenterPoint(a, b []float32) []float64 {
	return []float64{
		(float64(a[0]) + float64(b[0])) / 2,
		(float64(a[1]) + float64(b[1])) / 2,
		(float64(a[2]) + float64(b[2])) / 2,
	}
}
