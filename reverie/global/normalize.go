package global

import (
	"math"
	"reverie/model/template"
)

// GetLimbs calculates limb vectors of a 3D human skeleton
func GetLimbs(p3d [][]float64) [][]float64 {
	D := len(p3d[0])
	limbs := make([][]float64, 14)

	for i := 0; i < 14; i++ {
		limbs[i] = make([]float64, D)
	}

	limbs[0] = subtract(p3d[0], p3d[1])    // neck
	limbs[1] = subtract(p3d[2], p3d[1])    // r_shoulder
	limbs[2] = subtract(p3d[3], p3d[2])    // r_arm
	limbs[3] = subtract(p3d[4], p3d[3])    // r_forearm
	limbs[4] = subtract(p3d[5], p3d[1])    // l_shoulder
	limbs[5] = subtract(p3d[6], p3d[5])    // l_arm
	limbs[6] = subtract(p3d[7], p3d[6])    // l_forearm
	limbs[7] = subtract(p3d[1], p3d[8])    // spine
	limbs[8] = subtract(p3d[9], p3d[8])    // r_pelvis
	limbs[9] = subtract(p3d[10], p3d[9])   // r_thigh
	limbs[10] = subtract(p3d[11], p3d[10]) // r_shin
	limbs[11] = subtract(p3d[12], p3d[8])  // l_pelvis
	limbs[12] = subtract(p3d[13], p3d[12]) // l_thigh
	limbs[13] = subtract(p3d[14], p3d[13]) // l_shin
	return limbs
}

// Subtract calculates the element-wise subtraction of two slices
func subtract(a, b []float64) []float64 {
	result := make([]float64, len(a))
	for i := range a {
		result[i] = a[i] - b[i]
	}
	return result
}

// ScaleLimbs scales a human 3D skeleton to specified global and local scales
func ScaleLimbs(p3d [][]float64, globalScale float64, localScales []float64) [][]float64 {
	limbDependents := [][]int{
		{0},
		{2, 3, 4},
		{3, 4},
		{4},
		{5, 6, 7},
		{6, 7},
		{7},
		{0, 1, 2, 3, 4, 5, 6, 7},
		{9, 10, 11},
		{10, 11},
		{11},
		{12, 13, 14},
		{13, 14},
		{14},
	}

	limbs := GetLimbs(p3d)
	scaledLimbs := make([][]float64, 14)

	for i := 0; i < 14; i++ {
		scaledLimbs[i] = make([]float64, len(p3d[0]))
		copy(scaledLimbs[i], limbs[i])
	}

	for i := 0; i < 14; i++ {
		for j := 0; j < len(p3d[0]); j++ {
			scaledLimbs[i][j] *= globalScale
		}
	}
	scaledLimbs[0] = scaleVector(scaledLimbs[0], localScales[0])
	scaledLimbs[1] = scaleVector(scaledLimbs[1], localScales[1])
	scaledLimbs[2] = scaleVector(scaledLimbs[2], localScales[2])
	scaledLimbs[3] = scaleVector(scaledLimbs[3], localScales[3])
	scaledLimbs[4] = scaleVector(scaledLimbs[4], localScales[1])
	scaledLimbs[5] = scaleVector(scaledLimbs[5], localScales[2])
	scaledLimbs[6] = scaleVector(scaledLimbs[6], localScales[3])

	scaledLimbs[7] = scaleVector(scaledLimbs[7], localScales[4])

	scaledLimbs[8] = scaleVector(scaledLimbs[8], localScales[5])
	scaledLimbs[9] = scaleVector(scaledLimbs[9], localScales[6])
	scaledLimbs[10] = scaleVector(scaledLimbs[10], localScales[7])
	scaledLimbs[11] = scaleVector(scaledLimbs[11], localScales[5])
	scaledLimbs[12] = scaleVector(scaledLimbs[12], localScales[6])
	scaledLimbs[13] = scaleVector(scaledLimbs[13], localScales[7])

	delta := GetDelta(limbs, scaledLimbs)

	scaledP3D := make([][]float64, len(p3d))
	for i := range p3d {
		scaledP3D[i] = make([]float64, len(p3d[0]))
		copy(scaledP3D[i], p3d[i])
	}
	for _, indices := range limbDependents[7] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[7])
	}
	for _, indices := range limbDependents[1] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[1])
	}
	for _, indices := range limbDependents[4] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[4])
	}
	for _, indices := range limbDependents[2] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[2])
	}
	for _, indices := range limbDependents[5] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[5])
	}
	for _, indices := range limbDependents[3] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[3])
	}
	for _, indices := range limbDependents[6] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[6])
	}
	for _, indices := range limbDependents[0] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[0])
	}
	for _, indices := range limbDependents[8] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[8])
	}
	for _, indices := range limbDependents[11] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[11])
	}
	for _, indices := range limbDependents[9] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[9])
	}
	for _, indices := range limbDependents[12] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[12])
	}
	for _, indices := range limbDependents[10] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[10])
	}
	for _, indices := range limbDependents[13] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[13])
	}
	return scaledP3D
}

func scaleVector(vector []float64, scale float64) []float64 {
	scaledVector := make([]float64, len(vector))
	for i := range vector {
		scaledVector[i] = vector[i] * scale
	}
	return scaledVector
}

// GetDelta calculates the difference between two skeletons
func GetDelta(skeleton1, skeleton2 [][]float64) [][]float64 {
	delta := make([][]float64, len(skeleton1))

	for i := range skeleton1 {
		delta[i] = make([]float64, len(skeleton1[0]))
		for j := range skeleton1[0] {
			delta[i][j] = skeleton2[i][j] - skeleton1[i][j]
		}
	}

	return delta
}

func addVectors(a, b []float64) []float64 {
	result := make([]float64, len(a))
	for i := range a {
		result[i] = a[i] + b[i]
	}
	return result
}

// GetLimbLengths calculates limb lengths of a 3D human skeleton
func GetLimbLengths(p3d [][]float64) []float64 {
	limbs := GetLimbs(p3d)
	limbsNorm := make([]float64, 14)

	for i := 0; i < 14; i++ {
		limbsNorm[i] = math.Sqrt(limbs[i][0]*limbs[i][0] + limbs[i][1]*limbs[i][1] + limbs[i][2]*limbs[i][2])
	}

	limbLengths := []float64{
		limbsNorm[0],                         // neck
		(limbsNorm[1] + limbsNorm[4]) / 2.,   // shoulders
		(limbsNorm[2] + limbsNorm[5]) / 2.,   // arms
		(limbsNorm[3] + limbsNorm[6]) / 2.,   // forearms
		limbsNorm[7],                         // spine
		(limbsNorm[8] + limbsNorm[11]) / 2.,  // pelvis
		(limbsNorm[9] + limbsNorm[12]) / 2.,  // thighs
		(limbsNorm[10] + limbsNorm[13]) / 2., // shins
	}

	return limbLengths
}

// UnitLimbNorm scales a human 3D skeleton to be of unit limb lengths
func UnitLimbNorm(p3d [][]float64) [][]float64 {
	unitLimbLengths := make([]float64, 8)
	for i := range unitLimbLengths {
		unitLimbLengths[i] = 1.0
	}

	srcLimbLengths := GetLimbLengths(p3d)

	for i := range srcLimbLengths {
		if srcLimbLengths[i] < 1e-3 {
			srcLimbLengths[i] = 1e-3
		}
	}

	localScales := make([]float64, 8)

	for i := range localScales {
		localScales[i] = unitLimbLengths[i] / srcLimbLengths[i]
	}
	scaledP3d := ScaleLimbs(p3d, 1.0, localScales)
	return scaledP3d
}

// CalcLimbLengths calculates limb lengths for a sequence of 3D human skeletons
func CalcLimbLengths(p3dSeq [][][]float64) [][]float64 {
	limbLengths := make([][]float64, len(p3dSeq))

	for i, p3d := range p3dSeq {
		limbLengths[i] = make([]float64, 8)
		limbs := GetLimbs(p3d)
		for j := 0; j < 8; j++ {
			limbLengths[i][j] = math.Sqrt(limbs[j][0]*limbs[j][0] + limbs[j][1]*limbs[j][1] + limbs[j][2]*limbs[j][2])
		}
	}

	return limbLengths
}

// NormalizeSeqPose3D normalizes a sequence of 3D human skeletons to be of unit limb lengths
func NormalizeSeqPose3D(p3dSeq [][][]float64) [][][]float64 {
	newP3DSeq := make([][][]float64, len(p3dSeq))
	for i, p3d := range p3dSeq {
		newP3DSeq[i] = UnitLimbNorm(p3d)
	}
	return newP3DSeq
}

func BuildTemplate(frames [][][]float64) (template.Template, error) {
	normP3d := NormalizeSeqPose3D(frames)
	template := template.Template{
		P3d:     frames,
		NormP3d: normP3d,
	}
	return template, nil
}
