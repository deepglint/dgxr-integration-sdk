# HumanControlInputModule

HumanControlInputModule 是对EventSystem 中原有InputModule的功能扩展，HumanControlInputModule通过获取灵境空间内所有人的骨骼数据，并将人的foot 骨骼点作为与地屏的多点触控点，当人的脚在空间内移动，踩踏，抬腿，拖曳时会触发相应的PointerEnter, PointerExit, PointDrag, PointClick等事件，以简化人与空间中的UI或者3d元素的交互；

`HumanControlFootPointerInputModule`支持的事件：

| **事件类型**     | **功能说明** |
|:-------------|:---------|
| PointerEnter | 进入       |
| PointerExit  | 离开       |
| PointerDown  | 踩下       |
| PointerUp    | 抬起       |
| PointerClick | 点击       |
| Drag         | 拖拽       |
| BeginDrag    | 开始拖拽     |
| EndDrag      | 结束拖拽     |
| Move         | 移动       |

## 运行sample 程序
1. 导入sample ![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/Sample003.jpg);
2. 启用EventSystem上的HumanControlInputModule ![](https://static-1253924368.cos.ap-beijing.myqcloud.com/nebula/doc/images/Sample004.jpg)
3. 安装TextMesh Pro