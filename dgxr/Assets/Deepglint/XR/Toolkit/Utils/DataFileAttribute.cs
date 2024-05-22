using System;

namespace Deepglint.XR.Toolkit.Utils
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