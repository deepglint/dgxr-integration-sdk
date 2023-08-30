package global

import (
	"meta/model/config"
	games "meta/model/game"
	"meta/model/source"
	"sync"
)

type GameData struct {
	Game           games.GameName `json:"game"`
	StartEnable    bool           `json:"startEnable"`
	PersonID       string         `json:"personId"`
	OriginalSource source.SourceData
	Jump           bool
}

var (
	Games    GamesData
	Config   *config.Config
	Sources  map[string]*source.Source
	PersonID string
)

func InitSources() {
	Sources = make(map[string]*source.Source, 0)
}

type GamesData struct {
	Mutex sync.RWMutex // 读写锁
	Games map[games.GameName]*GameData
}

func (g *GamesData) GetGames(name games.GameName) *GameData {
	g.Mutex.RLock()
	defer g.Mutex.RUnlock()
	return g.Games[name]
}

func (g *GamesData) SetGames(name games.GameName, gameData *GameData) {
	g.Mutex.Lock()
	defer g.Mutex.Unlock()
	g.Games[name] = gameData
}

func (g *GamesData) SetGamesSource(name games.GameName, obj source.SourceData) {
	g.Mutex.Lock()
	defer g.Mutex.Unlock()
	g.Games[name].OriginalSource = obj
}

func (g *GamesData) SetGamesJump(name games.GameName, jump bool) {
	g.Mutex.Lock()
	defer g.Mutex.Unlock()
	g.Games[name].Jump = jump
}
