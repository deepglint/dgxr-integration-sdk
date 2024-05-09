using System;

namespace Runtime.Scripts.Tools.Manager
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