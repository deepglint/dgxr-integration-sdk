DG Unity Integration SDK is a software development kit developed by Deepglint based on the Unity engine. The SDK encapsulates a series of functions, covering rendering, tracking, input, interaction, mixed reality, platform services, and more.

DG Unity Integration SDK为 Deepglint 基于 Unity 引擎研发的软件开发工具包。SDK 封装了一系列功能，涉及渲染、追踪、输入、交互、混合现实、平台服务等。

[![dgxr-integration-sdk](https://img.youtube.com/vi/x69_Z1tR-p8/0.jpg)](https://www.youtube.com/watch?v=x69_Z1tR-p8)


# Contact
<a href="mailto:xinyuchen@deepglint.com">xinyuchen@deepglint.com</a>

# DG Integration 文件夹说明

在 Unity 编辑器内导入 SDK 后，Package 目录下将出现 DGXR Integration SDK 文件夹 (下图所示)：

![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.002.png)

# XR Manager 介绍

XR Manager 是 DGXR Unity Integration SDK 的重要组成部分。应用开发过程中，你需要为每个场景都添加 XR Manager。

## UI展示

XR Manager 脚本面板如下：

![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.003.png)

## 功能说明

你可以通过 XR Manager 管理和开启许多 SDK 提供的能力。

| 功能  | 简述                                                   |
|:----|:-----------------------------------------------------|
| 身体追踪| 身体追踪（Body Tracking）用于收集用户的身体位置和动作信息，并将其转换为可再现的姿态数据。  |
| 空间锚点| 空间锚点技术可以将场景展示位置与灵境空间的位置进行锚定，用于将虚拟场景根据现实的空间大小、屏幕数量进行展示|
| 零点过滤| 开启零点过滤，将自动筛选丢失的点，并使用上一帧该点的位置参与计算                     |

# 快速开始

## 导入 SDK

DGXR Unity Integration SDK 是 deepglint 官方基于 Unity XR 提供的开发工具，其中包含开发 DG XR 应用所需的功能、组件、插件和脚本。本文档介绍如何在 Unity Hub 中创建项目，然后在项目中导入 SDK。

### 第一步：创建项目

导入 SDK 前，需要在 Unity Hub 中新建项目。步骤如下：

1. 在 Unity Hub 主页，点击项目 ->新项目。

你将进入新建项目页。

![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.004.png)

2. 选择 Universa3D(URP模版)。
3. 在 项目设置 区域，设置项目名称和存储目录。

`  `提示：项目名称和存储目录不能包含中文字符和数字开头

4. 点击 创建项目。

项目初始化完毕后，你将进入 Unity 编辑器页面。

### 第二步：导入 DGXR Unity Integration SDK

你可以从以下方式中任选其一，导入 DGXR Unity Integration SDK 至你的项目。

| 方式          | 步骤                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
|:------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 导入本地的 SDK 包 | <p>1. 前往deepglint 开发者平台页面，下载最新版本的 SDK。 </p><p>2. 解压所下载的 SDK 压缩包。 你将会得到一个包含 package.json 文件的文件夹。 </p><p>3. 返回 Unity 编辑器页面。 </p><p>4. 在上方菜单栏处，选择 Windows > Package Manager。 </p><p>5. 在 Package Manager 窗口中，点击 + > Add package from disk。 </p><p>6. 选择 package.json 文件并导入。 </p><p>导入完成后，你将看到 XR SDK Setting 窗口，关闭即可。 </p>                                                                                                                                                                                                                                                                                            |
| 注册Scope     | <p>1. 打开 Unity 编辑器页面。 </p><p>2. 在上方菜单栏处，选择 Edit > Project Setting > Package Manager。 </p><p>3. 在 Package Manager 窗口中，点击 + > Add package。 </p><p>![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/package-manager.jpg)</p><p>4. 输入仓库的地址，然后点击 Add。<br/>`name: package.openupm.com`<br/>`url:https://package.openupm.com`<br/>` com.deepglint.xr`<br/>` com.yasirkula.ingamedebugconsole`<br/>` io.sentry.unity`<br/><p>5.Unity 编辑器Windows > Package Manager>My registry。开始从中导入 SDK。 </p> |


### 第三步：初始化XRApplicationSettings应用配置

导入DGXR Unity Integration SDK 后可通过DGXR-->XRApplicationSettings 菜单，根据应用的特点自定义应用描述文件。
![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/init-sdk-001.jpg)
![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/init-sdk-002.jpg)

| 配置项          | 功能说明                                                    |
|:--------------|:--------------------------------------------------------|
| Application Type | 描述应用的类型（如：益智、棋牌、影视等）                     |
| Minimum Player Count | 应用支持的最小玩家数                                  |
| Maxmum Player Count   | 应用支持的最大玩家数                                 |
| Description     | 应用描述信息                                               |
| EnableExitButton |  是否为应用启用默认退出按钮                                  |
| EnableLoseFocuseTip  | 是否为应用启用失焦提示                                  |


## 创建一个 XR 场景

**预计阅读和完成时间：15 分钟**

本文档介绍如何升级 XR Interaction SDK，然后创建一个基础 XR 场景（下图所示）。此外，文档也会介绍玩家绑定、骨骼跟踪相关内容。

![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.006.png)

### 第一步：升级 XR Interaction SDK 并导入示例文件

搭建基础 XR 场景需要用到 Unity 提供的基础功能和组件。升级XR Interaction SDK 以便获得 Unity 提供的新资源包和功能。

1. Unity 编辑器Windows -> Package Manager -> My registry。
2. 在 Package Manager 窗口中，列表中将展示 Unity Registry 中提供的工具包。
3. 在列表中找到 XR Interaction Toolkit，并将其展开。
4. 点击 See other versions 展开版本列表。
5. 从列表中选择 2.0.0 或以上版本，然后点击窗口右下角的 Update to version\_number 按钮。
6. 升级完成后，再次前往 WindowsPackage ManagerXR Interaction SDK。
7. 展开右侧的 Samples 面板。
8. 点击 Import，导入示例文件。

| 示例文件                    | 说明                                                                                                           |
|:------------------------|:-------------------------------------------------------------------------------------------------------------|
| PlayerManager           | 该示例文件默认位于 Assets/Samples/XR Interaction SDK/[version]/PlayerManager 目录下，提供了一套标准的玩家绑定示例，包括一套默认的输入动作和预设。       |
| HumanBody               | HumanBody ，提供用于空间中的人体跟踪，默认位于 Assets/Samples/XR Interaction SDK/[version]/HumanBody  目录下。该示例文件用于展示空间骨骼是否正常接入。 |
| HumanControlInputModule | sample程序位于Assets/Samples/XR Interaction SDK/[version]/PlayerManager，展示了通过脚与空间地屏中的UI和3D元素的交互                  | 

### 第二步：拖入 XRManager 预制体

预制体位于 Packages/DGXR Interaction SDK文件夹下

1. 预制体拖入后，会自动在坐标原点创建 灵境空间的场景（项目启动后会自动隐藏）
2. 移动要展示的场景与空间重合，调整场景大小，便于与实际空间物品比例相同(也可以修改 XRManager 的 space scale来调整灵境空间大小)。
3. 运行项目，即可根据默认配置文件生成对应屏幕数量的相机渲染内容
4. 在脚本中设置空间视角跟随功能，勾选 IsCave 选项，挂载脚本（示例脚本如下）
5. 修改 XRManager/XRspace 的 position，既可实现空间移动到场景不同位置渲染不同画面
```
using Deepglint.XR;
using Deepglint.XR.Source;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Update()
    {
        if (Source.Data.Count == 0)
        {
            return;
        }

        var body = Source.Data[0];
        DGXR.CavePosition = body.Joints.HeadTop.LocalPosition;
    }
}
```


**后续操作**

参考《开发》文档，探索更多 DGXR Interaction SDK更多功能！

**注意事项**

- 当应用失去焦点的时候，骨骼数据不会接收和更新（source.Data数据也不会实时更新）
- 可以通过 Source.Source.RealTimePoseFrameDataReceived 订阅实时帧数据

# 开发

## 两种游戏模式

### 固定视角模式

**介绍**

固定视角模式，指的是我们以场景中心点为固定视角进行开发，空间物品不根据人的位置进行动态的变化，当场景和位置固定均不移动的情况下，人在空间内移动，看到的所有画面都是不变的。
默认情况下，我们使用的就是这种模式，只需要导入预制体，不需要任何配置。

**使用方式**

* 导入预制体后，我们就提供了 5 台渲染相机，分别对应空间中的五面屏幕（其中地平Game 窗口设置分别率为 1920 * 1920，其余 Game 窗口均为 1920 * 1200）
* 接下来就可以正常开发具体游戏逻辑。每一个 Game 窗口看到的内容就是在灵境空间内对应屏幕的内容。


### 沉浸式 cave 模式

**介绍**

cave 模式指的是为增加用户的沉浸感，空间内的物体根据自身位置做出响应的形变。例如：空间内的一张桌子，你可以围绕着桌子观察桌子的上下左右各个面的细节。
切 cave 模式永远保证用户在最佳观察着视角，空间内的任何 3d 物体在第一视角上均不发生形变。

**缺点**

只有一个最佳视角跟踪

**使用方式**

XRManager 下勾选 isCave的开关


## 渲染

默认情况下预制体会创建一组 3d相机，用于渲染前后左右地五块屏幕。通过设置空间中人的视角位置，动态计算并渲染相机内容输出对应屏幕上。开发者无需关心渲染和相机相关操作和配置，此相机和屏幕根据实际空间大小和安装方式动态创建。

### UI 渲染

内部使用，暂不对外提供

### 自定义渲染

1. 创建自己的 3d 相机拍摄想要渲染的画面
2. 设置相机为 overlay 模式，并添加相机到对应的屏幕上（具体代码如下）
```
using Deepglint.XR;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NewBehaviourScript : MonoBehaviour
{
private void Start()
{
Camera camera = new Camera();
camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
DGXR.Space.Front.AddCameraToStack(camera);
}
} 
```

## 交互

### 人体跟踪获取

使用步骤：

1. 在场景中导入Runtime/Prefabs/DGXRManager预制体
2. 直接获取骨骼列表示例
```
using Deepglint.XR.Source;
using UnityEngine;

public class Demo: MonoBehaviour
{
    void Start()
    {
        if (Source.Data.Count == 0)
        {
            return;
        }

        var body = Source.Data[0];

        // position unity世界坐标系中的坐标
        var nose = body.Joints.Nose.Position;

        // LocalPosition 空间中心原点相对坐标
        var nose = body.Joints.Nose.LocalPosition;
    }
}
```
3. 订阅方式使用骨骼示例
```
using Deepglint.XR.Source;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // 订阅MetaPoseDataReceived事件的方法
    private void OnEnable()
    {
        Source.OnMetaPoseDataReceived += HandleMetaPoseDataReceived;
    }

    // 在禁用对象时取消订阅事件
    private void OnDisable()
    {
        Source.OnMetaPoseDataReceived -= HandleMetaPoseDataReceived;
    }

    // 当接收到MetaPose数据时被调用的方法
    private void HandleMetaPoseDataReceived(SourceData data)
    {
        // 在这里编写处理MetaPose数据的逻辑
        Debug.Log("Received MetaPose Data: " + data.ToString());
    }
}
```

### 设备管理

灵境空间中每一个被追踪到的人都被视为一个DGXRHumanController类型的设备，当空间中的人的位置和朝向发生变化或者空间中的人做了系统能够识别的动作时，其对应的设备上的布局也会发生相应变化。

| 设备布局          | 功能说明                                                    |
|:--------------|:--------------------------------------------------------|
| HumanPose     | 人的位置和朝向                                                 |
| HumanBody     | 人体骨骼关键点信息                                               |
| Stick         | 摇杆(stick 的取值为humanPose所在的位置相对与StickAnchor 在X，Z平面上的方向向量) |
| FreeSwim      | 自由泳动作                                                   |
| ButterflySwim | 蝶泳动作                                                    |
| HighKneeRun   | 高抬腿动作                                                   |
| DeepSquat     | 深蹲动作                                                    |
| Jump          | 跳跃动作                                                    |

DeviceManager 类管理了空间中所有的设备，可以通过DeviceManager监听设备的上下线、获取在场活跃设备数量和列表。
示例代码如下：
```
using Deepglint.XR.Inputs;
using UnityEngine.InputSystem;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // 订阅DeviceManager的事件
    private void OnEnable()
    {
        DeviceManager.OnDeviceAdd += OnDeviceAdd;
        DeviceManager.OnDeviceLost += OnDeviceLost;
        DeviceManager.OnDeviceRegain += OnDeviceRegain;
    }

    // 取消订阅
    private void OnDisable()
    {
        DeviceManager.OnDeviceAdd -= OnDeviceAdd;
        DeviceManager.OnDeviceLost -= OnDeviceLost;
        DeviceManager.OnDeviceRegain -= OnDeviceRegain;
    }

    // 处理设备加入 
    private void OnDeviceAdd(InputDevice device)
    {
        // 在这里编写处理设备加入的逻辑
        Debug.LogFormat("device {0} was added", device.deviceId);
    }
    
    // 处理设备离线 
    private void OnDeviceLost(InputDevice device)
    {
        // 在这里编写处理设备离线的逻辑
        Debug.LogFormat("device {0} was lost", device.deviceId);
    }
    
    // 处理设备重连 
    private void OnDeviceAdd(InputDevice device)
    {
        // 在这里编写处理设备重连的逻辑
        Debug.LogFormat("device {0} was reconnected", device.deviceId);
    }
}
```

### 场景互动

`Deepglint.XR.Interaction` SDK 提供了多种方式使得用户能与应用场景中的元素进行互动
1. 可以通过[玩家绑定及动作接入](#玩家绑定及设备接入)的方式，建立游戏角色、玩家与设备之间的映射关系，进而控制空间中的元素进行交互；
2. 通过使用`HumanControlFootPointerInputModule`模块提供的能力直接与canvas 中的UI元素或者场景中的3d元素进行交互；

#### HumanControlFootPointerInputModule 
`HumanControlFootPointerInputModule` 通过实时获取人体左右脚的骨骼数据，以类似于触控板多点触控的方式，将人在场景中与UI 或3D元素的交互转化为EventSystem 中的PointerEnter，PointerExit, PointerDown, PointerUp等事件，通过对EventSystem中回调事件的处理我们可以轻松的处理人与场景中UI 或者3D元素交互的逻辑。

`HumanControlFootPointerInputModule`支持的事件：

| 事件类型         | 功能说明 |
|:-------------|:-----|
| PointerEnter | 进入   |
| PointerExit  | 离开   |
| PointerDown  | 踩下   |
| PointerUp    | 抬起   |
| PointerClick | 点击   |
| Drag         | 拖拽   |
| BeginDrag    | 开始拖拽 |
| EndDrag      | 结束拖拽 |
| Move         | 移动   |

`HumanControlFootPointerInputModule`支持的使用流程如下(详情可参考示例程序中的HumanControlInputModule)：
1. 在场景中引入XRManager预制件；
2. 在Bottom的UIRoot中创建可交互的UI元素；![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/InputModule001.jpg);
3. 编写脚本监听`HumanControlFootPointerInputModule`交互事件，并将脚本挂载到UI元素上;

备注：当需要与3D 物体进行交互时需要手动开启物理射线
```
// 检查摄像机上是否已经有 PhysicsRaycaster 组件
if (DGXR.Space.Bottom.SpaceCamera.GetComponent<PhysicsRaycaster>() == null)
{
   // 添加 PhysicsRaycaster 组件
   DGXR.Space.Bottom.SpaceCamera.gameObject.AddComponent<PhysicsRaycaster>();
   Debug.Log("PhysicsRaycaster has been added to the bottom camera.");
}
```

>注意事项：
> 1. EventSystem上的Pointer事件都是通过发送射线的机制，检测到碰撞的物体进行交互的，因此在使用EventSystem上的HumanControlFootPointerInputModule时注意物体之间的遮挡关系，可通过调节射线照射的UI元素的layer层或者把不需要检测的元素的RaycastTarget属性反选掉以避免元素之间的遮挡关系导致事件无法触发。
> 2. 默认情况下整个UI元素都是射线的可交互区，可通过RaycastPadding属性调整可交互区的大小，以免造成交互事件的误触。

### 动作判定

`Deepglint.XR.Interaction`模块基于InputSystem的ActionMap机制，提供了包含算法识别、规则判定等多种丰富的动作判定功能，目前`Deepglint.XR.Interaction`支持的动作集有：

| 动作                  | 数据类型      | 功能说明   |
|:--------------------|:----------|:-------|
| RaiseLeftHand       | HumanBody | 举左手    |
| RaiseRightHand      | HumanBody | 举右手    |
| RaiseSingleHand     | HumanBody | 举单手    |
| RaiseBothHand       | HumanBody | 举双手    |
| RaiseHand           | HumanBody | 举手     |
| SlideLeftArmToRight | HumanBody | 左手向右翻页 |
| SlideRightArmToLeft | HumanBody | 右手向左翻页 |
| ButterflySwim       | Axis      | 蝶泳     |
| DeepSquat           | Axis      | 深蹲     |
| FreeSwim            | Axis      | 自由泳    |
| HighKneeRun         | Axis      | 高抬腿    | 
| Jump                | Axis      | 跳跃     |

#### 动作交互对接流程 

1. 创建动作资产![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/Action001.jpg)
2. 配置ActionMap, 并设置数据类型![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/Action002.jpg)
3. 选择设备类型和Interaction ![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/Action003.jpg)
4. 交互控制逻辑请参考文档中玩家绑定模块，或者直接导入`Deepglint.XR.Interaction` Samples中的PlayerManager 示例程序（或[PlayerInput](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.PlayerInput.html)）

### 玩家绑定及设备接入

在开发多玩家应用时，通过`Deepglint.XR.Interaction`的PlayerManager模块可轻松实现多个玩家的游戏角色绑定，每个玩家可独立控制单独的游戏角色进行交互。

#### 基本概念

1. Character 游戏角色，游戏角色是游戏内被玩家直接操作的游戏对象的数据抽象，它标识了游戏角色的身份信息，游戏角色与玩家是1:1对应关系；
2. Player 玩家，由PlayerManager通过PlayerPrefab实例化出来的对象，玩家的输入源是Unity系统中的各种设备，每个玩家可关联多个设备，通过操作设备的输入数据实现对游戏内角色的控制，每个玩家唯一控制一个游戏内的角色；
3. Device 设备，设备是游戏内的交互控制器，游戏手柄、键盘、鼠标、灵境空间中的被追踪的人等都是Unity系统可以感知的设备，通过InputSystem的[PlayerInput](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.PlayerInput.html)我们可以将玩家与设备进行配对，从而实现不同的玩家只从特定的设备获取输入数据来操作游戏内的角色；
4. PlayerManager 控制玩家加入，设备配对的游戏对象；
5. PlayerPrefab 玩家预制体，由应用开发者提供，并托管给PlayerManager, 当有玩家通过`JoinFromAction`或者`JoinFromUI`的方式加入游戏时，由PlayerManager完成PlayerPrefab的实例化；

#### 对接流程

1. 创建Action asset, 参考[动作交互对接流程](#动作交互对接流程-)；
2. 创建PlayerPrefab 玩家预制件, 并为预制件添加PlayerInput组件![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/PlayerManager001.jpg)；
3. 创建EmptyGameObject 添加PlayerManager组件，为PlayerManager添加上面创建的PlayerPrefab并选择合适的JoinBehavior完成PlayerManager的配置。PlayerManager提供了两种JoinBehavior, JoinFromAction(通过做指定动作加入) 和JoinFromUI(通过与指定UI元素交互加入)，开发者可根据需求选用合适的JoinBehavior, 详细使用流程可参考`Deepglint.XR.Interaction`中的PlayerManager Sample程序 ![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/PlayerManager002.jpg);
4. 创建Character的派生类或者实现ICharacter接口，并监听PlayerManager的回调事件，在回调事件中完成玩家与游戏角色的绑定。
   - 通过继承Character抽象类的方式对接需要实现OnTryToJoin接口, 当允许玩家操作当前Character时接口需要返回当前Character派生类示例给PlayerManager, 如果不允许玩家控制当前游戏角色直接返回null即可；
   - 通过实现ICharacter接口方式对接需要实现OnPlayerJoin 和OnPlayerLeft接口。当允许玩家操作当前游戏角色时，OnPlayerJoin方法需要将自身的引用返回给PlayerManager, 如果不允许玩家控制当前角色直接返回null即可；
通过继承Character抽象类的方式对接PlayerManager:
```
using Deepglint.XR.Inputs;
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

// This demo demonstrates the process of player joining, manually unbinding, and manually rebinding.
namespace Samples.CustomPlayerManager
{
    public class CustomPlayerManager1 : MonoBehaviour
    {
        private CustomCharacter1 _character;

        public void Start()
        {
            _character = new CustomCharacter1("领航员", new ROI(){ Anchor = Vector2.zero, Radius = 1.0f });
            PlayerManager.Instance.OnTryToJoinWithCharacter += _character.OnTryToJoin;
        }

        public void Update()
        {
            if (_character.Player is not null)
            {
                // check and unpair device manually
                foreach (var device in _character.Player.PairedDevices)
                {
                    if (device is DGXRHumanController dgXRDevice)
                    {
                        Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                        if (Vector2.Distance(_character.Roi.Anchor, new Vector2(position.x, position.z)) > _character.Roi.Radius)
                        {
                            Debug.LogFormat("device {0} stepped out from {1}'s roi", device.deviceId, _character.Name);
                            _character.Player.UnPairDeviceManually(device);
                            Debug.LogFormat("unpair device {0} from character {1} manually", device.deviceId, _character.Name);
                        }
                    }
                    else
                    {
                        Debug.LogFormat("device {0} is not dgxr device", device.deviceId);
                    }
                }

                // check and pair device manually
                if (_character.Player.PairedDevices.Count == 0)
                {
                    var devices = DeviceManager.AllActiveXRHumanDevices;
                    var allPairedDevices = PlayerManager.Instance.AllPairedDevices.ToArray();
                    foreach (var device in devices)
                    {
                        if (!ArrayHelper.Contains(allPairedDevices, device))
                        {
                            Vector3 position = device.HumanPose.Position.ReadValue();
                            if (Vector2.Distance(_character.Roi.Anchor, new Vector2(position.x, position.z)) < _character.Roi.Radius)
                            {
                                Debug.LogFormat("device {0} steeped into {1}'s roi", device.deviceId, _character.Name);
                                if (_character.Player.PairDeviceManually(device))
                                {
                                    Debug.LogFormat("pair device {0} to character {1} manually ", device.deviceId, _character.Name);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public struct ROI
        {
            public Vector2 Anchor;
            public float Radius;
        }

        public class CustomCharacter1 : Character
        {
            public ROI Roi;
            public CustomCharacter1(string name, ROI roi)
            {
                Name = name;
                Roi = roi;
            }
            public override Character OnTryToJoin(InputDevice device)
            {
                if (IsBindable())
                {
                    if (device is DGXRHumanController dgXRDevice)
                    {
                        Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                        if (Vector2.Distance(Roi.Anchor,new Vector2(position.x, position.z)) <= Roi.Radius)
                        {
                            Debug.LogFormat("character {0} is bindable", Name);
                            return this;
                        }
                    }
                }

                Debug.LogFormat("character {0} is not bindable", Name);
                return null;
            }
        }
    }
}
```
通过实现ICharacter接口的方式对接PlayerManager:
```
using Deepglint.XR.Inputs.Devices;
using Deepglint.XR.Player;
using UnityEngine;
using UnityEngine.InputSystem;

// This demo demonstrates the process of multi player management.
namespace Samples.CustomPlayerManager
{
    public class CustomPlayerManager3 : MonoBehaviour
    {
        private Character3 _character;

        public void Start()
        {
            _character = new Character3("火柴人", new ROI(){ Anchor = new Vector2(0, 0), Radius = 1.0f });
            PlayerManager.Instance.OnTryToJoinWithICharacter += _character.OnPlayerJoin;
        }

        public struct ROI
        {
            public Vector2 Anchor;
            public float Radius;
        }

        public class Character3 : ICharacter
        {
            private readonly ROI _roi;
            private GameObject _player;

            public GameObject Player => _player;

            public readonly string Name;

            public Character3(string name, ROI roi)
            {
                Name = name;
                _roi = roi;
            }

            public ICharacter OnPlayerJoin(GameObject player, InputDevice device)
            {
                if (_player is null)
                {
                    if (device is DGXRHumanController dgXRDevice)
                    {
                        Vector3 position = dgXRDevice.HumanPose.Position.ReadValue();
                        if (Vector2.Distance(_roi.Anchor,new Vector2(position.x, position.z)) < _roi.Radius)
                        {
                            Debug.LogFormat("character {0} is bindable", Name);
                            _player = player;
                            return this;
                        }
                    }
                }

                Debug.LogFormat("character {0} is not bindable", Name);
                return null;
            }

            public void OnPlayerLeft()
            {
                Debug.LogFormat("player {0} is left", Name);
                _player = null;
            }
        }
    }
}
```

## 空间管理
### 空间音频

基础功能有播放音频、播放音频列表、停止等

AudioManager的方法不再管理流程，如果想实现播放音频结束后触发某方法的功能，则自己开启一个TimerManager，时间为音频长度 

### 游戏排行榜
#### 生成游戏数据二维码

```
using System;
using Scene.Common;
using UnityEngine;

namespace Deepglint.XR.Toolkit.Game
{
    public class TestGameData :MonoBehaviour,RankConsumer
    {
        public GameObject QRObj; 
        private void Start()
        {
            ShareInfo info = new ShareInfo()
            {
                AvatarId = 1,
                GameMode = GameMode.Single,
                Score = new int[]{200,100},
                Time = DateTime.Now,
                SpaceId = "1111111",
                // QRImageColor = Color.black
            };

            GameObject instance = Instantiate(QRObj);
            var qr = instance.GetComponent<QR>();
            qr.SetQRInfo(info,DGXR.Space.Front,new Vector2(100,100),new Vector2(500,500),1);
        }
    }
}
```

#### 获取游戏排行榜数据

```
using System;
using Deepglint.XR.Toolkit.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Deepglint.XR.Toolkit.Game
{
    public class TestGameData :MonoBehaviour,RankConsumer
    {
        private void Start()
        {
            GameDataManager.Instance.Subscribe(this);
        }


        private void OnApplicationQuit()
        {
            GameDataManager.Instance.Unsubscribe(this);
        }
        
        public RankInfoReq GetRankInfoReq()
        {
            return new RankInfoReq("5f3c73f3", GameMode.Single, 20); 
        }
        
        public void OnDataReceived(RankInfo info)
        {
            Debug.Log($"id:{info.Id}");
            foreach (var sc in info.Data)
            {
                Debug.Log($"score:{sc.Score},time:{sc.Time},mode:{sc.Mode}");
                foreach (var player in sc.Player)
                {
                    Debug.Log($"{player.Id},{player.Url},{player.Name}");
                }
            }
        }
    }
}
```
