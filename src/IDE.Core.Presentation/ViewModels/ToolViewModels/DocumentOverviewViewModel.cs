using System.Collections.ObjectModel;
using System.ComponentModel;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Documents.Views;
using System.Windows.Input;
using IDE.Core.Commands;

namespace IDE.Core.ViewModels
{
    public class DocumentOverviewViewModel : ToolViewModel, IRegisterable, IDocumentToolWindow
    {
        public DocumentOverviewViewModel()
            : base("Overview")
        {
            CanHide = true;
            IsVisible = false;

            SelectedNodes = new ObservableCollection<OverviewSelectNode>();
            SelectedNodes.CollectionChanged += SelectedNodes_CollectionChanged;
        }

        void SelectedNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                //var overviewNode = (OverviewSelectNode)e.NewItems[0];
                foreach (OverviewSelectNode overviewNode in e.NewItems)
                {
                    // overviewNode.Document = Document;
                    overviewNode.OverviewModel = this;
                    overviewNode.IsSelected = true;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (OverviewSelectNode overviewNode in e.OldItems)
                {
                    overviewNode.OverviewModel = this;
                    overviewNode.IsSelected = false;
                }

            }

        }

        //public ObservableCollection<OverviewNode> Categories { get; set; } = new ObservableCollection<OverviewNode>();

        IDocumentOverview document;
        public IDocumentOverview Document
        {
            get { return document; }
            private set
            {
                document = value;
                OnPropertyChanged(nameof(Document));
            }
        }

        ICommand refreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                    refreshCommand = CreateCommand(async p => await Document?.RefreshOverview());

                return refreshCommand;
            }
        }

        public ObservableCollection<OverviewSelectNode> SelectedNodes { get; set; }


        public override PaneLocation PreferredLocation
        {
            get
            {
                return PaneLocation.Left;
            }
        }

        public void RegisterDocumentType(IDocumentTypeManager docTypeManager)
        {

        }

        public void SetDocument(IFileBaseViewModel document)
        {
            Document = document as IDocumentOverview;
        }
    }

    public class OverviewNode : BaseViewModel, IOverviewSelectNode
    {
        public bool BindPropertyName { get; set; } = false;

        protected string displayPropertyName = "Name";
        public string DisplayPropertyName
        {
            set { displayPropertyName = value; }
        }

        protected string formatText = "{0}";
        public string FormatText
        {
            set { formatText = value; }
        }

        protected string name = "unknown";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        bool isExpanded = true;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        public DocumentOverviewViewModel OverviewModel { get; set; }

        public ObservableCollection<OverviewSelectNode> Children { get; set; } = new ObservableCollection<OverviewSelectNode>();


    }

    public class OverviewSelectNode : OverviewNode
    {


        INotifyPropertyChanged dataItem;
        public INotifyPropertyChanged DataItem
        {
            get
            { return dataItem; }
            set
            {
                if (dataItem != null)
                    ((INotifyPropertyChanged)dataItem).PropertyChanged -= DataItem_PropertyChanged;
                dataItem = value;

                if (dataItem != null)
                    ((INotifyPropertyChanged)dataItem).PropertyChanged += DataItem_PropertyChanged;

                OnPropertyChanged(nameof(DataItem));
            }
        }


        bool isSelected;
        public bool IsSelected
        {
            get
            {
                if (dataItem != null)
                {
                    var si = dataItem as ISelectableItem;
                    if (si != null)
                    {
                        return si.IsSelected;
                    }

                }

                return isSelected;
            }
            set
            {
                if (isSelected == value)
                    return;

                isSelected = value;
                if (dataItem != null)
                {
                    switch (dataItem)
                    {
                        case ISelectableItem si:
                            si.IsSelected = value;
                            var canvas = OverviewModel?.Document as CanvasDesignerFileViewModel;
                            var canvasModel = canvas?.CanvasModel;
                            canvasModel?.UpdateSelection();
                            break;

                        case IBoardNetDesignerItem net:
                            net.HighlightNet(isSelected);
                            break;

                        case SchematicNet schematicNet:
                            schematicNet.IsHighlighted = isSelected;
                            break;
                    }
                }

                OnPropertyChanged(nameof(IsSelected));
            }
        }

        void DataItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISelectableItem.IsSelected))
            {
                OnPropertyChanged(nameof(IsSelected));

                if (dataItem is ISelectableItem si && si != null)
                {
                    if (si.IsSelected)
                    {
                        var contains = OverviewModel != null && OverviewModel.SelectedNodes != null && OverviewModel.SelectedNodes.Contains(this);
                        if (!contains)
                            OverviewModel?.SelectedNodes.Add(this);
                    }
                    else
                        OverviewModel?.SelectedNodes.Remove(this);
                }

            }


            OnPropertyChanged(nameof(DisplayText));
        }

        public string DisplayText
        {
            get
            {
                if (DataItem != null)
                {
                    if (BindPropertyName)
                    {
                        var txt = DataItem.GetPropertyValue<string>(displayPropertyName);
                        if (string.IsNullOrEmpty(txt))
                            return dataItem.ToString();
                        return string.Format(formatText, txt);
                    }
                    else return dataItem.ToString();

                }
                return Name;
            }
        }
    }

    public class OverviewFolderNode : OverviewSelectNode
    {

    }


}
