package global

import (
	"encoding/json"
	"reverie/model"
	"reverie/model/config"
	"reverie/model/template"
	"sync"

	"github.com/sirupsen/logrus"
	"xorm.io/xorm"
)

const (
	LicenseFileApi       = "/x-api/v1/license_nodes/authorize/download"
	LicenseTimeApi       = "/x-api/v1/license_nodes/authenticate/licenses"
	LicenseFileUploadApi = "/x-api/v1/license_nodes/authorize/upload_v2c"
)

var (
	Rw       sync.RWMutex
	Config   *config.Config
	DbEngine *xorm.Engine
	// Sources       map[string]*source.Source
	Sources       sync.Map
	PersonID      string
	LicenseStatus bool
	// TODO 模板测试完成后去掉action 中的所有代码，迁移到 global 中做一个 action 方法
	TemplateMap map[int]*template.TemplateData
)

func UpdateTemplateMap(id int, t *template.TemplateData) {
	Rw.Lock()
	defer Rw.Unlock()
	if TemplateMap == nil {
		TemplateMap = make(map[int]*template.TemplateData, 0)
	}
	TemplateMap[id] = t
}

func GetTemplatesByID(id int) (*template.TemplateData, bool) {
	Rw.RLock()
	defer Rw.RUnlock()
	if val, ok := TemplateMap[id]; ok {
		return val, true
	}
	return nil, false
}

func GetTemplates() map[int]*template.TemplateData {
	Rw.RLock()
	defer Rw.RUnlock()
	return TemplateMap
}

func UpdateTemplate() {
	t, err := model.FindTemplates(DbEngine)
	if err != nil {
		panic(err)
	}
	for _, v := range t {
		tem := template.Template{}
		keyNode := []int{}
		if err := json.Unmarshal([]byte(v.Pos), &tem); err != nil {
			logrus.Error(err)
			continue
		}

		if err := json.Unmarshal([]byte(v.KeyNode), &keyNode); err != nil {
			logrus.Error(err)
			continue
		}
		data := template.TemplateData{
			Id:          v.Id,
			SingleFrame: v.SingleFrame,
			KeyNode:     keyNode,
			Name:        v.Name,
			Tem:         tem,
			Score:       v.Score,
			PathLen:     v.PathLen,
		}
		UpdateTemplateMap(v.Id, &data)
	}
}
