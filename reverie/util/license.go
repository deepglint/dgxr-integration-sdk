package util

import (
	"bytes"
	"errors"
	"io"
	"mime/multipart"
	"net/http"
	"os"
	"path/filepath"
)

type LicenseRep struct {
	Error   string    `json:"error,omitempty"`
	Details []Details `json:"details,omitempty"`
}

type Features struct {
	FeatureId  int               `json:"feature_id,omitempty"`
	FeatureMap map[string]string `json:"feature_map,omitempty"`
}

type Details struct {
	NodeAddr    string     `json:"node_addr,omitempty"`
	ExpireTs    int        `json:"expire_ts,omitempty"`
	Features    []Features `json:"features,omitempty"`
	ProductId   string     `json:"product_id,omitempty"`
	ProductName string     `json:"product_name,omitempty"`
}

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
