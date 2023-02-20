using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace IDE.Documents.Views;

public class NewItemWindowViewModel : DialogViewModel
{
    public NewItemWindowViewModel()
    {
        _foldersTemplateRepo = new FoldersTemplateRepository();
        _builtInTemplateRepository = new BuiltInTemplateRepository();
    }

    private readonly IFoldersTemplateRepository _foldersTemplateRepo;
    private readonly IBuiltInTemplateRepository _builtInTemplateRepository;

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

    public IList<TemplateItemInfo> Templates { get; set; } = new ObservableCollection<TemplateItemInfo>();

    TemplateItemInfo selectedTemplate;
    public TemplateItemInfo SelectedTemplate
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

            OnPropertyChanged(nameof(WindowTitle));
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
            var solName = SolutionName;
            var projName = ItemName;

            if (IsSolution)
            {
                //create the solution
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

            if (templateType == TemplateType.Project)
            {
                SelectedItemFilePath = itemFilePath + ProjectDocument.ProjectExtension;
            }
        }
        else
        {
            itemFilePath = Path.Combine(Location, ItemName);

            SelectedItemFilePath = $"{itemFilePath}.{SelectedTemplate.TemplateItem.Extension}";
        }

        switch(selectedTemplate)
        {
            case FolderTemplateItemInfo folderTemplateItemInfo:
                _foldersTemplateRepo.CreateItemFromTemplate(folderTemplateItemInfo.TemplateFilePath, itemFilePath, IsProjectTemplate());
                break;

            case BuiltInTemplateItemInfo builtInTemplateItemInfo:
                _builtInTemplateRepository.CreateItemFromTemplate(builtInTemplateItemInfo, itemFilePath);
                break;
        }

    }


    bool IsProjectTemplate()
    {
        return templateType == TemplateType.Project || templateType == TemplateType.SampleProject;
    }

    protected override Task LoadData(object args)
    {

        Templates.AddRange(_foldersTemplateRepo.LoadTemplates(templateType));
        Templates.AddRange(_builtInTemplateRepository.LoadTemplates(templateType));

        SelectedTemplate = Templates.FirstOrDefault();

        return Task.CompletedTask;
    }
}
