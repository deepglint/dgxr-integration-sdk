# DGXR
### 骨骼接入
#### 使用步骤：
- 1. 在场景中导入Runtime/Prefabs/DGXRManager预制体
- 2. 直接获取骨骼列表示例
```
using Runtime.Scripts;

public class Demo
{
    void Start(){
       if (Source.Data.Count==0)
        {
            return;
        }
        var body = Source.Data[0];
        var nose = body.Joints.Nose;
    }
}
```
- 3. 订阅方式使用骨骼示例
```
using Runtime.Scripts;
using UnityEngine;

public class Demo : MonoBehaviour
{
    // 订阅MetaPoseDataReceived事件的方法
    private void OnEnable()
    {
         Global.OnMetaPoseDataReceived += HandleMetaPoseDataReceived;
    }

    // 在禁用对象时取消订阅事件
    private void OnDisable()
    {
         Global.OnMetaPoseDataReceived -= HandleMetaPoseDataReceived;
    }

    // 当接收到MetaPose数据时被调用的方法
    private void HandleMetaPoseDataReceived(Source.SourceData data)
    {
        // 在这里编写处理MetaPose数据的逻辑
        Debug.Log("Received MetaPose Data: " + data.ToString());
    }
}
```
