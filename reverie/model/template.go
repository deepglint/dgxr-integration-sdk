package model

import (
	"errors"

	"xorm.io/xorm"
)

type Template struct {
	BaseModel   `xorm:"extends"`
	SingleFrame int     `json:"singleFrame" xorm:"bit 'single_frame'"`
	Name        string  `json:"name" xorm:"varchar(36) 'name'"`
	Pos         string  `json:"pos" xorm:"text 'pos'"`
	Score       float32 `json:"score" xorm:"float 'score'"`
	KeyNode     string  `json:"keyNode" xorm:"text 'key_node'"`
	PathLen     float32 `json:"pathLen" xorm:"float 'path_len'"`
}

func (t *Template) TableName() string {
	return "template"
}

// AddTemplate adds a new template record to the database
func (t *Template) AddTemplate(engine *xorm.Engine) error {
	_, err := engine.Insert(t)
	if err != nil {
		return err
	}
	return nil
}

// GetTemplatesByIDArray returns n template records from the database based on an array of ids
func GetTemplatesByIDArray(engine *xorm.Engine, ids []int) ([]*Template, error) {
	if len(ids) == 0 {
		return nil, errors.New("empty id array")
	}
	templates := make([]*Template, 0)
	err := engine.In("id", ids).Find(&templates)
	if err != nil {
		return nil, err
	}
	return templates, nil
}

func FindTemplates(engine *xorm.Engine) ([]*Template, error) {
	templates := make([]*Template, 0)
	err := engine.Find(&templates)
	if err != nil {
		return nil, err
	}
	return templates, nil
}
