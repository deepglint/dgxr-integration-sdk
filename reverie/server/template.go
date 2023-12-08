package server

import (
	"encoding/json"
	"net/http"

	"reverie/global"
	"reverie/model"
	"reverie/model/template"

	"github.com/gin-gonic/gin"
	"github.com/sirupsen/logrus"
	log "github.com/sirupsen/logrus"
)

func InitTemplate(r *gin.Engine) {
	system := r.Group("template")
	system.POST("", AddTemplate)
}

func AddTemplate(c *gin.Context) {
	t := new(template.AddTemplateReq)
	if err := c.ShouldBindJSON(t); err != nil {
		log.Error(err)
		c.JSON(http.StatusBadRequest, gin.H{
			"msg": err.Error(),
		})
		return
	}
	if err := BuildTemplate(t); err != nil {
		logrus.Error(err)
		c.JSON(http.StatusBadRequest, gin.H{
			"msg": err.Error(),
		})
		return
	}
	c.JSON(http.StatusCreated, nil)
}

func BuildTemplate(tem *template.AddTemplateReq) error {
	data, err := global.BuildTemplate(tem.POS)
	if err != nil {
		return err
	}
	jsonData, _ := json.Marshal(data)
	jsonKeyNode, _ := json.Marshal(tem.KeyNode)
	t := model.Template{
		BaseModel: model.BaseModel{
			Id: tem.Id,
		},
		SingleFrame: *tem.SingleFrame,
		Name:        tem.Name,
		Pos:         string(jsonData),
		Score:       tem.Score,
		PathLen:     tem.PathLen,
		KeyNode:     string(jsonKeyNode),
	}
	template := template.TemplateData{
		Id:          tem.Id,
		SingleFrame: *tem.SingleFrame,
		KeyNode:     tem.KeyNode,
		Name:        tem.Name,
		Tem:         data,
		Score:       tem.Score,
		PathLen:     tem.PathLen,
	}
	if err := t.AddTemplate(global.DbEngine); err != nil {
		return err
	}
	global.UpdateTemplateMap(tem.Id, &template)
	return nil
}
