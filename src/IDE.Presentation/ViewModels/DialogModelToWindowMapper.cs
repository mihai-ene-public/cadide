using IDE.Core.Common.Utilities;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Storage;
using IDE.Core.Designers;
using System.Windows;
using IDE.Dialogs.About;
using IDE.Documents.Views;
using IDE.Core.Presentation;
using IDE.Presentation.Views.ImporterDialogs;
using IDE.Core.Presentation.Importers.DXF;

namespace IDE.Core.ViewModels
{
    public class DialogModelToWindowMapper : GenericMapper, IDialogModelToWindowMapper
    {
        public DialogModelToWindowMapper()
        {
        }


        protected override void CreateMappings()
        {
            AddMapping(typeof(AboutViewModel), typeof(AboutDlg));

            AddMapping(typeof(EagleImporterViewModel), typeof(EagleImporterView));
            AddMapping(typeof(DxfImporterViewModel), typeof(DxfImporterView));

            AddMapping(typeof(AddReferencesDialogViewModel), typeof(AddReferencesDialog));
            //AddMapping(typeof(BomSearchViewModel), typeof(BomSearchView));
            AddMapping(typeof(ItemSelectDialogViewModel), typeof(ItemSelectDialog));
            AddMapping(typeof(NewItemWindowViewModel), typeof(NewItemWindow));
            AddMapping(typeof(SettingsDialogViewModel), typeof(SettingsDialog));

            AddMapping(typeof(PackageManagerDialogViewModel), typeof(PackageManagerDialog));
        }

        public IWindow GetWindow(IDialogViewModel viewModel)
        {
            var mappedType = GetMapping(viewModel.GetType());
            if (mappedType != null)
            {
                var window = Activator.CreateInstance(mappedType) as IWindow;

                if (window != null)
                {
                    var win = window as Window;
                    if (win != null)
                    {
                        var mainWindow = Application.Current.MainWindow;
                        win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        win.Owner = mainWindow;
                    }

                    return window;
                }
            }

            throw new NotSupportedException();
        }
    }
}
