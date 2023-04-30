using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class ParametricPackageViewModel : BaseViewModel
    {
        public event Action Close;
        public ParametricPackageViewModel(ICanvasDesignerFileViewModel canvas)
        {
            canvasModel = canvas;
            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();

            originalItems = canvasModel.Items.ToList();

            PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(CurrentItem))
                {
                    if (CurrentItem != null)
                    {
                        var packageItem = (ParametricPackageMeshItem)Activator.CreateInstance(currentItem.Type);
                        await packageItem.GenerateItems();

                        dispatcher.RunOnDispatcher(() =>
                           {
                               canvasModel.RemoveItems(canvasModel.Items.ToList());
                               canvasModel.AddItem(packageItem);
                           });
                    }
                }
            };
        }

        protected IDispatcherHelper dispatcher;
        ICanvasDesignerFileViewModel canvasModel;

        List<ISelectableItem> originalItems;


        ParametricPackageToolboxItem currentItem;
        public ParametricPackageToolboxItem CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                OnPropertyChanged(nameof(CurrentItem));
            }
        }

        List<ParametricPackageToolboxItem> packageItems;
        public List<ParametricPackageToolboxItem> PackageItems
        {
            get
            {
                if (packageItems == null)
                {
                    packageItems = new List<ParametricPackageToolboxItem>
                                    {
                                        new ParametricPackageToolboxItem{  Name="Axial", Type=typeof(AxialParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="BGA", Type=typeof(BGAParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="Chip", Type=typeof(ChipParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="Crystal SMD", Type=typeof(CrystalSMDParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="DIP", Type=typeof(DIPParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="DFN", Type=typeof(DFNParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="D-Pak", Type=typeof(DPakParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="E-Cap", Type=typeof(ECapParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="MELF", Type=typeof(MelfParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="SOD", Type=typeof(SODParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="SMA", Type=typeof(SMAParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="SOIC / SOP", Type=typeof(SOICParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="SOT23", Type=typeof(SOT23ParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="SOT223", Type=typeof(SOT223ParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="QFN", Type=typeof(QFNParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="QFP", Type=typeof(QFPParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="Pin Header - Straight", Type=typeof(PinHeaderStraightParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="Radial (Round)", Type=typeof(RadialGenericRoundParametricPackageMeshItem)},
                                        new ParametricPackageToolboxItem{  Name="Radial LED", Type=typeof(RadialLEDParametricPackageMeshItem)},
                                    };

                }

                return packageItems;
            }
        }

        ICommand selectGeneratorCommand;

        public ICommand SelectGeneratorCommand
        {
            get
            {
                if (selectGeneratorCommand == null)
                {
                    selectGeneratorCommand = CreateCommand(p =>
                    {
                        if (p is ParametricPackageToolboxItem pg)
                            CurrentItem = pg;
                    });

                }

                return selectGeneratorCommand;
            }
        }

        //OKCommand
        ICommand okCommand;

        public ICommand OKCommand
        {
            get
            {
                if (okCommand == null)
                {
                    okCommand = CreateCommand(p =>
                    {
                        var currentItems = canvasModel.Items.ToList();

                        canvasModel.RegisterUndoActionExecuted(
                            undo: o =>
                            {
                                canvasModel.RemoveItems(currentItems);
                                canvasModel.AddItems(originalItems);
                                return null;
                            },
                            redo: o =>
                            {
                                canvasModel.RemoveItems(canvasModel.Items.ToList());
                                canvasModel.AddItems(currentItems);
                                return null;
                            }, null);
                            

                        OnClose();
                    });

                }

                return okCommand;
            }
        }

        //CancelCommand
        ICommand cancelCommand;

        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {
                    cancelCommand = CreateCommand(p =>
                    {
                        //put back original items
                        canvasModel.RemoveItems(canvasModel.Items.ToList());

                        canvasModel.AddItems(originalItems);

                        OnClose();
                    });

                }

                return cancelCommand;
            }
        }

        void OnClose()
        {
            Close?.Invoke();
        }
    }
}
