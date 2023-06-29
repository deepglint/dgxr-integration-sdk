package global

//	{
//	    "frameId": 12334,
//	    "personId": "xxxx",
//	    "tsEngine": 1233444,
//	    "tsWS": 12334444,
//	    "pose": {
//	        "keyPoints": [[]],
//	        "direction": 90,
//	        "event": "squat"
//	    }
//	}

type Pose struct {
	KeyPoints [][]float32 `json:"keyPoints"`
	Direction int         `json:"direction"`
	Action    string      `json:"action"`
}

type Action struct {
	FrameId  string `json:"frameId"`
	PersonId string `json:"personId"`
	TsEngine int64  `json:"tsEngine"`
	TsWS     int64  `json:"tsWS"`
	Pose     Pose   `json:"pose"`
}

var ActionChan chan Action

// var Q *model.Queue
