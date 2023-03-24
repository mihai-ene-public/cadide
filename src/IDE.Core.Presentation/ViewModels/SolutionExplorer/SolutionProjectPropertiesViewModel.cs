using IDE.Core.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using IDE.Core.Storage;
using System.Collections.ObjectModel;
using IDE.Core.Interfaces;
using IDE.Core;
using System.Collections.Generic;

namespace IDE.Documents.Views;

public class SolutionProjectPropertiesViewModel : FileBaseViewModel, ISolutionProjectPropertiesDocument
{
    public SolutionProjectPropertiesViewModel()
    {
        IsDirty = false;
    }


    public ObservableCollection<PropertyDisplay> Properties { get; set; } = new ObservableCollection<PropertyDisplay>();

    public PackagePropertiesViewModel PackageProperties { get; set; } = new PackagePropertiesViewModel();

    public override bool CanClose()
    {
        return true;
    }

    public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
    {
        return new List<IDocumentToolWindow>();
    }

    protected override Task LoadDocumentInternal(string filePath)
    {
        var project = XmlHelper.Load<ProjectDocument>(filePath);

        var properties = new List<PropertyDisplay>();

        if (project.Properties.Properties != null)
        {

            properties.AddRange(project.Properties.Properties.Select(p => new PropertyDisplay
            {
                Name = p.Name,
                Value = p.Value,
                PropertyType = p.Type
            }));

            foreach (var p in properties)
            {
                p.PropertyChanged += (s, e) => { IsDirty = true; };
            }
        }

        Properties = new ObservableCollection<PropertyDisplay>(properties);

        PackageProperties.LoadFrom(project.Package);

        return Task.CompletedTask;
    }

    protected override void SaveDocumentInternal(string filePath)
    {
        var project = XmlHelper.Load<ProjectDocument>(filePath);
        project.Properties.Properties = Properties.Select(p => new Property
        {
            Name = p.Name,
            Value = p.Value,
            Type = p.PropertyType
        }).ToList();

        project.Package = PackageProperties.ToPackageMetadata();

        XmlHelper.Save(project, filePath);
    }

}

public class SolutionPropertiesViewModel : FileBaseViewModel
{
    public SolutionPropertiesViewModel()
    {
        IsDirty = false;
    }

    public PackagePropertiesViewModel PackageProperties { get; set; } = new PackagePropertiesViewModel();

    public override bool CanClose()
    {
        return true;
    }

    public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
    {
        return new List<IDocumentToolWindow>();
    }

    protected override Task LoadDocumentInternal(string filePath)
    {
        var solution = XmlHelper.Load<SolutionDocument>(filePath);

        PackageProperties.LoadFrom(solution.Package);

        return Task.CompletedTask;
    }

    protected override void SaveDocumentInternal(string filePath)
    {
        var solution = XmlHelper.Load<SolutionDocument>(filePath);

        solution.Package = PackageProperties.ToPackageMetadata();

        XmlHelper.Save(solution, filePath);
    }
}
