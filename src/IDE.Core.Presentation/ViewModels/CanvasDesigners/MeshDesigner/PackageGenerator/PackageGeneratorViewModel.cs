using IDE.Core;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace IDE.Documents.Views
{


    public class PackageGeneratorViewModel : BaseViewModel
    {
        public event Action Close;
        public PackageGeneratorViewModel(IDrawingViewModel canvas)
        {
            canvasModel = canvas;
            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();

            originalItems = canvasModel.Items.ToList();

            PropertyChanged += async (s, e) =>
            {
                if (CurrentPackageGenerator != null)
                {
                    var meshItems = await CurrentPackageGenerator.GeneratePackage();

                    if (meshItems != null)
                    {
                        dispatcher.RunOnDispatcher(() =>
                        {

                            canvasModel.RemoveItems(canvasModel.Items.ToList());

                            canvasModel.AddItems(meshItems);
                        });
                    }
                }
            };
        }

        protected IDispatcherHelper dispatcher;
        IDrawingViewModel canvasModel;

        List<ISelectableItem> originalItems;

        public bool ShowList => CurrentPackageGenerator == null;

        PackageGenerator currentPackageGenerator;
        public PackageGenerator CurrentPackageGenerator
        {
            get { return currentPackageGenerator; }
            set
            {
                currentPackageGenerator = value;
                OnPropertyChanged(nameof(CurrentPackageGenerator));
                OnPropertyChanged(nameof(ShowList));
            }
        }

        List<PackageGenerator> packageGenerators;
        public List<PackageGenerator> PackageGenerators
        {
            get
            {
                if (packageGenerators == null)
                {
                    packageGenerators = new List<PackageGenerator>
                                    {
                                        new AxialPackageGenerator(),
                                        new BGAPackageGenerator(),
                                        new ChipPackageGenerator(),
                                        new CrystalSMDPackageGenerator(),
                                        new DIPPackageGenerator(),
                                        new DFNPackageGenerator(),
                                        new DPakPackageGenerator(),
                                        new ECapPackageGenerator(),
                                        new MelfPackageGenerator(),
                                        new SODPackageGenerator(),
                                        new SMAPackageGenerator(),
                                        new SOICPackageGenerator(),
                                        new SOT23PackageGenerator(),
                                        new SOT223PackageGenerator(),
                                        new QFNPackageGenerator(),
                                        new QFPPackageGenerator(),
                                        new PinHeaderStraightPackageGenerator(),
                                        new RadialGenericRoundPackageGenerator(),
                                        new RadialLEDPackageGenerator(),
                                    };

                    foreach (var pg in packageGenerators)
                    {
                        pg.PropertyChanged += (s, e) => OnPropertyChanged("Package");
                    }
                }

                return packageGenerators;
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
                          if (p is PackageGenerator pg)
                              CurrentPackageGenerator = pg;
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
                        CurrentPackageGenerator = null;
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
