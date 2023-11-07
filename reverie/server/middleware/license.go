package middleware

import (
	"net/http"

	"reverie/global"

	"github.com/gin-gonic/gin"
)

func License() gin.HandlerFunc {
	return func(c *gin.Context) {
		if !global.LicenseStatus {
			c.AbortWithStatusJSON(http.StatusUnauthorized, gin.H{
				"msg": "license is unauthorized",
			})
			return
		}
		c.Next()
	}
}
