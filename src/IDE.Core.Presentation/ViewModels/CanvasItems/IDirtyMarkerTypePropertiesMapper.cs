using IDE.Core.Interfaces;
using IDE.Core.Types.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IDE.Core.Designers
{
    public interface IDirtyMarkerTypePropertiesMapper
    {
        /// <summary>
        /// returns the list of property names in a type that will mark a document as dirty
        /// </summary>
        /// <param name="canvasItemType"></param>
        /// <returns></returns>
        IList<string> GetPropertyNames(object targetObject);
    }

    public class DirtyMarkerTypePropertiesMapper : IDirtyMarkerTypePropertiesMapper
    {
        private Dictionary<Type, IList<string>> types = new Dictionary<Type, IList<string>>();

        public IList<string> GetPropertyNames(object targetObject)
        {
            var type = targetObject.GetType();

            if (types.TryGetValue(type, out var list))
            {
                return list;
            }

            list = (from p in type.GetProperties()
                    where p.GetCustomAttribute<MarksDirtyAttribute>() != null
                    select p.Name)
                    .ToList();

            types[type] = list;

            return list;
        }
    }
}
