package server

import (
	"encoding/json"
	"fmt"
	"net/http"
	"path"
	"time"

	"reverie/global"
	"reverie/model/license"
	"reverie/util"

	"github.com/gin-gonic/gin"
	log "github.com/sirupsen/logrus"
	"github.com/spf13/viper"
)

func InitLicense(r *gin.Engine) {
	system := r.Group("system")
	system.POST("/license", UploadLicense)
	system.GET("/license/fingerprint", LicenseFile)
	system.GET("/license/due", GetLicenseTime)
}

func UploadLicense(c *gin.Context) {
	file, err := c.FormFile("file")
	if err != nil {
		log.Error(err)
		c.JSON(http.StatusBadRequest, gin.H{
			"msg": "invalid parameter",
		})
		return
	}
	if err = c.SaveUploadedFile(file, path.Join(util.ConfigPath, "file", file.Filename)); err != nil {
		log.Error(err)
		c.JSON(http.StatusBadRequest, gin.H{
			"msg": err.Error(),
		})
		return
	}

	if err := PostLicense(file.Filename); err != nil {
		log.Error(err)
		c.JSON(http.StatusBadRequest, gin.H{
			"msg": err.Error(),
		})
		return
	}
	c.JSON(http.StatusNoContent, nil)
}

func LicenseFile(c *gin.Context) {
	data, err := GetLicense()
	if err != nil {
		log.Error(err)
		c.JSON(http.StatusBadRequest, gin.H{
			"msg": err.Error(),
		})
		return
	}
	c.Writer.Header().Add("Content-Disposition", fmt.Sprintf("attachment; filename=%s.C2V", time.Now().Format("2006-01-02")))
	c.Data(http.StatusOK, "application/octet-stream", data)
}

func GetLicenseTime(c *gin.Context) {
	data, err := GetLicenseFile()
	if err != nil {
		log.Error(err)
		c.JSON(http.StatusBadRequest, gin.H{
			"msg": err.Error(),
		})
		return
	}
	c.JSON(http.StatusOK, data)
}

type LicenseService struct{}

// PostLicense 下发授权文件
func PostLicense(fileName string) error {
	endpoint := fmt.Sprintf("%s:%s", viper.GetString("license.host"), viper.GetString("license.port"))
	if err := util.DoUploadLicense(endpoint+global.LicenseFileUploadApi, path.Join(util.ConfigPath, "file", fileName)); err != nil {
		return err
	}
	return nil
}

// GetLicense 获取指纹文件
func GetLicense() (data []byte, err error) {
	endpoint := fmt.Sprintf("%s:%s", viper.GetString("license.host"), viper.GetString("license.port"))
	data, err = util.Get(endpoint + global.LicenseFileApi)
	return
}

// GetLicenseFile 获取授权文件时间
func GetLicenseFile() (data *license.LicenseInfoRep, err error) {
	endpoint := fmt.Sprintf("%s:%s", viper.GetString("license.host"), viper.GetString("license.port"))
	reqData, err := util.Get(endpoint + global.LicenseTimeApi)
	if err != nil {
		return nil, err
	}
	rep := &license.LicenseInfo{}
	if err = json.Unmarshal(reqData, rep); err != nil {
		return nil, err
	}
	loc, err := time.LoadLocation("Asia/Shanghai")
	if err != nil {
		return nil, err
	}
	data = &license.LicenseInfoRep{
		Expire: time.Unix(rep.ExpireAt, 0).In(loc).Format("2006-01-02 15:04:05"),
	}
	if rep.Status == "已授权" {
		data.Status = license.Authorized
	} else {
		data.Status = license.Unauthorized
	}
	return
}
