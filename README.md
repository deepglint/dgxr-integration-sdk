# meta

基于灵境空间3dPos的动作识别（举手、转向、蹲跳、向上挥手、向前指）



time="2023-07-28T10:45:00+08:00" level=info msg="开 始判断开始动作"
time="2023-07-28T10:45:00+08:00" level=info msg="vjoy button1"
2
time="2023-07-28T10:45:00+08:00" level=info msg="开 始判断开始动作"
time="2023-07-28T10:45:00+08:00" level=info msg="vjoy button1"
2
panic: runtime error: index out of range [5] with length 0

goroutine 36 [running]:
meta/games/skiing.Skiing({0xc0007ec236?, 0xc000516180?})
        /Users/lzc/liu/pro/meta/meta/games/skiing/skiing.go:42 +0x905
meta/games.InitGames()
        /Users/lzc/liu/pro/meta/meta/games/initGames.go:32 +0x145
created by main.main
        /Users/lzc/liu/pro/meta/meta/main.go:17 +0x5a

E:\lzc\meta>











