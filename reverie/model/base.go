package model

import "time"

type BaseModel struct {
	Id        int       `json:"id" xorm:"id"`              // 主键ID
	CreatedAt time.Time `json:"createdAt"  xorm:"created"` // 创建时间
	UpdatedAt time.Time `json:"updatedAt"  xorm:"updated"` // 更新时间
}
