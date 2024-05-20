using System;

namespace Deepglint.Tool.Utils
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