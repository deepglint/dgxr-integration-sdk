package global

import (
	"meta/model/config"
	"meta/model/source"
)

var (
	Config   *config.Config
	Sources  map[string]*source.Source
	PersonID string
)

func InitSources() {
	Sources = make(map[string]*source.Source, 0)
}
