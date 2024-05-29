**DG Unity Integration SDK**

**DG Unity Integration SDK 介绍**

DG Unity Integration SDK为 Deepglint 基于 Unity 引擎研发的软件开发工具包。SDK 封装了一系列功能，涉及渲染、输入&追踪、混合现实、平台服务等。

**DG Integration 文件夹说明**

在 Unity 编辑器内导入 SDK 后，Package 目录下将出现 DGXR Integration SDK 文件夹 (下图所示)：

![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.002.png)

**XR Manager 介绍**

XR Manager 是 DGXR Unity Integration SDK 的重要组成部分。应用开发过程中，你需要为每个场景都添加 XR Manager。

**UI展示**

XR Manager 脚本面板如下：

![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.003.png)

**功能说明**

你可以通过 XR Manager 管理和开启许多 SDK 提供的能力。

|**功能**|**简述**|
| :- | :- |
|身体追踪|身体追踪（Body Tracking）用于收集用户的身体位置和动作信息，并将其转换为可再现的姿态数据。|
|空间锚点|空间锚点技术可以将场景展示位置与灵境空间的位置进行锚定，用于将虚拟场景根据现实的空间大小、屏幕数量进行展示|
|零点过滤|开启零点过滤，将自动筛选丢失的点，并使用上一帧该点的位置参与计算|
**快速开始**

**导入 SDK**

DGXR Unity Integration SDK 是 deepglint 官方基于 Unity XR 提供的开发工具，其中包含开发 DG XR 应用所需的功能、组件、插件和脚本。本文档介绍如何在 Unity Hub 中创建项目，然后在项目中导入 SDK。

**第一步：创建项目**

导入 SDK 前，需要在 Unity Hub 中新建项目。步骤如下：

1. 在 Unity Hub 主页，点击**项目 ->新项目**。

你将进入新建项目页。

![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.004.png)

2. 选择 Universa**3D(URP模版)**。
3. 在 **项目设置** 区域，设置项目名称和存储目录。

`  `提示：项目名称和存储目录不能包含中文字符和数字开头

4. 点击 **创建项目**。

项目初始化完毕后，你将进入 Unity 编辑器页面。

**第二步：导入 DGXR Unity Integration SDK**

你可以从以下方式中任选其一，导入 DGXR Unity Integration SDK 至你的项目。

|**方式**|**步骤**|
| :- | :- |
|导入本地的 SDK 包 |<p>1. 前往deepglint 开发者平台页面，下载最新版本的 SDK。 </p><p>2. 解压所下载的 SDK 压缩包。 你将会得到一个包含 package.json 文件的文件夹。 </p><p>3. 返回 Unity 编辑器页面。 </p><p>4. 在上方菜单栏处，选择 **Windows** > **Package Manager**。 </p><p>5. 在 **Package Manager** 窗口中，点击 **+** > **Add package from disk**。 </p><p>6. 选择 **package.json** 文件并导入。 </p><p>导入完成后，你将看到 **XR SDK Setting** 窗口，关闭即可。 </p>|
|导入 Git URL |<p>1. 打开 Unity 编辑器页面。 </p><p>2. 在上方菜单栏处，选择 Edit > **Project Setting > Package Manager**。 </p><p>3. 在 **Package Manager** 窗口中，点击 **+** > **Add package**。 </p><p>![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.005.png)</p><p>4. 输入仓库的地址，然后点击 **Add**。 </p><p>Unity 编辑器**Windows** > **Package Manager>My registry**。开始从中导入 SDK。 </p>|
**创建一个 XR 场景**

|预计阅读和完成时间：15 分钟 |
| :- |
本文档介绍如何升级 XR Interaction SDK，然后创建一个基础 XR 场景（下图所示）。此外，文档也会介绍玩家绑定、骨骼跟踪相关内容。

![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/DG%20Unity%20Integration%20SDK.006.png)

**第一步：升级 XR Interaction SDK 并导入示例文件**

搭建基础 XR 场景需要用到 Unity 提供的基础功能和组件。升级**XR Interaction SDK 以便**获得 Unity 提供的新资源包和功能。

1. Unity 编辑器**Windows** > **Package Manager>My registry**。
2. 在 **Package Manager** 窗口中，列表中将展示 Unity Registry 中提供的工具包。
3. 在列表中找到 **XR Interaction Toolkit**，并将其展开。
4. 点击 **See other versions** 展开版本列表。
5. 从列表中选择 **2.0.0** 或以上版本，然后点击窗口右下角的 **Update to version\_number** 按钮。
6. 升级完成后，再次前往 **WindowsPackage ManagerXR Interaction SDK**。
7. 展开右侧的 **Samples** 面板。
8. 点击 **Import**，导入示例文件。

|**示例文件**|**说明**|
| :- | :- |
|PlayerManager|该示例文件默认位于 Assets/Samples/XR Interaction SDK/[version]/PlayerManager 目录下，提供了一套标准的玩家绑定示例，包括一套默认的输入动作和预设。 |
|HumanBody |HumanBody ，提供用于空间中的人体跟踪，默认位于 Assets/Samples/XR Interaction SDK/[version]/HumanBody  目录下。该示例文件用于展示空间骨骼是否正常接入。|
**第二步：拖入 XRManager 预制体**

预制体位于 Packages/DGXR Interaction SDK/Prefabs文件夹下

1. 预制体拖入后，会自动在坐标原点创建 灵境空间的场景（项目启动后会自动隐藏）
2. 移动要展示的场景与空间重合，调整场景大小，便于与实际空间物品比例相同。
3. 运行项目，即可根据默认配置文件生成对应屏幕数量的相机渲染内容
4. 在脚本中设置空间视角跟随功能，挂载脚本
```C#
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
        Global.CavePosition = body.Joints.HeadTop;
    }
}
```

**后续操作**

参考《开发》文档，探索更多 DGXR Interaction SDK更多功能！

**开发**

**渲染**

默认情况下预制体会创建一组 3d相机，用于渲染前后左右地五块屏幕。通过设置空间中人的视角位置，动态计算并渲染相机内容输出对应屏幕上。开发者无需关系渲染和相机相关操作和配置，此相机和屏幕根据实际空间大小和安装方式动态创建。

**UI 渲染**

` `TODO 补充文档  @陈新宇

**自定义渲染**

1. 创建自己的 3d 相机拍摄想要渲染的画面
2. 设置相机为 overlay 模式，并添加相机到对应的屏幕上（具体代码如下）
```C#
using Deepglint.XR;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NewBehaviourScript : MonoBehaviour
{
private void Start()
{
Camera camera = new Camera();
camera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
Global.Space.Front.AddCameraToStack(camera);
}
        }
```

3. **交互**

**人体跟踪获取**

使用步骤：

1. 在场景中导入Runtime/Prefabs/DGXRManager预制体
2. 直接获取骨骼列表示例
```c#
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
        var nose = body.Joints.Nose;
    }
}
```
3. 订阅方式使用骨骼示例
```c#
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

**玩家绑定及动作接入**

TODO @张梦豪

**空间音频**

基础功能有播放音频、播放音频列表、停止等

AudioManager的方法不再管理流程，如果想实现播放音频结束后触发某方法的功能，则自己开启一个TimerManager，时间为音频长度 

