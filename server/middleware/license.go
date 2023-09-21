package middleware

import (
	"meta/global"
	"net/http"

	"github.com/gin-gonic/gin"
)

func License() gin.HandlerFunc {
	return func(c *gin.Context) {
		if !global.LicenseStatus {
			c.AbortWithStatusJSON(http.StatusUnauthorized, gin.H{
				"msg": "license is not authorized",
			})
			return
		}
		c.Next()
	}
}
