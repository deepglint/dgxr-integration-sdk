package games

// import (
// 	"fmt"
// 	"meta/common/models/games"
// 	"meta/games/skiing"
// 	"meta/global"
// 	"sync"

// 	"github.com/sirupsen/logrus"
// )

// var personId string

// func InitGames() {
// 	global.Games = global.GamesData{
// 		Games: make(map[games.GameName]*global.GameData, 0),
// 		Mutex: sync.RWMutex{},
// 	}
// 	for {
// 		// time.Sleep(33 * time.Millisecond)
// 		// for k, source := range global.Sources {
// 		// 	// logrus.Info("识别举手")
// 		// 	sd, err := source.LastData()
// 		// 	if err == nil && rule.RaiseHandRight(sd) {
// 		// 		// logrus.Info("识别到举手。举手人id", k)
// 		// 		personId = k
// 		// 	}
// 		// }
// 		// rules.Conductor(personId)
// 		if game := global.Games.GetGames(games.Skiing); game != nil && game.PersonID != "" {
// 			fmt.Println(len(global.Sources))
// 			if game.StartEnable {
// 				skiing.Skiing(game.PersonID)
// 			} else {
// 				if personSource, ok := global.Sources[game.PersonID]; ok {
// 					logrus.Info("开始判断开始动作")
// 					game.StartEnable = skiing.StartGame(personSource)
// 				}
// 			}
// 		}

// 	}
// }
