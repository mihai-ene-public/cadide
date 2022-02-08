using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core
{
    public class TypeActivator
    {


        public static T CreateInstanceByTypeName<T>(string typeName)
        {
            var type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from t in assembly.GetTypes()
                        where t.Name == typeName || t.FullName == typeName
                        select t).FirstOrDefault();

            if (type != null)
            {
                var instance = Activator.CreateInstance(type);

                if (instance is T)
                    return (T)instance;
            }

            return default(T);
        }
    }

    
}
