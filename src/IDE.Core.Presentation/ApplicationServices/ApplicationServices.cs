using IDE.Core.Compilation;
using IDE.Core.Documents;
using IDE.Core.Interfaces;
using IDE.Core.MRU;
using IDE.Core.ViewModels;
using System.Diagnostics;

namespace IDE.Core.Utilities
{

    /// <summary>
    /// class to register services (interfaces) common and unique to the application (they act like singletons)
    /// maybe this is not the best way, but until we understand how MEF really works
    /// This is the alternative on not to have an [ImportingConstructor]
    /// </summary>
    public static class ApplicationServices
    {

        static ActiveCompiler activeCompiler;
        public static ActiveCompiler ActiveCompiler
        {
            get
            {
                if (activeCompiler == null)
                    activeCompiler = new ActiveCompiler();

                return activeCompiler;
            }
        }

        public static ISettingsManager SettingsManager
        {
            get
            {
               return ServiceProvider.Resolve<ISettingsManager>();
            }
        }

        public static IThemesManager ThemesManager
        {
            get
            {
                return ServiceProvider.Resolve<IThemesManager>();
            }
        }

        //static RecentFilesModel recentFiles;
        //public static RecentFilesModel RecentFiles
        //{
        //    get
        //    {
        //        if (recentFiles == null)
        //            recentFiles = new RecentFilesModel();

        //        return recentFiles;
        //    }
        //}

        static IToolWindowRegistry toolRegistry;

        public static IToolWindowRegistry ToolRegistry
        {
            get
            {
                if (toolRegistry == null)
                    toolRegistry = new ToolWindowRegistry();

                return toolRegistry;
            }
        }

        static IDocumentTypeManager documentTypeManager;

        public static IDocumentTypeManager DocumentTypeManager
        {
            get
            {
                if (documentTypeManager == null)
                    documentTypeManager = new DocumentTypeManager();

                return documentTypeManager;
            }
        }

        static IAppCoreModel appCoreModel;
        public static IAppCoreModel AppCoreModel
        {
            get
            {
                if (appCoreModel == null)
                    appCoreModel = new AppCoreModel();

                return appCoreModel;
            }
        }

        public static IApplicationViewModel ApplicationViewModel
        {
            get
            {
                return ServiceProvider.Resolve<IApplicationViewModel>();
            }
        }
    }
}
