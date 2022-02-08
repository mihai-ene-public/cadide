using IDE.Core;
using IDE.Core.Build;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class BoardBuildOptionsAssemblyViewModel : BaseViewModel
    {
        public BoardBuildOptionsAssemblyViewModel(BoardDesignerFileViewModel board)
        {
            innerBoard = board;

            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();

            PropertyChanged += ViewModel_PropertyChanged;
        }



        BoardDesignerFileViewModel innerBoard;

        IDispatcherHelper dispatcher;

        public bool IsOutputAssemblyEditable => true;

        public ObservableCollection<AssemblyOutputColumn> PickAndPlaceColumns { get; set; } = new ObservableCollection<AssemblyOutputColumn>();
        public ObservableCollection<AssemblyDrawingOutputLayer> DrawingLayers { get; set; } = new ObservableCollection<AssemblyDrawingOutputLayer>();

        DynamicList previewItems;
        public DynamicList PreviewItems
        {
            get { return previewItems; }
            set
            {
                previewItems = value;
                OnPropertyChanged(nameof(PreviewItems));
            }
        }

        OutputUnits positionUnits;
        public OutputUnits PositionUnits
        {
            get
            {
                return positionUnits;
            }
            set
            {
                positionUnits = value;
                OnPropertyChanged(nameof(PositionUnits));
            }
        }

        string fieldSeparator;
        public string FieldSeparator
        {
            get
            {
                return fieldSeparator;
            }
            set
            {
                fieldSeparator = value;
                OnPropertyChanged(nameof(FieldSeparator));
            }
        }

        ICommand moveAssyOutColumnDownCommand;

        public ICommand MoveAssyOutColumnDownCommand
        {
            get
            {
                if (moveAssyOutColumnDownCommand == null)
                    moveAssyOutColumnDownCommand = CreateCommand((selectedItem) =>
                    {
                        PickAndPlaceColumns.MoveDown((AssemblyOutputColumn)selectedItem);
                    });

                return moveAssyOutColumnDownCommand;
            }
        }

        ICommand moveAssyOutColumnUpCommand;

        public ICommand MoveAssyOutColumnUpCommand
        {
            get
            {
                if (moveAssyOutColumnUpCommand == null)
                    moveAssyOutColumnUpCommand = CreateCommand((selectedItem) =>
                    {
                        PickAndPlaceColumns.MoveUp((AssemblyOutputColumn)selectedItem);
                    });

                return moveAssyOutColumnUpCommand;
            }
        }

        public void LoadFrom(AssemblyOutputSpec assySpec)
        {
            PickAndPlaceColumns.Clear();

            assySpec = CreateDefaultAssyOutputSpec(assySpec);

            PickAndPlaceColumns.AddRange(assySpec.PickAndPlace.Columns);
            fieldSeparator = assySpec.PickAndPlace.Separator;
            positionUnits = assySpec.PickAndPlace.Units;


            LoadDrawingLayers(assySpec);

            AttachHandlers();
        }

        void LoadDrawingLayers(AssemblyOutputSpec assySpec)
        {
            var candidateLayers = GetCandidateLayers();

            var layers = new List<AssemblyDrawingOutputLayer>();
            foreach (var candidateLayer in candidateLayers)
            {
                var plotLayer = assySpec.Drawings.Layers.FirstOrDefault(l => l.LayerId == candidateLayer.LayerId);

                layers.Add(new AssemblyDrawingOutputLayer
                {
                    LayerId = candidateLayer.LayerId,
                    Layer = candidateLayer,
                    Plot = plotLayer != null && plotLayer.Plot
                });
            }

            DrawingLayers.AddRange(layers);
        }

        void AttachHandlers()
        {
            //detach
            PickAndPlaceColumns.CollectionChanged -= Columns_CollectionChanged;

            foreach (var c in PickAndPlaceColumns)
                c.PropertyChanged -= Column_PropertyChanged;


            //attach
            PickAndPlaceColumns.CollectionChanged += Columns_CollectionChanged;

            foreach (var c in PickAndPlaceColumns)
                c.PropertyChanged += Column_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PositionUnits):
                //case nameof(FieldSeparator):
                    await GenerateOutput();
                    break;
            }
        }

        private async void Column_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await GenerateOutput();
        }

        private async void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            await GenerateOutput();
        }

        public void SaveTo(AssemblyOutputSpec assySpec)
        {
            assySpec.PickAndPlace.Columns = PickAndPlaceColumns.ToList();
            assySpec.PickAndPlace.Separator = FieldSeparator;
            assySpec.PickAndPlace.Units = PositionUnits;

            assySpec.Drawings.Layers = DrawingLayers.Where(l => l.Plot).ToList();
        }

        async Task GenerateOutput()
        {
            var assyHelper = new AssemblyPickAndPlaceHelper();
            var list = await assyHelper.GetOutputData(innerBoard, PickAndPlaceColumns);

            dispatcher.RunOnDispatcher(() => PreviewItems = list);
        }

        AssemblyOutputSpec CreateDefaultAssyOutputSpec(AssemblyOutputSpec assemblySpec)
        {
            if (assemblySpec == null)
                assemblySpec = new AssemblyOutputSpec();

            if (assemblySpec.PickAndPlace.Columns.Count == 0)
            {
                assemblySpec.PickAndPlace.Columns = new List<AssemblyOutputColumn>
                {
                    new AssemblyOutputColumn { ColumnName = nameof(AssemblyPickAndPlaceItemDisplay.PartName)},
                    new AssemblyOutputColumn { ColumnName = nameof(AssemblyPickAndPlaceItemDisplay.Layer)},
                    new AssemblyOutputColumn { ColumnName = nameof(AssemblyPickAndPlaceItemDisplay.Footprint)},
                    new AssemblyOutputColumn { ColumnName = nameof(AssemblyPickAndPlaceItemDisplay.CenterX)},
                    new AssemblyOutputColumn { ColumnName = nameof(AssemblyPickAndPlaceItemDisplay.CenterY)},
                    new AssemblyOutputColumn { ColumnName = nameof(AssemblyPickAndPlaceItemDisplay.Rot)},
                };
            }

            //drawing layers
            if (assemblySpec.Drawings.Layers.Count == 0)
            {
                var candidateLayers = GetCandidateLayers();

                assemblySpec.Drawings.Layers = candidateLayers.Select(l =>
                      new AssemblyDrawingOutputLayer
                      {
                          Plot = true,
                          LayerId = l.LayerId,
                          Layer = l
                      }
                  ).ToList();
            }

            return assemblySpec;
        }

        IList<ILayerDesignerItem> GetCandidateLayers()
        {
            var candidateLayers = new List<ILayerDesignerItem>();

            var validLayerTypes = new[] {
                                         LayerType.SilkScreen,
                                         LayerType.Mechanical,
                                         LayerType.Generic
                                        };

            var layers = innerBoard.LayerItems.Where(l => validLayerTypes.Contains(l.LayerType)).ToList();

            var topLayers = layers.Where(l => l.IsTopLayer).ToList();
            var bottomLayers = layers.Where(l => l.IsBottomLayer).ToList();

            foreach (var layerPair in innerBoard.BoardProperties.LayerPairs)
            {
                var layerTop = layers.FirstOrDefault(l => l.LayerId == layerPair.LayerStart?.LayerId);
                var layerBottom = layers.FirstOrDefault(l => l.LayerId == layerPair.LayerEnd?.LayerId);

                if (layerTop != null)
                {
                    topLayers.Add(layerTop);
                }

                if (layerBottom != null)
                {
                    bottomLayers.Add(layerBottom);
                }
            }

            candidateLayers.AddRange(topLayers);
            candidateLayers.AddRange(bottomLayers);

            return candidateLayers;
        }

        public void PreviewAssembly()
        {
            GenerateOutput().ConfigureAwait(false);
        }

    }
}
