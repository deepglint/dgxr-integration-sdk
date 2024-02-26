package db

import (
	"bufio"
	"encoding/json"
	"fmt"
	"os"
	"time"

	"reverie/global"
	"reverie/model"
	pb "reverie/source/input/grpc/proto"

	_ "github.com/mattn/go-sqlite3"
	"github.com/sirupsen/logrus"
	log "github.com/sirupsen/logrus"
	"github.com/spf13/viper"
	"xorm.io/xorm"
	xormLog "xorm.io/xorm/log"
)

func InitDB() {
	if global.Config != nil {
		if engine, err := xorm.NewEngine("sqlite3", fmt.Sprintf("%s/reverie.db?_busy_timeout=300000", "./")); err != nil {
			log.Fatalf("init db failed, err:%v", err)
		} else {
			engine.ShowSQL(true)
			switch viper.GetString(global.Config.Log.Level) {
			case "debug":
				engine.Logger().SetLevel(xormLog.LOG_DEBUG)
			case "info":
				engine.Logger().SetLevel(xormLog.LOG_INFO)
			case "warn":
				engine.Logger().SetLevel(xormLog.LOG_WARNING)
			case "error":
				engine.Logger().SetLevel(xormLog.LOG_ERR)
			default:
				engine.Logger().SetLevel(xormLog.LOG_INFO)
			}
			global.DbEngine = engine
			initTables()
			InitTemplate()
		}
	} else {
		log.Fatal("init pg failed, try to init config first")
	}
}

func initTables() {
	err := global.DbEngine.Sync2(
		new(model.Template),
	)
	if err != nil {
		log.Fatalf("init tables failed, %v", err)
		return
	}
}

type ReqDataInfo struct {
	ReqInfo  *pb.Request `json:"reqInfo"`
	RecvTime time.Time   `json:"recvTime"`
}

func InitTemplate() {
	buildTemplate(2001, "db/template/slidLeft.txt", "SlidLeft", 0.1, 0.2, []int{5, 6, 8, 10, 11, 12, 13, 14, 21, 23}, 0)
	buildTemplate(2002, "db/template/slidRight.txt", "SlidRight", 0.1, 0.2, []int{5, 6, 7, 9, 11, 12, 13, 14, 21, 22}, 0)
	buildTemplate(2003, "db/template/slidUp.txt", "SlidUp", 0.4, 0.4, []int{5, 6, 7, 9, 11, 12, 13, 14, 21, 22}, 0)
	buildTemplate(2004, "db/template/slidDown.txt", "SlidDown", 0.25, 0.6, []int{5, 6, 8, 10, 11, 12, 13, 14, 21, 23}, 0)
	buildTemplate(2005, "db/template/handAlway.txt", "HandAlway", 0.3, 0.7, []int{5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 22, 23}, 0)
	buildTemplate(2006, "db/template/handClose.txt", "HandClose", 0.3, 0.5, []int{5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 22, 23}, 0)

	buildTemplate(2008, "db/template/armToForward.txt", "ArmToForward", 0.2, 0.4, []int{5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 23}, 1)
	buildTemplate(2009, "db/template/armToBack.txt", "ArmToBack", 0.2, 0.4, []int{5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 23}, 1)
	buildTemplate(2010, "db/template/armToLeft.txt", "ArmToLeft", 0.2, 0.4, []int{5, 6, 7, 9, 11, 12, 13, 14, 21, 22}, 1)
	buildTemplate(2011, "db/template/armToRight.txt", "ArmToRight", 0.15, 0.4, []int{5, 6, 8, 10, 11, 12, 13, 14, 21, 23}, 1)

	buildTemplate(2013, "db/template/close.txt", "Close", 0.3, 0.4, []int{5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 22, 23}, 1)
}

func buildTemplate(id int, fileName, actionName string, score, pathLen float32, keyNode []int, singleFrame int) {
	t := model.Template{
		BaseModel: model.BaseModel{
			Id: id,
		},
		SingleFrame: singleFrame,
		Name:        actionName,
		Score:       score,
		PathLen:     pathLen,
	}
	if ok, err := t.Get(global.DbEngine); ok && err == nil {
		return
	} else if err != nil {
		logrus.Errorf("get template fail: %v", err)
		return
	}

	file, err := os.Open(fileName)
	if err != nil {
		logrus.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	posArrData := [][][]float64{}
	n := 0
	for scanner.Scan() {
		n++
		line := scanner.Text()
		data := &ReqDataInfo{}
		_ = json.Unmarshal([]byte(line), data)
		if data.ReqInfo == nil || data.ReqInfo.FrameId == "" {
			continue
		}
		posArr := [][]float64{}
		for k, v := range data.ReqInfo.Result {
			if k != "999001" || len(v.ThreeDim) < 0 {
				continue
			}
			for _, data := range v.ThreeDim {
				for _, pos := range data.Objs {
					posArr = append(posArr, []float64{float64(pos.Value[0]), float64(pos.Value[1]), float64(pos.Value[2])})
				}
				posArrData = append(posArrData, posArr)
				break
			}
		}
	}
	data, err := global.BuildTemplate(posArrData)
	if err != nil {
		panic(err)
	}
	jsonData, _ := json.Marshal(data)
	jsonKeyNode, _ := json.Marshal(keyNode)
	t.Pos = string(jsonData)
	t.KeyNode = string(jsonKeyNode)
	if err := t.AddTemplate(global.DbEngine); err != nil {
		logrus.Errorf("add template fail: %v", err)
	}
}
