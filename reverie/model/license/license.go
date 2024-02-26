package license

var ()

type LicenseInfo struct {
	Status    string            `json:"status"`
	ExpireAt  int64             `json:"expire_ts"`
	Features  map[string]string `json:"features"`
	FeatureId []string          `json:"feature_id"`
}

type LicenseStatus string

const (
	Authorized   LicenseStatus = "authorized"
	Unauthorized LicenseStatus = "unauthorized"
)

var (
	GameToFeatureID = map[string]string{
		"DG_CHICKEN":     "3600",
		"DG_MILLIONAIRE": "3601",
		"DG_DUMP":        "3602",
	}
)

type LicenseInfoRep struct {
	Status LicenseStatus `json:"status"`
	Expire string        `json:"expire"`
}

type LicenseGameReq struct {
	Game string `json:"game"`
}
