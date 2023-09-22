package util

import (
	"bytes"
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"mime/multipart"
	"net/http"
	"os"
	"path/filepath"
	"time"

	"reverie/global"
	"reverie/model/license"

	"github.com/sirupsen/logrus"
	"github.com/spf13/viper"
)

func createReqBody(filePath string) (string, io.Reader, error) {
	var err error
	buf := new(bytes.Buffer)
	bw := multipart.NewWriter(buf)
	f, err := os.Open(filePath)
	if err != nil {
		return "", nil, err
	}
	defer f.Close()

	_, fileName := filepath.Split(filePath)
	fw1, _ := bw.CreateFormFile("bigtoe", fileName)
	_, _ = io.Copy(fw1, f)
	bw.Close()
	return bw.FormDataContentType(), buf, nil
}

func DoUploadLicense(addr, filePath string) error {
	contType, reader, err := createReqBody(filePath)
	if err != nil {
		return err
	}
	req, err := http.NewRequest("POST", addr, reader)
	if err != nil {
		return err
	}
	req.Header.Add("Content-Type", contType)

	client := &http.Client{}
	resp, err := client.Do(req)
	if err != nil {
		return err
	}
	resp.Body.Close()
	if !(resp.StatusCode == 200 || resp.StatusCode == 201) {
		return errors.New(resp.Status)
	}
	return nil
}

func CheckLicense() {
	GetLicenseStatus()

	ticker := time.NewTicker(10 * time.Minute)
	for {
		select {
		case <-ticker.C:
			GetLicenseStatus()
		}
	}
}

func GetLicenseStatus() {
	endpoint := fmt.Sprintf("%s:%s", viper.GetString("license.host"), viper.GetString("license.port"))
	reqData, err := Get(endpoint + global.LicenseTimeApi)
	if err != nil {
		logrus.Errorf("get license time error: %v", err)
		return
	}
	rep := &license.LicenseInfo{}
	if err = json.Unmarshal(reqData, rep); err != nil {
		logrus.Errorf("unmarshal license time error: %v", err)
		return
	}
	if rep.Status == "已授权" {
		global.LicenseStatus = true
	} else {
		global.LicenseStatus = false
	}
}
