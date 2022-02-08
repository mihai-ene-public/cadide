using System;
using System.Windows.Input;
using Microsoft.Win32;
using IDE.Core.Importers;
using IDE.Documents.Views;
using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IDE.Core.Storage;

namespace IDE.Core.Presentation.Importers.DXF
{
    //currently the scenario is to load a board outline
    public class DxfImporterViewModel : DialogViewModel
    {
        public DxfImporterViewModel(IBoardDesigner boardModel)
        {
            _boardModel = boardModel;
        }
        
        private readonly IBoardDesigner _boardModel;

        public string WindowTitle
        {
            get
            {
                return "DXF Import";
            }
        }


        private string sourceFile;

        public string SourceFile
        {
            get { return sourceFile; }
            set
            {
                sourceFile = value;
                OnPropertyChanged(nameof(SourceFile));
            }
        }

        public double DefaultLineWidth { get; set; } = 0.2;

        public DXFUnits DxfUnits { get; set; } = DXFUnits.mm;

        public IList<DxfLayerMappingItem> FootprintLayerMapping { get; set; } = new ObservableCollection<DxfLayerMappingItem>();

        //board layers

        //dxf layers

        ICommand selectFileCommand;

        public ICommand SelectFileCommand
        {
            get
            {
                if (selectFileCommand == null)
                    selectFileCommand = CreateCommand(p =>
                    {
                        var dlg = ServiceProvider.Resolve<IOpenFileDialog>();
                        dlg.Multiselect = false;
                        //libraries and projects
                        dlg.Filter = $"DXF files ({DxfImporter.FilesFilter})|{DxfImporter.FilesFilter}"; 
                        if (dlg.ShowDialog() == true)
                        {
                            SourceFile = dlg.FileName;

                            // todo: refresh source layers
                            // todo: refresh preview (if any)
                        }
                    });
                return selectFileCommand;
            }
        }

        public IList<LayerPrimitive> RunImport()
        {
            if (string.IsNullOrEmpty(sourceFile))
                throw new Exception("You must specify the file to convert");


            var importer = new DxfImporter();
            importer.DefaultLineWidth = DefaultLineWidth;
            importer.DxfUnits = DxfUnits;

            var primitives = importer.Import(sourceFile);
            return primitives;
        }
    }
}
