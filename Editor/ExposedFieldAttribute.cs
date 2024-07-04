using System;

namespace Darkan.Editor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ExposedFieldAttribute : Attribute
    {
        public ExposedFieldAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public readonly string DisplayName;
    }
}
