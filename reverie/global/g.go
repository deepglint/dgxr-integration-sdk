package global

import (
	"reverie/model/config"
	"reverie/model/source"
)

const (
	LicenseFileApi       = "/x-api/v1/license_nodes/authorize/download"
	LicenseTimeApi       = "/x-api/v1/license_nodes/authenticate/licenses"
	LicenseFileUploadApi = "/x-api/v1/license_nodes/authorize/upload_v2c"
)

var (
	Config        *config.Config
	Sources       map[string]*source.Source
	PersonID      string
	LicenseStatus bool
)

func InitSources() {
	Sources = make(map[string]*source.Source, 0)
}
