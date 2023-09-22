package util

import (
	"io/ioutil"
	"net/http"
)

const ConfigPath = "./"

func Get(url string) (data []byte, err error) {
	resp, err := http.Get(url)
	if err != nil {
		return
	}
	defer resp.Body.Close()

	data, err = ioutil.ReadAll(resp.Body)
	if err != nil {
		return
	}
	return
}
