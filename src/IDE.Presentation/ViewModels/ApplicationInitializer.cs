using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using IDE.Core.Resources;
using System;
using System.Linq;
using IDE.Core.ViewModels;
using IDE.Core.Settings;
using IDE.Core.Documents;
using IDE.Documents.Views;
using IDE.Core.Utilities;
using IDE.Core.Interfaces;
using IDE.Core;

namespace IDE.Documents.Module
{
    public class ApplicationInitializer
    {

        public static void Initialize()
        {
            //var avLayout = ServiceProvider.Resolve<IAvalonDockLayoutViewModel>();
            //RegisterStyles(avLayout);

            RegisterEditorModels();

            RegisterServices();

            MoreStuff();
        }

        //TODO: DocumentTypeManager will become a singleton
        //every file opener will register itself 

        static IApplicationViewModel applicationViewModel() => ApplicationServices.ApplicationViewModel;

        static void RegisterEditorModels()//IAvalonDockLayoutViewModel avLayout)
        {
            //there is the possibility to load all these using MEF, but for now we can use this

            var toolRegistry = ApplicationServices.ToolRegistry;
            var documentTypeManager = ApplicationServices.DocumentTypeManager;

            var registerables = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from t in assembly.GetTypes()
                                 where t.GetInterfaces().Contains(typeof(IRegisterable))
                                       && t.GetConstructor(Type.EmptyTypes) != null
                                 select t).ToList();
            foreach (var r in registerables)
            {
                try
                {
                    var instance = (IRegisterable)Activator.CreateInstance(r);

                    instance.RegisterDocumentType(documentTypeManager);

                    if (instance is ToolViewModel)
                    {
                        var t = instance as ToolViewModel;
                        t.IsVisible = false;
                        toolRegistry.RegisterTool(t);
                    }

                }
                catch //(Exception ex)
                {
                    //log these in Output window or with log4net
                }

            }
        }

        static void MoreStuff()
        {
            var appCore = ApplicationServices.AppCoreModel;
            //var settingsManager = ServiceProvider.SettingsManager;
            //var themesManager = ServiceProvider.ThemesManager;

            // Setup location of config files
            //settingsManager.AppDir = appCore.DirAppData;
            //settingsManager.LayoutFileName = appCore.LayoutFileName;

            //todo:put back
            applicationViewModel().LoadConfig();

            appCore.CreateAppDataFolder();
        }

        static void RegisterServices()
        {
            var registerables = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from t in assembly.GetTypes()
                                 where t.GetInterfaces().Contains(typeof(IService))
                                       && t.GetConstructor(Type.EmptyTypes) != null
                                 select t).ToList();
            foreach (var r in registerables)
            {
                try
                {
                    //it is enough to create the service; it will register by itself
                    var instance = (IService)Activator.CreateInstance(r);
                }
                catch //(Exception ex)
                {
                    //log these in Output window or with log4net
                }

            }
        }
    }
}
