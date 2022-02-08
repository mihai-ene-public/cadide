using IDE.Core;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class BoardBuildOptionsBomViewModel : BaseViewModel
    {
        public BoardBuildOptionsBomViewModel(BoardDesignerFileViewModel board)
        {
            innerBoard = board;

            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
        }

        BoardDesignerFileViewModel innerBoard;

        IDispatcherHelper dispatcher;

        //public BomOutputSpec BomSpec { get; set; } = new BomOutputSpec();

        public ObservableCollection<BomOutputColumn> Columns { get; set; } = new ObservableCollection<BomOutputColumn>();

        public ObservableCollection<BomOutputColumn> GroupColumns { get; set; } = new ObservableCollection<BomOutputColumn>();


        //List<CustomType> bomPreviewItems = new List<CustomType>();
        DynamicList bomPreviewItems;
        public DynamicList BomPreviewItems
        {
            get { return bomPreviewItems; }
            set
            {
                bomPreviewItems = value;
                OnPropertyChanged(nameof(BomPreviewItems));
            }
        }


        ICommand moveBomOutColumnDownCommand;

        public ICommand MoveBomOutColumnDownCommand
        {
            get
            {
                if (moveBomOutColumnDownCommand == null)
                    moveBomOutColumnDownCommand = CreateCommand((selectedItem) =>
                      {
                          Columns.MoveDown((BomOutputColumn)selectedItem);
                      });

                return moveBomOutColumnDownCommand;
            }
        }

        ICommand moveBomOutColumnUpCommand;

        public ICommand MoveBomOutColumnUpCommand
        {
            get
            {
                if (moveBomOutColumnUpCommand == null)
                    moveBomOutColumnUpCommand = CreateCommand((selectedItem) =>
                    {
                        Columns.MoveUp((BomOutputColumn)selectedItem);
                    });

                return moveBomOutColumnUpCommand;
            }
        }

        ICommand moveBomOutGroupDownCommand;

        public ICommand MoveBomOutGroupDownCommand
        {
            get
            {
                if (moveBomOutGroupDownCommand == null)
                    moveBomOutGroupDownCommand = CreateCommand((selectedItem) =>
                    {
                        GroupColumns.MoveDown((BomOutputColumn)selectedItem);
                    });

                return moveBomOutGroupDownCommand;
            }
        }

        ICommand moveBomOutGroupUpCommand;

        public ICommand MoveBomOutGroupUpCommand
        {
            get
            {
                if (moveBomOutGroupUpCommand == null)
                    moveBomOutGroupUpCommand = CreateCommand((selectedItem) =>
                    {
                        GroupColumns.MoveUp((BomOutputColumn)selectedItem);
                    });

                return moveBomOutGroupUpCommand;
            }
        }

        ICommand addBomOutputGroupCommand;

        public ICommand AddBomOutputGroupCommand
        {
            get
            {
                if (addBomOutputGroupCommand == null)
                    addBomOutputGroupCommand = CreateCommand((p) =>
                    {
                        var selectedItems = p as IList;
                        if (selectedItems == null)
                            return;

                        foreach (BomOutputColumn col in selectedItems)
                        {
                            var exists = GroupColumns.Any(c => c.ColumnName == col.ColumnName);
                            if (!exists)
                                GroupColumns.Add(col);
                        }
                    });

                return addBomOutputGroupCommand;
            }
        }

        ICommand removeBomOutputGroupCommand;

        public ICommand RemoveBomOutputGroupCommand
        {
            get
            {
                if (removeBomOutputGroupCommand == null)
                    removeBomOutputGroupCommand = CreateCommand((p) =>
                    {
                        var selectedItems = p as IList;
                        if (selectedItems == null)
                            return;

                        foreach (var col in selectedItems.Cast<BomOutputColumn>().ToList())
                        {
                            GroupColumns.Remove(col);
                        }
                    });

                return removeBomOutputGroupCommand;
            }
        }

        public void LoadFrom(BomOutputSpec bomSpec)
        {
            Columns.Clear();
            GroupColumns.Clear();

            if (bomSpec == null || bomSpec.Columns.Count == 0)
                bomSpec = CreateDefaultBomOutputSpec();

            Columns.AddRange(bomSpec.Columns);
            GroupColumns.AddRange(bomSpec.GroupColumns);

            AttachHandlers();
        }

        void AttachHandlers()
        {
            //detach
            Columns.CollectionChanged -= Columns_CollectionChanged;
            GroupColumns.CollectionChanged -= Columns_CollectionChanged;

            foreach (var c in Columns)
                c.PropertyChanged -= Column_PropertyChanged;


            //attach
            Columns.CollectionChanged += Columns_CollectionChanged;
            GroupColumns.CollectionChanged += Columns_CollectionChanged;

            foreach (var c in Columns)
                c.PropertyChanged += Column_PropertyChanged;
        }

        private async void Column_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await GenerateOutput();
        }

        private async void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await GenerateOutput();
        }

        public void SaveTo(BomOutputSpec bomSpec)
        {
            bomSpec.Columns = Columns.ToList();
            bomSpec.GroupColumns = GroupColumns.ToList();
        }

        async Task GenerateOutput()
        {
            var bomHelper = new BomHelper();
            var list = await bomHelper.GetOutputData(innerBoard, Columns, GroupColumns);

            dispatcher.RunOnDispatcher(() => BomPreviewItems = list);
        }

        BomOutputSpec CreateDefaultBomOutputSpec()
        {
            return new BomOutputSpec
            {
                Columns = new List<BomOutputColumn>
                {
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Comment)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Component)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.PartName)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Supplier)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Sku)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Manufacturer)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.MPN)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Packaging)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Package)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Stock)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Quantity)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Price)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Currency)},
                },

                GroupColumns = new List<BomOutputColumn>
                {
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Comment)},
                    new BomOutputColumn{ ColumnName=nameof(PartBomOutputItemDisplay.Component)},
                }
            };
        }

        public void PreviewBom()
        {
            GenerateOutput().ConfigureAwait(false);
        }
    }
}
