using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Common.Utilities
{
    //this will be more likely replaced with AutoMapper
    public class GenericMapper
    {
        public GenericMapper()
        {
            CreateMappings();
        }



        protected Dictionary<Type, Type> typeMappings = new Dictionary<Type, Type>();

        protected virtual void CreateMappings()
        {
        }

        public void AddMapping(Type firstType, Type secondType)
        {
            if (typeMappings.ContainsKey(firstType))
                return;

            typeMappings.Add(firstType, secondType);
        }

        public Type GetMapping(Type type)
        {
            if (typeMappings.TryGetValue(type, out Type mappedType))
                return mappedType;

            return null;
        }
    }
}
