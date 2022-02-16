using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core
{

    /// <summary>
    /// class to register services (interfaces) common and unique to the application (they act like singletons)
    /// maybe this is not the best way, but until we understand how MEF really works
    /// This is the alternative on not to have an [ImportingConstructor]
    /// </summary>
    public static class ServiceProvider
    {
        public static T GetToolWindow<T>(bool throwException = true) where T : IToolWindow
        {
            var toolRegistry = Resolve<IToolWindowRegistry>();
            if (toolRegistry != null)
                return (T)toolRegistry.Tools.FirstOrDefault(t => t is T);

            if (throwException)
                throw new NotSupportedException();

            return default(T);
        }

        static Func<Type, object> containerResolver;

        public static void RegisterResolver(Func<Type, object> resolver)
        {
            containerResolver = resolver;
        }

        public static T Resolve<T>()
        {
            var r = (T)containerResolver(typeof(T));
            return r;
        }
    }

}
