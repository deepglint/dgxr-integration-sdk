package global

import (
	"math"

	"gonum.org/v1/gonum/mat"
)

// KabschWeighted 计算带权重的Kabsch算法
func KabschWeighted(P, Q *mat.Dense, W *mat.Dense) (*mat.Dense, *mat.VecDense, float64) {
	rows, cols := P.Dims()
	d := cols

	CMP := mat.NewVecDense(3, nil)
	CMQ := mat.NewVecDense(3, nil)
	C := mat.NewDense(d, d, nil)

	if W == nil {
		W = mat.NewDense(rows, d, nil)
		for i := 0; i < rows; i++ {
			for j := 0; j < d; j++ {
				W.Set(i, j, 1.0/float64(rows))
			}
		}
	}

	Wmat := mat.NewDense(rows, d, nil)
	for i := 0; i < d; i++ {
		for j := 0; j < rows; j++ {
			Wmat.Set(j, i, W.At(j, i))
		}
	}

	iw := 3.0 / mat.Sum(Wmat)

	for i := 0; i < d; i++ {
		for j := 0; j < rows; j++ {
			for k := 0; k < d; k++ {
				C.Set(i, k, C.At(i, k)+(P.At(j, i)*Q.At(j, k)*W.At(j, i)))
			}
		}
	}
	PW := mat.NewDense(rows, d, nil)
	QW := mat.NewDense(rows, d, nil)
	for j := 0; j < cols; j++ {
		// 获取 P 和 W 矩阵的列视图
		colViewP := P.ColView(j)
		colViewW := W.ColView(j)
		// 初始化一个临时向量用于存储元素相乘的结果
		tmp := mat.NewVecDense(rows, nil)
		// 手动计算元素相乘
		for i := 0; i < rows; i++ {
			tmp.SetVec(i, colViewP.At(i, 0)*colViewW.At(i, 0))
		}
		// 计算元素相乘后的和并设置给 CMP
		CMP.SetVec(j, mat.Sum(tmp))
	}
	for j := 0; j < cols; j++ {
		colViewQ := Q.ColView(j)
		colViewW := W.ColView(j)
		tmp := mat.NewVecDense(rows, nil)
		for i := 0; i < rows; i++ {
			tmp.SetVec(i, colViewQ.At(i, 0)*colViewW.At(i, 0))
		}
		CMQ.SetVec(j, mat.Sum(tmp))
	}
	PW.MulElem(P, P)
	PW.MulElem(PW, W)

	QW.MulElem(Q, Q)
	QW.MulElem(QW, W)

	PSQ := mat.Sum(PW) - mat.Dot(CMP, CMP)
	QSQ := mat.Sum(QW) - mat.Dot(CMQ, CMQ)

	outerProduct := mat.NewDense(d, d, nil)
	outerProduct.Mul(CMP, CMQ.T())
	result := mat.NewDense(d, d, nil)
	result.Scale(iw, outerProduct)
	C.Sub(C, result)

	var svd mat.SVD
	ok := svd.Factorize(C, mat.SVDThin)
	if !ok {
		return nil, nil, 0.0
	}
	var V, WW mat.Dense
	var S mat.VecDense
	svd.UTo(&V)
	svd.VTo(&WW)
	S = *mat.NewVecDense(len(svd.Values(nil)), svd.Values(nil))
	v := cloneAsMatrix(&V)
	detV := mat.Det(v)
	w := cloneAsMatrix(&WW)
	detW := mat.Det(w)
	if detV*detW < 0 {
		sliceS := S.RawVector().Data
		sliceS[len(sliceS)-1] = -sliceS[len(sliceS)-1]
		S.SetVec(len(sliceS)-1, -S.AtVec(len(sliceS)-1))
		rows, cols := V.Dims()
		vSlice := make([]float64, rows)
		for i := 0; i < rows; i++ {
			vSlice[i] = V.At(i, cols-1)
		}

		for i := 0; i < rows; i++ {
			vSlice[i] = -vSlice[i]
		}
		for i := 0; i < rows; i++ {
			V.Set(i, cols-1, vSlice[i])
		}
	}

	var msd float64
	var sumS float64
	for i := 0; i < S.Len(); i++ {
		sumS += S.AtVec(i)
	}
	msd = (PSQ+QSQ)*iw - 2.0*sumS
	if msd < 0.0 {
		msd = 0.0
	}
	rmsd := math.Sqrt(msd)

	return nil, nil, rmsd
}

func cloneAsMatrix(m *mat.Dense) mat.Matrix {
	r, c := m.Dims()
	clone := mat.NewDense(r, c, nil)
	clone.Copy(m)
	return clone
}

// KabschWeightedFit 使用带权重的Kabsch算法将P拟合到Q
func KabschWeightedFit(P, Q *mat.Dense, W *mat.Dense) (*mat.Dense, float64) {
	var rmsd float64
	_, _, rmsd = KabschWeighted(Q, P, W)
	//
	return nil, rmsd
}

// ComputeDistMatrix 计算两个3D点序列之间的距离矩阵
func ComputeDistMatrix(srcP3DSeq, dstP3DSeq [][][]float64) *mat.Dense {
	rows := len(srcP3DSeq)
	cols := len(dstP3DSeq)
	distMat := mat.NewDense(rows, cols, nil)
	for i := 0; i < rows; i++ {
		for j := 0; j < cols; j++ {
			srcP3dMat := mat.NewDense(len(srcP3DSeq[i]), 3, nil)
			dstP3dMat := mat.NewDense(len(dstP3DSeq[j]), 3, nil)
			for k, v := range srcP3DSeq[i] {
				tmp := []float64{}
				tmp = append(tmp, v...)
				srcP3dMat.SetRow(k, tmp)
			}
			for k, v := range dstP3DSeq[j] {
				tmp := []float64{}
				tmp = append(tmp, v...)
				dstP3dMat.SetRow(k, tmp)
			}
			_, v := KabschWeightedFit(srcP3dMat, dstP3dMat, nil)
			distMat.Set(i, j, v)
		}
	}

	return distMat
}

func ComputeAccumulatedCostMatrix(C *mat.Dense) *mat.Dense {
	N, M := C.Dims()
	D := mat.NewDense(N, M, nil)

	var sum float64
	for i := 0; i < N; i++ {
		val := C.At(i, 0)
		sum += val
		D.Set(i, 0, sum)
	}

	for m := 0; m < M; m++ {
		D.Set(0, m, C.At(0, m))
	}

	for n := 1; n < N; n++ {
		for m := 1; m < M; m++ {
			val := C.At(n, m) + min(D.At(n-1, m-1), D.At(n-1, m), D.At(n, m-1))
			D.Set(n, m, val)
		}
	}

	return D
}

func FindBestPath(accumulatedDistMat *mat.Dense, distMat *mat.Dense) ([][2]int, int, float64) {
	bestPath := computeOptimalWarpingPathSubsequenceDTW(accumulatedDistMat)
	pathScore := computePathScore(bestPath, distMat)
	pathLen := bestPath[len(bestPath)-1][1] - bestPath[0][1] + 1
	return bestPath, pathLen, pathScore
}

func computeOptimalWarpingPathSubsequenceDTW(D *mat.Dense) [][2]int {
	N, M := D.Dims()
	n := N - 1
	m := -1
	if m < 0 {
		// 如果 m 小于 0，则设置 m 为最后一行的最小值所在的列
		minVal := D.At(N-1, 0)
		for i := 1; i < M; i++ {
			if val := D.At(N-1, i); val < minVal {
				minVal = val
				m = i
			}
		}
	}

	P := [][2]int{{n, m}}

	for n > 0 {
		var cell [2]int

		if m == 0 {
			cell = [2]int{n - 1, 0}
		} else {
			val := min(D.At(n-1, m-1), D.At(n-1, m), D.At(n, m-1))

			if val == D.At(n-1, m-1) {
				cell = [2]int{n - 1, m - 1}
			} else if val == D.At(n-1, m) {
				cell = [2]int{n - 1, m}
			} else {
				cell = [2]int{n, m - 1}
			}
		}

		P = append(P, cell)
		n, m = cell[0], cell[1]
	}

	// 反转切片
	for i, j := 0, len(P)-1; i < j; i, j = i+1, j-1 {
		P[i], P[j] = P[j], P[i]
	}

	return P
}

func computePathScore(path [][2]int, distMatrix *mat.Dense) float64 {
	var scores []float64
	for _, p := range path {
		scores = append(scores, distMatrix.At(p[0], p[1]))
	}
	return mean(scores)
}

func argMin(vec mat.Vector) int {
	_, c := vec.Dims()
	minIndex := 0
	minValue := vec.At(0, 0)

	for i := 1; i < c; i++ {
		value := vec.At(0, i)
		if value < minValue {
			minValue = value
			minIndex = i
		}
	}

	return minIndex
}

func min(a, b, c float64) float64 {
	if a < b {
		if a < c {
			return a
		} else {
			return c
		}
	} else {
		if b < c {
			return b
		} else {
			return c
		}
	}
}

func mean(slice []float64) float64 {
	var sum float64
	for _, v := range slice {
		sum += v
	}
	return sum / float64(len(slice))
}

func reversePath(path [][2]int) {
	for i, j := 0, len(path)-1; i < j; i, j = i+1, j-1 {
		path[i], path[j] = path[j], path[i]
	}
}
