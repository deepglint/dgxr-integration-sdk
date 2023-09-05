## 简介
DGCAVE是一款高度定制化的插件，基于格灵深瞳M2硬件相机和先进的人体三维骨骼算法开发而成。它为用户提供了互动式、沉浸式的裸眼3D体验，让您沉浸在虚拟世界中的同时，无需佩戴任何特殊设备。
## 功能

- 实时画面旋转：根据头部位置的变化，实现主视角屏幕拼接处的实时旋转，打造完美的裸眼3D效果。
- 灵活拼接空间：在任意矩形空间中，您可以随意调整画面拼接，实现无缝连接，营造裸眼沉浸式体验。
- 多人空间：支持配置主视角跟随特定用户，让多个参与者在同一个虚拟空间中共享互动体验。
- 交互性：您可以根据不同游戏类型配置插件，跟踪识别特定用户的游戏操作，实现更加自由的交互。
## 使用方法

1. 在Unity中导入DGCAVE插件，将DGCAVE/prefabs/VRWorldManager预制体拖放到自己场景的Hierarchy窗口中
2. 关闭场景中原来的相机，或者将其display属性调整成和cave配置的相机不冲突
3. 在unity编辑器的Edit菜单/ProjectSettings/Player/Other Settings中，勾选Allow unsafe code
4. 在Assets下创建StreamingAssets目录（如果已经有此目录就不用创建了），将calibration.xml文件放入其中。VRWorldManager预制体会根据此文件的配置去生成相机
### 锁定视角
目前简单实现了一个锁定XZ轴和锁定全部轴的功能
```csharp
private void SetHeadPosition()
{
    headLockPosition = _head.transform.position;
    if (LockAll)
    {
        headLockPosition = centerViewPoint;
    } else if (LockXZ)
    {
        LockValue = _head.transform.position;
        headLockPosition = new Vector3(transform.position.x, _head.transform.position.y, transform.position.z);
    }

    foreach (var userCamera in _userProjectorViewCameras) {
        userCamera.transform.position = headLockPosition;
    }


    foreach (var userCamera in _userScreenViewCameras)
        userCamera.transform.position = headLockPosition;

}
```


### 空间中人员的3dPose数据获取
1.引入BodySource命名空间
2.引入_bodyManager实例（为单例，可放心引用）
3.获取数据

比如下面例子是获取空间中每一个人的左右脚位置
```
using BodySource;

...

ConcurrentDictionary<string, BodyDataSource> personBodySource = VRDGBodySource.Instance.GetData();

foreach (KeyValuePair<string, BodyDataSource> player in personBodySource)
{
    BodyDataSource person = personBodySource[player.Key];
    JointData LeftFootJointData = person.Joints[JointType.LeftHeel];
    JointData RightFootJointData = person.Joints[JointType.RightHeel];
}
```



### 配置cave主视角
1.引入BodySource命名空间
```
using BodySource;
```

2.引入_bodyManager实例（为单例，可放心引用）
```
 _bodyManager = VRDGBodySource.Instance;
```

3.设置cavePersonId的视角
```
_bodyManager.SetCavePersonId(personId) //personId获取步骤参考3dpos的接入
```


### 配置游戏事件识别
1.引入BodySource命名空间
```
using BodySource;
```

2.引入_bodyManager实例（为单例，可放心引用）
```
 _bodyManager = VRDGBodySource.Instance;
```

3.设置运行的游戏和识别人的ID
```
// personId 获取步骤参考3dpos的接入
// skiing 代表滑雪
_bodyManager.SetGameInfo("skiing",personId)
```


## Coding建议
在VRWorldManager中有一些基础的相机配置，如果需要进行更改相机配置等操作时，不要直接更改Cave包中的代码，而是将脚本中的东西暴露出来，在自己的脚本中进行修改。


## 支持与联系

- 如有任何问题或反馈，请联系我们的技术支持团队：Alpha Space。
- 更多信息，请访问我们的官方网站：https://deepglint.com/。

