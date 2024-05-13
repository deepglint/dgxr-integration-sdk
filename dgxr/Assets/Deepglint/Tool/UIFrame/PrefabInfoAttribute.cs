using System;

namespace Deepglint.Tool.UIFrame
{
    public enum PathRule
    {
        Empty,
        Specified,
        NamespaceHierarchy,
    }

    public class PrefabInfoAttribute : Attribute
    {
        public string Path { get; }
        public PathRule Rule { get; }

        public PrefabInfoAttribute(string path)
        {
            Rule = PathRule.Specified;
            Path = path;
        }

        public PrefabInfoAttribute(PathRule rule)
        {
            Rule = rule;
        }
    }
}