package source

import (
	"fmt"
	"math"
	"reverie/source/output/xbox"
	"sync"
)

// source
/*
- 1.帧结构
- 2.source结构
- 3.初始化source结构体
- 4.source的先进先出逻辑
- 5.获取source最后一个值逻辑
- 6.计算source的平均值
*/

type JointType int

const (
	Nose JointType = iota
	LeftEye
	RightEye
	LeftEar
	RightEar
	LeftShoulder
	RightShoulder
	LeftElbow
	RightElbow
	LeftWrist
	RightWrist
	LeftHip
	RightHip
	LeftKnee
	RightKnee
	LeftAnkle
	RightAnkle
	LeftTiptoe
	RightTiptoe
	LeftHeel
	RightHeel
	HeadTop
	LeftHand
	RightHand
)

type Point struct {
	X, Y, Z float64
}

type SourceData struct {
	Objs [][]float64
}

// Window 维护一个滑动窗口
type Window struct {
	mu      sync.Mutex
	data    []int
	size    int
	index   int
	counter map[int]int
}

func NewWindow(size int) *Window {
	return &Window{
		data:    make([]int, size),
		size:    size,
		index:   0,
		counter: make(map[int]int),
	}
}

func (w *Window) Add(value int) {
	w.mu.Lock()
	defer w.mu.Unlock()
	oldValue := w.data[w.index]
	w.counter[oldValue]--
	w.data[w.index] = value
	w.counter[value]++
	w.index = (w.index + 1) % w.size
}

func (w *Window) MaxCount() int {
	w.mu.Lock()
	defer w.mu.Unlock()
	maxIndex := 0
	max := -1
	for k, v := range w.counter {
		if k == 0 {
			continue
		}
		if v > max && v > w.size/4 {
			max = v
			maxIndex = k
		}
	}
	return maxIndex
}

// TODO source结构体单独定义，与grpc数据解耦
type Source struct {
	items        []SourceData
	PersonId     string
	ActionWindow *Window
	cap          int
	Xbox         *xbox.Xbox
	mutex        sync.RWMutex // 读写锁
}

func InitSource(cap int) *Source {
	win := NewWindow(20)
	return &Source{
		items:        []SourceData{},
		mutex:        sync.RWMutex{},
		ActionWindow: win,
		cap:          cap,
	}
}

// 存放source数据
func (q *Source) Enqueue(item SourceData) {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	if len(q.items) >= q.cap {
		q.items = q.items[1:]
	}
	q.items = append(q.items, item)
}

// 取出source第一个数据（注意队列的先进先出）
func (q *Source) Dequeue() (SourceData, error) {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	if len(q.items) == 0 {
		return SourceData{}, fmt.Errorf("Source is empty")
	}
	item := q.items[0]
	q.items = q.items[1:]
	return item, nil
}

// 取出source所有数据
func (q *Source) All() []SourceData {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	return q.items
}

// 获取数据源的大小
func (q *Source) Size() int {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	return len(q.items)
}

// 取出source第最后一个数据（注意队列的先进先出）
func (q *Source) LastData() (SourceData, error) {
	// q.mutex.Lock()
	// defer q.mutex.Unlock()
	if len(q.items) == 0 {
		return SourceData{}, fmt.Errorf("Source is empty")
	}
	item := q.items[q.Size()-1]
	return item, nil
}

// 返回数据源中某个点的x，y，z三点的数组
func (q *Source) XYZArrValues(a int) (X, Y, Z []float64) {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	for i := q.Size() - 1; i >= 0; i-- {
		X = append(X, q.items[i].Objs[a][0])
		Y = append(Y, q.items[i].Objs[a][1])
		Z = append(Z, q.items[i].Objs[a][2])
	}
	return
}

// 获取指定关节点的X的平均值
func (q *Source) AverageAX(a int) float64 {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	var sum float64
	for _, value := range q.items {
		sum += float64(value.Objs[a][0])
	}
	return sum / float64(len(q.items))
}

// 获取指定关节点的Y的平均值
func (q *Source) AverageAY(a int) float64 {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	var sum float64
	for _, value := range q.items {
		sum += float64(value.Objs[a][1])
	}
	return sum / float64(len(q.items))
}

// 获取指定关节点的Z的平均值
func (q *Source) AverageAZ(a int) float64 {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	var sum float64
	for _, value := range q.items {
		sum += float64(value.Objs[a][2])
	}
	return sum / float64(len(q.items))
}

// 计算两个骨骼点的中心点A的x坐标加y坐标的平均值
func (q *Source) CalculateAverageAXY(a, b int) float64 {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	var sum float64
	for _, value := range q.items {
		center := q.CalculateCenterPoint(value.Objs[a], value.Objs[b])
		sum += center[0] + center[1]
	}
	return sum / float64(len(q.items))
}

// 计算两个骨骼点的中心点A的x坐标加y坐标加z坐标的平均值
func (q *Source) CalculateAverageA(a, b int) float64 {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	var sum float64
	for _, value := range q.items {
		center := q.CalculateCenterPoint(value.Objs[a], value.Objs[b])
		sum += center[0] + center[1] + center[2]
	}
	return sum / float64(len(q.items))
}

// 计算两个点之间的中心点
func (q *Source) CalculateCenterPoint(a, b []float64) []float64 {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	return []float64{
		(float64(a[0]) + float64(b[0])) / 2,
		(float64(a[1]) + float64(b[1])) / 2,
		(float64(a[2]) + float64(b[2])) / 2,
	}
}

// 返回任意两个骨骼关键点旋转角度
func (q *Source) CalculateAverageAB(a, b int) float64 {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	var sum float64
	for _, value := range q.items {
		sum += q.CalculateRotateAngle(value.Objs[a], value.Objs[b])
	}
	return sum / float64(len(q.items))
}

// 计算线段的旋转角度
func (q *Source) CalculateRotateAngle(a, b []float64) float64 {
	// 计算线段AB的方向向量
	X := float64(b[0] - a[0])
	Y := float64(b[1] - a[1])
	Z := float64(b[2] - a[2])

	// 计算线段AB的长度
	lengthAB := math.Sqrt(math.Pow(X, 2) + math.Pow(Y, 2) + math.Pow(Z, 2))

	// 将方向向量AB标准化
	unitVector := Point{
		X: X / lengthAB,
		Y: Y / lengthAB,
		Z: Z / lengthAB,
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

// 计算ABC三点的选择角度的均值
func (q *Source) CalculateAverageABC(a, b, c int) float64 {
	q.mutex.Lock()
	defer q.mutex.Unlock()
	var sum float64
	for _, value := range q.items {
		sum += q.CalculateAngle(value.Objs[a], value.Objs[b], value.Objs[c])
	}
	return sum / float64(len(q.items))
}

// CalculateAngle 计算三点之间的夹角
func (q *Source) CalculateAngle(a, b, c []float64) float64 {
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
