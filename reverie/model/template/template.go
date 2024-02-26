package template

type AddTemplateReq struct {
	Id          int           `json:"id" binding:"required"`
	SingleFrame *int          `json:"singleFrame" binding:"required"`
	KeyNode     []int         `json:"keyNode" binding:"required"`
	Name        string        `json:"name" binding:"required"`
	POS         [][][]float64 `json:"pos" binding:"required"`
	Score       float32       `json:"score"`
	PathLen     float32       `json:"pathLen"`
}

type Template struct {
	P3d     [][][]float64 `json:"p3d"`
	NormP3d [][][]float64 `json:"norm_p3d"`
}

type TemplateData struct {
	Id          int      `json:"id"`
	Name        string   `json:"name"`
	SingleFrame int      `json:"singleFrame"`
	Tem         Template `json:"pos"`
	KeyNode     []int    `json:"keyNode"`
	Score       float32  `json:"score"`
	PathLen     float32  `json:"pathLen"`
}
