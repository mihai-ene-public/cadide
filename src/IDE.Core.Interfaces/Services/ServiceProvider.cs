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
        static List<IService> services = new List<IService>();

        //public static void RegisterService<T>(IService service, bool replace = true) where T : IService
        //{
        //    //if the service already exists, it is replaced with the new
        //    var existing = services.FirstOrDefault(s => s is T);
        //    if (existing != null)
        //    {
        //        if (replace == false)
        //            return;

        //        services.Remove(existing);
        //    }

        //    services.Add(service);
        //}

        //public static T GetService<T>() where T : IService
        //{
        //    var service = services.FirstOrDefault(s => s is T);
        //    return (T)service;
        //}

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
