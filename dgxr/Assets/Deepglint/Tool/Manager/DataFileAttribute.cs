using System;

namespace Deepglint.Tool.Manager
{
    public class DataFileAttribute : Attribute
    {
        public string Name { get; }

        public DataFileAttribute(string name)
        {
            Name = name;
        }
    }
}