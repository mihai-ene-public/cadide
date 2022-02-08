using IDE.Core.Interfaces;

namespace IDE.Core
{
    public static class DocumentHelper
    {
        public static T GetCurrentDocument<T>() where T : class
        {
            var app = ServiceProvider.Resolve<IApplicationViewModel>();//ServiceProvider.GetService<IApplicationViewModel>();
            if (app != null)
            {
                var layeredDoc = app.ActiveDocument as T;
                return layeredDoc;
            }
            return default(T);
        }
    }
}
