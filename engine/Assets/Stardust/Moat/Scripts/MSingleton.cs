using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSingleton<T>
    where T : class, new()
{
    private static T sInstance = null;
    public static T Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = new T();
            }
            return sInstance;
        }
    }
    public static void Dispose()
    {
        sInstance = null;
    }
    
    // 添加构造函数
    protected MSingleton()
    {
        Initialize();
    }
    
    protected virtual void Initialize()
    {
    }
}