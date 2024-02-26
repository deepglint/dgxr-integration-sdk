package global

import (
	"math"
	"reverie/model/template"
)

// GetLimbs calculates limb vectors of a 3D human skeleton
func GetLimbs(p3d [][]float64) [][]float64 {
	D := len(p3d[0])
	limbs := make([][]float64, 20)

	for i := 0; i < 20; i++ {
		limbs[i] = make([]float64, D)
	}
	neck := averageKeyPoints(p3d[5], p3d[6])
	root := averageKeyPoints(p3d[11], p3d[12])

	limbs[0] = subtract(p3d[21], neck)    // neck > head, 0
	limbs[1] = subtract(p3d[6], neck)     // neck > r_shoulder, 1
	limbs[2] = subtract(p3d[5], neck)     // neck > l_shoulder, 2
	limbs[3] = subtract(p3d[8], p3d[6])   // r_shoulder > r_elbow, 3
	limbs[4] = subtract(p3d[7], p3d[5])   // l_shoulder > l_elbow, 4
	limbs[5] = subtract(p3d[10], p3d[8])  // r_elbow > r_wrist, 5
	limbs[6] = subtract(p3d[9], p3d[7])   // l_elbow > l_wrist, 6
	limbs[7] = subtract(p3d[23], p3d[10]) // r_wrist > r_hand, 7
	limbs[8] = subtract(p3d[22], p3d[9])  // l_wrist > l_hand, 8

	limbs[9] = subtract(neck, root) // root > neck, 9

	limbs[10] = subtract(p3d[12], root)    // root > r_hip, 10
	limbs[11] = subtract(p3d[11], root)    // root > l_hip, 11
	limbs[12] = subtract(p3d[14], p3d[12]) // r_hip > r_knee, 12
	limbs[13] = subtract(p3d[13], p3d[11]) // l_hip > l_knee, 13
	limbs[14] = subtract(p3d[16], p3d[14]) // r_knee > r_ankle, 14
	limbs[15] = subtract(p3d[15], p3d[13]) // l_knee > l_ankle, 15
	limbs[16] = subtract(p3d[18], p3d[16]) // r_ankle > r_tiptoe, 16
	limbs[17] = subtract(p3d[17], p3d[15]) // l_ankle > l_tiptoe, 17
	limbs[18] = subtract(p3d[20], p3d[16]) // r_ankle > r_heel, 18
	limbs[19] = subtract(p3d[19], p3d[15]) // l_ankle > l_heel, 19
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
		{6, 8, 10, 23},
		{5, 7, 9, 22},
		{8, 10, 23},
		{7, 9, 22},
		{10, 23},
		{7, 9},
		{10},
		{7},
		{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 22, 23},
		{12, 14, 16, 18, 20},
		{11, 13, 15, 17, 19},
		{14, 16, 18, 20},
		{23, 15, 17, 19},
		{16, 18, 20},
		{15, 17, 19},
		{18},
		{17},
		{20},
		{19},
	}

	limbs := GetLimbs(p3d)
	scaledLimbs := make([][]float64, len(limbs))

	for i := 0; i < len(limbs); i++ {
		scaledLimbs[i] = make([]float64, len(p3d[0]))
		copy(scaledLimbs[i], limbs[i])
		for j := 0; j < len(p3d[0]); j++ {
			scaledLimbs[i][j] *= globalScale
		}
	}

	scaledLimbs[0] = scaleVector(scaledLimbs[0], localScales[0])

	scaledLimbs[1] = scaleVector(scaledLimbs[1], localScales[1])
	scaledLimbs[2] = scaleVector(scaledLimbs[2], localScales[1])
	scaledLimbs[3] = scaleVector(scaledLimbs[3], localScales[2])
	scaledLimbs[4] = scaleVector(scaledLimbs[4], localScales[2])
	scaledLimbs[5] = scaleVector(scaledLimbs[5], localScales[3])
	scaledLimbs[6] = scaleVector(scaledLimbs[6], localScales[3])
	scaledLimbs[7] = scaleVector(scaledLimbs[7], localScales[4])
	scaledLimbs[8] = scaleVector(scaledLimbs[8], localScales[4])

	scaledLimbs[9] = scaleVector(scaledLimbs[9], localScales[5])

	scaledLimbs[10] = scaleVector(scaledLimbs[10], localScales[6])
	scaledLimbs[11] = scaleVector(scaledLimbs[11], localScales[6])
	scaledLimbs[12] = scaleVector(scaledLimbs[12], localScales[7])
	scaledLimbs[13] = scaleVector(scaledLimbs[13], localScales[7])
	scaledLimbs[14] = scaleVector(scaledLimbs[14], localScales[8])
	scaledLimbs[15] = scaleVector(scaledLimbs[15], localScales[8])
	scaledLimbs[16] = scaleVector(scaledLimbs[16], localScales[9])
	scaledLimbs[17] = scaleVector(scaledLimbs[17], localScales[9])

	delta := GetDelta(limbs, scaledLimbs)

	scaledP3D := make([][]float64, len(p3d))
	for i := range p3d {
		scaledP3D[i] = make([]float64, len(p3d[0]))
		copy(scaledP3D[i], p3d[i])
	}

	for _, indices := range limbDependents[9] {
		scaledP3D[indices] = addVectors(scaledP3D[indices], delta[9])
	}
	for i := 1; i < 20; i++ {
		for _, indices := range limbDependents[i] {
			scaledP3D[indices] = addVectors(scaledP3D[indices], delta[i])
		}
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
	limbsNorm := make([]float64, 20)

	for i := 0; i < 20; i++ {
		limbsNorm[i] = math.Sqrt(limbs[i][0]*limbs[i][0] + limbs[i][1]*limbs[i][1] + limbs[i][2]*limbs[i][2])
	}

	limbLengths := []float64{
		limbsNorm[0],                         // neck - head
		(limbsNorm[1] + limbsNorm[2]) / 2.,   // neck - shoulders
		(limbsNorm[3] + limbsNorm[4]) / 2.,   // shoulders - elbow
		(limbsNorm[5] + limbsNorm[6]) / 2.,   // elbow - wrist
		(limbsNorm[7] + limbsNorm[8]) / 2.,   // wrist - hand
		limbsNorm[9],                         // root - neck
		(limbsNorm[10] + limbsNorm[11]) / 2., // root - hip
		(limbsNorm[12] + limbsNorm[13]) / 2., // hip - knee
		(limbsNorm[14] + limbsNorm[15]) / 2., // knee - ankle
		(limbsNorm[16] + limbsNorm[17]) / 2., // ankle - tiptoe
		(limbsNorm[18] + limbsNorm[19]) / 2., // ankle - heel
	}

	return limbLengths
}

// UnitLimbNorm scales a human 3D skeleton to be of unit limb lengths
func UnitLimbNorm(p3d [][]float64) [][]float64 {
	unitLimbLengths := make([]float64, 11)
	for i := range unitLimbLengths {
		unitLimbLengths[i] = 1.0
	}

	srcLimbLengths := GetLimbLengths(p3d)

	for i := range srcLimbLengths {
		if srcLimbLengths[i] < 1e-3 {
			srcLimbLengths[i] = 1e-3
		}
	}

	localScales := make([]float64, 11)

	for i := range localScales {
		localScales[i] = unitLimbLengths[i] / srcLimbLengths[i]
	}
	scaledP3d := ScaleLimbs(p3d, 1.0, localScales)
	return scaledP3d
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

func ConvertKeyPoints(keyPoints [][]float64) [][]float64 {
	newKeyPoints := make([][]float64, 15)
	for i := range newKeyPoints {
		newKeyPoints[i] = make([]float64, len(keyPoints[0]))
	}

	newKeyPoints[8] = averageKeyPoints(newKeyPoints[11], newKeyPoints[12])
	newKeyPoints[1] = averageKeyPoints(newKeyPoints[5], newKeyPoints[6])

	indexMapping := []int{0, 2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14}
	originalIndices := []int{0, 6, 8, 10, 5, 7, 9, 12, 14, 16, 11, 13, 15}
	for i, j := range indexMapping {
		newKeyPoints[j] = keyPoints[originalIndices[i]]
	}

	return newKeyPoints
}

func averageKeyPoints(keyPoint1, keyPoint2 []float64) []float64 {
	result := make([]float64, len(keyPoint1))
	for i := range result {
		result[i] = (keyPoint1[i] + keyPoint2[i]) / 2
	}
	return result
}
