using System;

namespace Deepglint.XR.Toolkit.UIFrame
{
    /// <summary>
    /// 阈值件加载规则
    /// </summary>
    public enum PathRule
    {
        /// <summary>
        /// 空的模块预制件，模块创建时创建一个空的 gameObject 节点
        /// </summary>
        Empty,
        
        /// <summary>
        /// 指定模块预制件路径，需要同时设置 Path 属性
        /// </summary>
        Specified,
        
        /// <summary>
        /// 通过代码的名称空间层级规则到对应的文件夹目录中找到同名的预制件， 规则为 `Assets/Scripts/Scene` 对应到 `Assets/Resources/Prefabs` 目录
        /// </summary>
        NamespaceHierarchy,
    }

    /// <summary>
    /// 该 Attribute 用于标记 BaseModule/UIModule 的阈值件加载方式
    /// </summary>
    public class PrefabInfoAttribute : Attribute
    {
        /// <summary>
        /// 预制件路径
        /// </summary>
        public string Path { get; }
        
        /// <summary>
        /// 阈值件加载规则
        /// </summary>
        public PathRule Rule { get; }

        
        /// <summary>
        /// 通过自定义路径加载预制件，会自动将 PathRule 设置为 `Specified`
        /// </summary>
        /// <param name="path">预制件路径</param>
        public PrefabInfoAttribute(string path)
        {
            Rule = PathRule.Specified;
            Path = path;
        }

        /// <summary>
        /// 设置预制件加载规则
        /// </summary>
        /// <param name="rule">加载规则</param>
        public PrefabInfoAttribute(PathRule rule)
        {
            Rule = rule;
        }
    }
}