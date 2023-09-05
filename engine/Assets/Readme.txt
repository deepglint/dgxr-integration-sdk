# DGCAVE - 沉浸式裸眼3D通用插件

## 简介

DGCAVE是一款高度定制化的插件，基于格灵深瞳M2硬件相机和先进的人体三维骨骼算法开发而成。它为用户提供了互动式、沉浸式的裸眼3D体验，让您沉浸在虚拟世界中的同时，无需佩戴任何特殊设备。

## 引用方式

- 在Unity中导入DGCAVE插件并将VRWorldManager预制体拖放到场景中。
- 调整VRWorldManager预制体中HEAD对象的位置，以确保主视角屏幕拼接处的实时变换。

## 功能

- 实时画面旋转：根据头部位置的变化，实现主视角屏幕拼接处的实时旋转，打造完美的裸眼3D效果。
- 灵活拼接空间：在任意矩形空间中，您可以随意调整画面拼接，实现无缝连接，营造裸眼沉浸式体验。
- 多人空间：支持配置主视角跟随特定用户，让多个参与者在同一个虚拟空间中共享互动体验。
- 交互性：您可以根据不同游戏类型配置插件，跟踪识别特定用户的游戏操作，实现更加自由的交互。

## 使用方法

1. 在Unity中导入DGCAVE插件，将VRWorldManager预制体拖放到场景中的合适位置。
2. 调整VRWorldManager预制体中HEAD对象的位置，确保画面拼接处变换时的效果最佳。
3. 在项目中设置适合您的游戏类型和交互方式，享受沉浸式裸眼3D的全新体验！

### 空间中人员的3dPose数据获取

1.引入BodySource命名空间

```
using BodySource;
```

2.引入_bodyManager实例（为单例，可放心引用）

``` 
 _bodyManager = VRDGBodySource.Instance;
```

3.获取数据

```
var data = _bodyManager.GetData()
// data 为字典，key为人员id，valve为这个人的信息（包含人员id，是否跟踪，3dpos等信息）
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
//personId获取步骤参考3dpos的接入
//skiing 代表滑雪
_bodyManager.SetGameInfo("skiing",personId) 
```

## 支持与联系

- 如有任何问题或反馈，请联系我们的技术支持团队：Alpha Space。
- 更多信息，请访问我们的官方网站：https://deepglint.com/。
