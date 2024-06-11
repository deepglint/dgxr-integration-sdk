# CustomPlayerManager
CustomPlayerManager示例程序展示了如何进行玩家绑定以及设备接入。

## 基本概念

1. Character 游戏角色，游戏角色是游戏内被玩家直接操作的游戏对象的数据抽象，它标识了游戏角色的身份信息，游戏角色与玩家是1:1对应关系；
2. Player 玩家，由PlayerManager通过PlayerPrefab实例化出来的对象，玩家的输入源是Unity系统中的各种设备，每个玩家可关联多个设备，通过操作设备的输入数据实现对游戏内角色的控制，每个玩家唯一控制一个游戏内的角色；
3. Device 设备，设备是游戏内的交互控制器，游戏手柄、键盘、鼠标、灵境空间中的被追踪的人等都是Unity系统可以感知的设备，通过InputSystem的[PlayerInput](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.PlayerInput.html)我们可以将玩家与设备进行配对，从而实现不同的玩家只从特定的设备获取输入数据来操作游戏内的角色；
4. PlayerManager 控制玩家加入，设备配对的游戏对象；
5. PlayerPrefab 玩家预制体，由应用开发者提供，并托管给PlayerManager, 当有玩家通过`JoinFromAction`或者`JoinFromUI`的方式加入游戏时，由PlayerManager完成PlayerPrefab的实例化；

## 对接流程

1. 创建Action asset, 参考[动作交互对接流程](#动作交互对接流程-)；
2. 创建PlayerPrefab 玩家预制件, 并为预制件添加PlayerInput组件![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/PlayerManager001.jpg)；
3. 创建EmptyGameObject 添加PlayerManager组件，为PlayerManager添加上面创建的PlayerPrefab并选择合适的JoinBehavior完成PlayerManager的配置。PlayerManager提供了两种JoinBehavior, JoinFromAction(通过做指定动作加入) 和JoinFromUI(通过与指定UI元素交互加入)，开发者可根据需求选用合适的JoinBehavior, 其中CustomPlayerManager1, CustomPlayerManager2, CustomPlayerManager3展示了通过JoinFromAction的方式加入游戏，CustomPlayerManager4展示了通过JoinFromUI的方式加入游戏;
4. 创建Character的派生类或者实现ICharacter接口，并监听PlayerManager的回调事件，在回调事件中完成玩家与游戏角色的绑定。其中CustomPlayerManager1，CustomPlayerManager2展示了通过继承Character类的方式对接PlayerManager, CustomPlayerManager3, CustomPlayerManager4展示了通过实现ICharacter的方式对接PlayerManager；

## 运行Sample 程序

1. 通过PackageManager导入Sample程序![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/Sample001.jpg);
2. 选择对应脚本并运行![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/Sample002.jpg);

功能说明：
- CustomPlayerManager1 展示了玩家通过JoinFromAction的方式加入游戏，游戏中在地屏的（0，0）坐标处有一个半径为1米的ROI，当玩家在ROI范围内作出加入动作时完成玩家与角色的绑定，否则拒绝绑定；当已绑定玩家走出ROI区域时解绑玩家身上的设备，当由新的DGXRHumanController设备进入ROI区域时手动将新设备绑定到Player上；
- CustomPlayerManager2 展示了多玩家通过JoinFromAction的方式加入游戏，当空间的DGXRHumanController设备走入角色对应的ROI区域并作出join的动作时，PlayerManager会完成对应的玩家的创建以及角色绑定的操作；
- CustomPlayerManager3 展示了通过JoinFromAction 实现ICharacter接口的方式加入游戏，游戏中在地屏的（0，0）坐标处有一个半径为1米的ROI，当玩家在ROI范围内作出加入动作时完成玩家与角色的绑定，否则拒绝绑定；
- CustomPlayerManager4 展示了通过JoinFromUI的方式加入游戏，游戏运行时动态的在地屏创建一个圆形光圈，当玩家走进光圈时，PlayerManager会完成对应玩家的创建以及角色绑定操作；

> 备注：
> CustomPlayerManager1, CustomPlayerManager2, CustomPlayerManager3, CustomPlayerManager4不能同时运行，否则会互相影响；