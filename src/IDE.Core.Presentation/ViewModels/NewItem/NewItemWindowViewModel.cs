using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class NewItemWindowViewModel : DialogViewModel
    {
        public NewItemWindowViewModel()
        {
            //Templates = new ObservableCollection<TemplateItem>();
            //TemplateGroups = new ObservableCollection<TemplateGroupDisplayItem>();
        }

        string solutionName;
        public string SolutionName
        {
            get { return solutionName; }
            set
            {
                solutionName = value;
                OnPropertyChanged(nameof(SolutionName));
            }
        }

        bool createSolutionFolder;
        public bool CreateSolutionFolder
        {
            get
            {
                return createSolutionFolder;
            }
            set
            {
                createSolutionFolder = value;
                OnPropertyChanged(nameof(CreateSolutionFolder));
            }
        }

        string itemName;
        public string ItemName
        {
            get { return itemName; }
            set
            {
                itemName = value;
                OnPropertyChanged(nameof(ItemName));
            }
        }

        string location;
        /// <summary>
        /// the path folder where to copy the template item
        /// </summary>
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public string WindowTitle
        {
            get
            {
                var title = "New Item";

                switch (templateType)
                {
                    case TemplateType.Solution:
                    case TemplateType.Project:
                    case TemplateType.SampleProject:
                        {
                            title = "New Project";
                            break;
                        }

                    case TemplateType.Symbol:
                    case TemplateType.Footprint:
                    case TemplateType.Model:
                    case TemplateType.Component:
                    case TemplateType.Schematic:
                    case TemplateType.Board:
                        title = $"New {templateType}";
                        break;
                }

                return title;
            }
        }

        public string SelectedItemFilePath { get; private set; }

        ICommand browseLocationCommand;
        public ICommand BrowseLocationCommand
        {
            get
            {
                if (browseLocationCommand == null)
                    browseLocationCommand = CreateCommand(
                        p =>
                        {
                            //var folderBrowserDialog = new FolderSelectDialog();
                            var folderBrowserDialog = ServiceProvider.Resolve<IFolderSelectDialog>();
                            folderBrowserDialog.Title = "Project Location";
                            folderBrowserDialog.InitialDirectory = Location;

                            if (folderBrowserDialog.ShowDialog())
                            {
                                Location = folderBrowserDialog.FileName;
                            }
                        });


                return browseLocationCommand;
            }
        }

        public ObservableCollection<TemplateItem> Templates { get; set; } = new ObservableCollection<TemplateItem>();

        //this will be updated by TemplateType property
        //public ObservableCollection<TemplateGroupDisplayItem> TemplateGroups
        //{
        //    get; set;
        //}

        //TemplateGroupDisplayItem selectedGroup;
        //public TemplateGroupDisplayItem SelectedGroup
        //{
        //    get
        //    {
        //        return selectedGroup;
        //    }
        //    set
        //    {
        //        selectedGroup = value;

        //        LoadTemplatesByGroup();

        //        OnPropertyChanged(nameof(SelectedGroup));
        //    }
        //}

        TemplateItem selectedTemplate;
        public TemplateItem SelectedTemplate
        {
            get
            {
                return selectedTemplate;
            }
            set
            {
                selectedTemplate = value;
                OnPropertyChanged(nameof(SelectedTemplate));
            }
        }

        TemplateType templateType;
        public TemplateType TemplateType
        {
            get
            {
                return templateType;
            }
            set
            {
                templateType = value;
                // IsSolution = templateType == TemplateType.Solution;

                // OnPropertyChanged(nameof(IsSolution));
                OnPropertyChanged(nameof(WindowTitle));

                LoadTemplateGroupsByType();
            }
        }

        bool isSolution;
        public bool IsSolution
        {
            get { return isSolution; }
            set
            {
                isSolution = value;
                OnPropertyChanged(nameof(IsSolution));
            }
        }

        void LoadTemplateGroupsByType()
        {
            // TemplateGroups.Clear();

            //todo: we will have a some folders from which we can load the templates. This will be local startup folder, Common Documents, and others
            var folders = new[] { "Templates" };

            switch (templateType)
            {
                case TemplateType.Solution:
                case TemplateType.Project:
                case TemplateType.SampleProject:
                    {
                        LoadGroupsFromFolders(folders, "Projects");
                        break;
                    }

                case TemplateType.Symbol:
                case TemplateType.Footprint:
                case TemplateType.Model:
                case TemplateType.Component:
                case TemplateType.Schematic:
                case TemplateType.Board:
                case TemplateType.Misc:
                    {
                        LoadGroupsFromFolders(folders, "Items");
                        break;
                    }
            }

            // SelectedGroup = TemplateGroups.FirstOrDefault();
        }

        private void LoadGroupsFromFolders(string[] folders, string subFolder)
        {
            Templates.Clear();

            foreach (var folder in folders)
            {
                //Projects folder
                foreach (var projRootDir in Directory.GetDirectories(folder, subFolder))
                {
                    foreach (var groupDir in Directory.GetDirectories(projRootDir))
                    {
                        LoadTemplatesByFolder(groupDir);
                    }
                }
            }

            SelectedTemplate = Templates.FirstOrDefault();
        }

        void LoadTemplatesByFolder(string folderPath)
        {
            //Templates.Clear();
            //if (SelectedGroup == null)
            //    return;

            foreach (var templateFolder in Directory.GetDirectories(folderPath))
            {
                //take the 1st template file
                var templateFile = Directory.GetFiles(templateFolder, "*.template").FirstOrDefault();
                if (!string.IsNullOrEmpty(templateFile))
                {
                    //deserialize it; ignore the errors
                    try
                    {
                        var templateItem = XmlHelper.Load<TemplateItem>(templateFile);
                        templateItem.FilePath = templateFile;

                        //if we create a new solution, we add search for projects
                        var type = templateType;
                        if (type == TemplateType.Solution)// || type == TemplateType.SampleProject)
                            type = TemplateType.Project;

                        if (type == templateItem.TemplateType)
                            Templates.Add(templateItem);
                    }
                    catch { }
                }
            }

            //SelectedTemplate = Templates.FirstOrDefault();
        }

        public void CreateItem()
        {
            //validation
            if (string.IsNullOrEmpty(ItemName))
                throw new Exception("Name of the item was not set");
            if (string.IsNullOrEmpty(Location))
                throw new Exception("Item location was not set");
            if (SelectedTemplate == null)
                throw new Exception("A template must be selected");

            string itemFilePath = null;

            if (templateType == TemplateType.Project
                || templateType == TemplateType.Solution
                || templateType == TemplateType.SampleProject
                )
            {
                var projLocation = CreateSolutionFolder ? Path.Combine(Location, SolutionName) : Location;
                var solName = SolutionName;//CreateSolutionFolder ? SolutionName : ItemName;
                var projName = ItemName;

                //if (templateType == TemplateType.Solution)
                if (IsSolution)
                {
                    //create the solution
                    //SolutionManager.CreateNewSolution(projName);
                    var solution = new SolutionDocument();
                    solution.Children = new List<ProjectBaseFileRef>
                    {
                         new SolutionProjectItem
                         {
                             RelativePath = $@"{projName}/{projName}{ProjectDocument.ProjectExtension}"
                         },
                    };

                    Directory.CreateDirectory(projLocation);
                    var solPathFile = Path.Combine(projLocation, solName + "." + SolutionDocument.SolutionExtension);

                    //todo: check if we already have a solution in this folder
                    XmlHelper.Save(solution, solPathFile);
                    SelectedItemFilePath = solPathFile;
                }

                //create folder for project
                var projPath = Path.Combine(projLocation, projName);
                Directory.CreateDirectory(projPath);

                //we will add the extension on copy
                itemFilePath = Path.Combine(projPath, projName);

                if (templateType == TemplateType.Project)// || templateType == TemplateType.SampleProject)
                {
                    SelectedItemFilePath = itemFilePath + ProjectDocument.ProjectExtension;
                }
            }
            else
            {
                itemFilePath = Path.Combine(Location, ItemName);

                SelectedItemFilePath = $"{itemFilePath}.{SelectedTemplate.Extension}";
            }

            //copy the files (except .template) from template folder to Location
            //todo: use recursive for folders
            //bug: for more files, copies files with the same name as item name from window
            var templateFolder = Path.GetDirectoryName(SelectedTemplate.FilePath);
            var templateExtension = Path.GetExtension(SelectedTemplate.FilePath);
            foreach (var templateFile in Directory.GetFiles(templateFolder))
            {
                var templateFileExtension = Path.GetExtension(templateFile);
                if (templateFileExtension != templateExtension)
                {
                    var destFile = itemFilePath + templateFileExtension;
                    if (IsProjectTemplate() && Path.GetExtension(destFile) != ProjectDocument.ProjectExtension)
                    {
                        //we just copy the same file with the same extension
                        var fn = Path.GetFileName(templateFile);
                        var destFolder = Path.GetDirectoryName(itemFilePath);

                        destFile = Path.Combine(destFolder, fn);
                    }
                    File.Copy(templateFile, destFile);
                }
            }

            //copy folders recusively
            var destProjFolder = Path.GetDirectoryName(itemFilePath);
            foreach (var templateSubFolder in Directory.GetDirectories(templateFolder))
            {
                var destFolder = Path.Combine(destProjFolder, Path.GetFileName(templateSubFolder));

                Extensions.CopyDirectory(templateSubFolder, destFolder, false);
            }

        }


        bool IsProjectTemplate()
        {
            return templateType == TemplateType.Project || templateType == TemplateType.SampleProject;
        }
    }
}
