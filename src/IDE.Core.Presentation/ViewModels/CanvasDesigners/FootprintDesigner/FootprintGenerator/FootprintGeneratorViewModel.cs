using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class FootprintGeneratorViewModel : BaseViewModel
    {

        public event Action Close;
        public FootprintGeneratorViewModel(IDrawingViewModel canvas)
        {
            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            canvasModel = canvas;
            var doc = canvasModel.FileDocument as ILayeredViewModel;

            originalItems = canvasModel.GetItems().ToList();

            PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(ShowList))
                    return;

                if (CurrentFootprintGenerator != null)
                {
                    var newitems = await CurrentFootprintGenerator.GenerateFootprint();

                    if (newitems != null)
                    {
                        dispatcher.RunOnDispatcher(() =>
                        {

                            canvasModel.RemoveItems(canvasModel.GetItems().ToList());

                            foreach (BoardCanvasItemViewModel brdItem in newitems)
                            {
                                brdItem.LayerDocument = doc;
                                brdItem.LoadLayers();
                                canvasModel.AddItem(brdItem);
                            }

                            //  canvasModel.AddItems(newitems);
                        });
                    }
                }
            };
        }

        IDrawingViewModel canvasModel;

        IDispatcherHelper dispatcher;

        List<ISelectableItem> originalItems;

        public bool ShowList => CurrentFootprintGenerator == null;

        FootprintGenerator currentFootprintGenerator;
        public FootprintGenerator CurrentFootprintGenerator
        {
            get { return currentFootprintGenerator; }
            set
            {
                currentFootprintGenerator = value;
                OnPropertyChanged(nameof(CurrentFootprintGenerator));
                OnPropertyChanged(nameof(ShowList));
            }
        }

        List<FootprintGenerator> footprintGenerators;
        public List<FootprintGenerator> FootprintGenerators
        {
            get
            {
                if (footprintGenerators == null)
                {
                    var doc = canvasModel.FileDocument as ILayeredViewModel;

                    if (doc != null)
                    {
                        footprintGenerators = new List<FootprintGenerator>
                                    {
                                        new AxialFootprintGenerator(doc),
                                        new BGAFootprintGenerator(doc),
                                        new ChipFootprintGenerator(doc),
                                        new CrystalSMDFootprintGenerator(doc),
                                        new DIPFootprintGenerator(doc),
                                        new DFNFootprintGenerator(doc),
                                        new ECapFootprintGenerator(doc),
                                        new SODFootprintGenerator(doc),
                                        new SOICFootprintGenerator(doc),
                                        new SOT23FootprintGenerator(doc),
                                        new SOT223FootprintGenerator(doc),
                                        new QFPFootprintGenerator(doc),
                                        new QFNFootprintGenerator(doc),
                                        new PinHeaderStraightFootprintGenerator(doc),
                                        new RadialGenericRoundFootprintGenerator(doc),
                                        new RadialLEDFootprintGenerator(doc),
                                    };
                    }

                    foreach (var pg in footprintGenerators)
                    {
                        pg.PropertyChanged += (s, e) => OnPropertyChanged("Footprint");
                    }
                }

                return footprintGenerators;
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
                        if (p is FootprintGenerator pg)
                            CurrentFootprintGenerator = pg;
                    });

                }

                return selectGeneratorCommand;
            }
        }

        ICommand backCommand;

        public ICommand BackCommand
        {
            get
            {
                if (backCommand == null)
                {
                    backCommand = CreateCommand(p =>
                    {
                        CurrentFootprintGenerator = null;
                    });

                }

                return backCommand;
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
                        var doc = canvasModel.FileDocument as ILayeredViewModel;

                        //put back original items
                        canvasModel.RemoveItems(canvasModel.GetItems().ToList());

                        foreach (BoardCanvasItemViewModel brdItem in originalItems)
                        {
                            brdItem.LayerDocument = doc;
                            brdItem.LoadLayers();
                            canvasModel.AddItem(brdItem);
                        }

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
