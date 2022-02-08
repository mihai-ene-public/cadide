using IDE.Core.Commands;
using System;
using System.Windows.Input;
using Microsoft.Win32;
using IDE.Core.Importers;
using IDE.Documents.Views;
using IDE.Core.Interfaces;

namespace IDE.Core.Presentation
{
    public class EagleImporterViewModel : DialogViewModel
    {
        public string WindowTitle
        {
            get
            {
                return "Eagle Converter";
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

        private string destinationFolder;

        public string DestinationFolder
        {
            get { return destinationFolder; }
            set
            {
                destinationFolder = value;
                OnPropertyChanged(nameof(DestinationFolder));
            }
        }



        ICommand selectFileCommand;

        public ICommand SelectFileCommand
        {
            get
            {
                if (selectFileCommand == null)
                    selectFileCommand = CreateCommand(p =>
                      {
                          var dlg = ServiceProvider.Resolve<IOpenFileDialog>();//new OpenFileDialog();
                          dlg.Multiselect = false;
                          //libraries and projects
                          dlg.Filter = $"EAGLE files ({EagleImporter.FilesFilter})|{EagleImporter.FilesFilter}"; //$"EAGLE files (*{EagleImporter.libraryExtension};*{EagleImporter.boardExtension})|*{EagleImporter.libraryExtension};*{EagleImporter.boardExtension}";
                          if (dlg.ShowDialog() == true)
                          {
                              SourceFile = dlg.FileName;
                          }
                      });
                return selectFileCommand;
            }
        }

        ICommand selectSolutionFolderCommand;

        public ICommand SelectSolutionFolderCommand
        {
            get
            {
                if (selectSolutionFolderCommand == null)
                    selectSolutionFolderCommand = CreateCommand(p =>
                    {
                        //var folderBrowserDialog = new FolderSelectDialog();
                        var folderBrowserDialog = ServiceProvider.Resolve<IFolderSelectDialog>();
                        folderBrowserDialog.Title = "Destination folder";
                        if (folderBrowserDialog.ShowDialog() == true)
                        {
                            DestinationFolder = folderBrowserDialog.FileName;
                        }
                    });
                return selectSolutionFolderCommand;
            }
        }


        public void RunImport()
        {
            if (string.IsNullOrEmpty(sourceFile))
                throw new Exception("You must specify the file to convert");

            if (string.IsNullOrEmpty(destinationFolder))
                throw new Exception("You must specify the destination folder");

            var importer = new EagleImporter();
            importer.Import(sourceFile, destinationFolder);
        }

    }
}
