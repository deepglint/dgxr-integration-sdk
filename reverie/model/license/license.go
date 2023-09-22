package license

type LicenseInfo struct {
	Status   string `json:"status"`
	ExpireAt int64  `json:"expire_ts"`
}

type LicenseStatus string

const (
	Authorized   LicenseStatus = "authorized"
	Unauthorized LicenseStatus = "unauthorized"
)

type LicenseInfoRep struct {
	Status LicenseStatus `json:"status"`
	Expire string        `json:"expire"`
}
