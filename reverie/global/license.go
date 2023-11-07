package global

import (
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"reverie/util"
	"time"

	"github.com/sirupsen/logrus"
	"github.com/spf13/viper"
)

func CheckLicense() {
	if err := GetLicenseStatus(); err != nil {
		LicenseStatus = false
		logrus.Errorf("get license status error: %v", err)
	}

	ticker := time.NewTicker(10 * time.Minute)
	for {
		select {
		case <-ticker.C:
			if err := GetLicenseStatus(); err != nil {
				LicenseStatus = false
				logrus.Errorf("get license status error: %v", err)
			}
		}
	}
}

func GetLicenseStatus() error {
	endpoint := fmt.Sprintf("%s:%s", viper.GetString("license.host"), viper.GetString("license.port"))
	req, err := http.NewRequest("GET", endpoint+LicenseTimeApi, nil)
	if err != nil {
		return err
	}
	req.Header.Set("Authorization", "e524ec5951a68da5")
	client := &http.Client{}
	resp, err := client.Do(req)
	if err != nil {
		return err
	}
	defer resp.Body.Close()
	respData, err := io.ReadAll(resp.Body)
	if err != nil {
		return err
	}
	rep := &util.LicenseRep{}
	if err = json.Unmarshal(respData, rep); err != nil {
		return err
	}
	if len(rep.Details) > 0 && len(rep.Details[0].Features) > 0 {
		for _, v := range rep.Details[0].Features {
			if v.FeatureId == 11 && v.FeatureMap["3600"] == "1" {
				LicenseStatus = true
				return nil
			}
		}
	}
	LicenseStatus = false
	return nil
}
