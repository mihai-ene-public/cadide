using IDE.Core.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using IDE.Core.Storage;
using System.Collections.ObjectModel;
using IDE.Core.Interfaces;
using IDE.Core;
using System.Collections.Generic;

namespace IDE.Documents.Views
{
    public class SolutionProjectPropertiesViewModel : FileBaseViewModel
    {
        public SolutionProjectPropertiesViewModel() 
            : base(null)
        {
            //Project = project;
            IsDirty = false;

            //var p = new SolutionProjectPropertiesViewModel(project)
            //=p.OpenFile
        }

        public ProjectDocument Project { get; set; }

        public ObservableCollection<PropertyDisplay> Properties { get; set; } = new ObservableCollection<PropertyDisplay>();

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
            return Task.Run(() =>
            {
                Properties.Clear();
                if (Project.Properties.Properties != null)
                    Properties.AddRange(Project.Properties.Properties.Select(p => new PropertyDisplay
                    {
                        Name = p.Name,
                        Value = p.Value,
                        PropertyType = p.Type
                    }));

                //todo: add back this
                //Project.Properties.PropertyChanged += (s, e) => { IsDirty = true; };

                foreach (var p in Properties)
                {
                    p.PropertyChanged += (s, e) => { IsDirty = true; };
                }

                //todo on item added or remove and PropertyChanged

                //force save
                //IsDirty = true;
            });
        }

        protected override void SaveDocumentInternal(string filePath)
        {
            Project.Properties.Properties = Properties.Select(p => new Property
            {
                Name = p.Name,
                Value = p.Value,
                Type = p.PropertyType
            }).ToList();

            Project.Save();
        }

        public override void RegisterDocumentType(IDocumentTypeManager docTypeManager)
        {

        }
    }
}
