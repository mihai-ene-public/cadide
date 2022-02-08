using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class MarksDirtyAttribute : Attribute
    {
        //public MarksDirtyAttribute()
        //{

        //}
        public MarksDirtyAttribute(bool marksDirty = true)
        {
            MarksDirty = marksDirty;
        }

        public bool MarksDirty { get; set; } = true;
    }
}
