using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace IDE.Documents.Views
{

    public class BoardPropertiesViewModel : BaseViewModel
    {
        public BoardPropertiesViewModel(BoardDesignerFileViewModel _board)
        {
            board = _board;
            buildOptions = new BoardBuildOptionsViewModel(board);

            ReloadLayers();
        }


        void LayerItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == 0)//added
                OnPropertyChanged(nameof(StackupTotalThickness));
        }


        public PartsBOMViewModel PartsBOMViewModel { get; set; } = new PartsBOMViewModel();

        #region General

        BoardDesignerFileViewModel board;

        public BoardDesignerFileViewModel Board => board;

        Units boardUnits;
        public Units BoardUnits
        {
            get { return boardUnits; }
            set
            {
                boardUnits = value;
                OnPropertyChanged(nameof(BoardUnits));
            }
        }

        string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        SchematicRef schematicReference;
        public SchematicRef SchematicReference
        {
            get { return schematicReference; }
            set
            {
                schematicReference = value;
                OnPropertyChanged(nameof(SchematicReference));
            }
        }

        #region SetSchematicReferenceCommand

        ICommand setSchematicReferenceCommand;
        public ICommand SetSchematicReferenceCommand
        {
            get
            {
                if (setSchematicReferenceCommand == null)
                    setSchematicReferenceCommand = CreateCommand(p => SetSchematicReference());
                return setSchematicReferenceCommand;
            }
        }

        public bool IsUpdateBoardFromSchematicRequired { get; private set; }

        void SetSchematicReference()
        {
            //show list of schematics
            var itemSelectDlg = new ItemSelectDialogViewModel();
            itemSelectDlg.TemplateType = TemplateType.Schematic;
            itemSelectDlg.ProjectModel = ParentProject;
            if (itemSelectDlg.ShowDialog() == true)
            {
                var selectedSchematic = itemSelectDlg.SelectedItem as LibraryItemDisplay;
                if (selectedSchematic != null)
                {
                    //update schematic ref
                    var sch = selectedSchematic.Document as LibraryItem;
                    SchematicReference = new SchematicRef
                    {
                        schematicId = sch.Id,
                        hintPath = sch.FoundPath
                    };

                    //this will be read in the handler
                    IsUpdateBoardFromSchematicRequired = true;
                    OnPropertyChanged(nameof(SchematicReference));
                    //reset
                    IsUpdateBoardFromSchematicRequired = false;
                }
            }


        }
        #endregion

        #endregion

        #region Layer Stack Manager


        ICommand layersAddDrillPairCommand;
        public ICommand LayersAddDrillPairCommand
        {
            get
            {
                if (layersAddDrillPairCommand == null)
                {
                    layersAddDrillPairCommand = CreateCommand(p =>
                      {
                          DrillPairs.Add(new LayerPairModel());
                      });
                }

                return layersAddDrillPairCommand;
            }
        }

        ICommand layersAddLayerPairCommand;
        public ICommand LayersAddLayerPairCommand
        {
            get
            {
                if (layersAddLayerPairCommand == null)
                {
                    layersAddLayerPairCommand = CreateCommand(p =>
                    {
                        LayerPairs.Add(new LayerPairModel());
                    });
                }

                return layersAddLayerPairCommand;
            }
        }

        ICommand addLayerCommand;
        public ICommand AddLayerCommand
        {
            get
            {
                if (addLayerCommand == null)
                {
                    addLayerCommand = CreateCommand(p =>
                    {
                        //RunOnDispatcher(() =>
                        //{
                        try
                        {
                            if (p == null)
                                return;
                            var layerType = LayerType.Unknown;
                            if (Enum.TryParse(p.ToString(), out layerType))
                            {
                                var layer = new LayerDesignerItem(board)
                                {
                                    LayerType = layerType,
                                    LayerName = "New layer",
                                    LayerColor = XColors.Blue,
                                };
                                layer.CreateLayerId(board.LayerItems);

                                layer.PropertyChanged += Layer_PropertyChanged;
                                try { board.LayerItems.Add(layer); } catch { }


                                // StackLayers.Refresh();
                                OnPropertyChanged(nameof(StackLayers));
                                OnPropertyChanged(nameof(StackupTotalThickness));
                            }
                        }
                        catch { }
                        //finally { StackLayers.Refresh(); }
                    });


                    // });
                }

                return addLayerCommand;
            }
        }



        ICommand moveLayerDownCommand;
        public ICommand MoveLayerDownCommand
        {
            get
            {
                if (moveLayerDownCommand == null)
                    moveLayerDownCommand = CreateCommand(p =>
                    {
                        if (selectedStackLayer != null)
                        {
                            var oldIndex = board.LayerItems.IndexOf(selectedStackLayer);
                            var newIndex = oldIndex + 1;
                            if (newIndex < board.LayerItems.Count)
                            {
                                try { MoveLayers(oldIndex, newIndex); } catch { }
                                //stackLayersView.Refresh();
                                OnPropertyChanged(nameof(StackLayers));

                                // SynchronizationContext.Current.Send(x => board.LayerItems.Move(oldIndex, newIndex), null);
                                //if (layerDisplay == LayerStackupSpec.Stackup)
                                //    OnPropertyChanged(nameof(StackLayers));
                            }
                        }
                    });

                return moveLayerDownCommand;
            }

        }

        void MoveLayers(int oldIndex, int newIndex)
        {
            ((ObservableCollection<ILayerDesignerItem>)board.LayerItems).Move(oldIndex, newIndex);
        }

        ICommand moveLayerUpCommand;
        public ICommand MoveLayerUpCommand
        {
            get
            {
                if (moveLayerUpCommand == null)
                    moveLayerUpCommand = CreateCommand(p =>
                    {
                        if (selectedStackLayer != null)
                        {
                            var oldIndex = board.LayerItems.IndexOf(selectedStackLayer);
                            var newIndex = oldIndex - 1;
                            if (newIndex >= 0)
                            {
                                try { MoveLayers(oldIndex, newIndex); } catch { }
                                //stackLayersView.Refresh();

                                OnPropertyChanged(nameof(StackLayers));
                            }

                        }
                    });

                return moveLayerUpCommand;
            }

        }

        ICommand exportLayersCommand;
        public ICommand ExportLayersCommand
        {
            get
            {
                if (exportLayersCommand == null)
                    exportLayersCommand = CreateCommand(p =>
                    {
                        var dlg = ServiceProvider.Resolve<ISaveFileDialog>();
                        dlg.FileName = "board-layers.board";

                        dlg.Filter = "Boards (*.board)|*.board";

                        if (dlg.ShowDialog() == true)     // SaveAs file if user OK'ed it so
                        {
                            var filePath = dlg.FileName;

                            var brd = new BoardDocument();
                            var brdSaver = new BoardSaver();
                            brdSaver.SaveLayers(board, brd);

                            XmlHelper.Save(brd, filePath);
                        }
                    });

                return exportLayersCommand;
            }
        }

        ICommand importLayersCommand;
        public ICommand ImportLayersCommand
        {
            get
            {
                if (importLayersCommand == null)
                    importLayersCommand = CreateCommand(p =>
                    {
                        var res = MessageDialog.Show(
@"This operation might remove items from already existing items.
It is highly recommended to do this operation in the beginning, when very little work was done on board.

Do you really want to continue?", "Warning", XMessageBoxButton.YesNo);

                        if (res != XMessageBoxResult.Yes)
                            return;


                        var dlg = ServiceProvider.Resolve<IOpenFileDialog>();
                        dlg.Multiselect = false;

                        dlg.Filter = "Boards (*.board)|*.board";

                        if (dlg.ShowDialog() == true)     
                        {
                            var filePath = dlg.FileName;

                            var currentCanvasItems = board.CanvasModel.GetItems().ToList();

                            var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
                            var brd = XmlHelper.Load<BoardDocument>(filePath);
                            var brdLoader = new BoardLoader(dispatcher, ParentProject);
                            brdLoader.LoadLayers(brd, board);

                            //re-assign the new layers
                            dispatcher.RunOnDispatcher(() =>
                            {
                                foreach (BoardCanvasItemViewModel item in currentCanvasItems)
                                {
                                    item.LoadLayers();
                                }
                            });
                        }
                    });

                return importLayersCommand;
            }
        }

        //
        ICommand exportRulesCommand;
        public ICommand ExportRulesCommand
        {
            get
            {
                if (exportRulesCommand == null)
                    exportRulesCommand = CreateCommand(p =>
                    {
                        var dlg = ServiceProvider.Resolve<ISaveFileDialog>();
                        dlg.FileName = "board-rules.board";

                        dlg.Filter = "Boards (*.board)|*.board";

                        if (dlg.ShowDialog() == true)     // SaveAs file if user OK'ed it so
                        {
                            var filePath = dlg.FileName;

                            var brd = new BoardDocument();
                            var brdSaver = new BoardSaver();
                            brdSaver.SaveRules(board, brd);

                            XmlHelper.Save(brd, filePath);
                        }
                    });

                return exportRulesCommand;
            }
        }

        ICommand importRulesCommand;
        public ICommand ImportRulesCommand
        {
            get
            {
                if (importRulesCommand == null)
                    importRulesCommand = CreateCommand(p =>
                    {
                        var res = MessageDialog.Show(
@"Existing rules will be lost

Do you really want to continue?", "Warning", XMessageBoxButton.YesNo);

                        if (res != XMessageBoxResult.Yes)
                            return;


                        var dlg = ServiceProvider.Resolve<IOpenFileDialog>();
                        dlg.Multiselect = false;

                        dlg.Filter = "Boards (*.board)|*.board";

                        if (dlg.ShowDialog() == true)     // SaveAs file if user OK'ed it so
                        {
                            var filePath = dlg.FileName;

                            //var currentCanvasItems = board.CanvasModel.GetItems().ToList();

                            var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
                            var brd = XmlHelper.Load<BoardDocument>(filePath);
                            var brdLoader = new BoardLoader(dispatcher, ParentProject);
                            brdLoader.LoadRules(brd, board);

                            ////re-assign the new layers
                            //dispatcher.RunOnDispatcher(() =>
                            //{
                            //    foreach (BoardCanvasItemViewModel item in currentCanvasItems)
                            //    {
                            //        item.LoadLayers();
                            //    }
                            //});
                        }
                    });

                return importRulesCommand;
            }
        }

        void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LayerDesignerItem.Thickness):
                    OnPropertyChanged(nameof(StackupTotalThickness));
                    break;
            }
        }

        ICommand deleteLayerCommand;
        public ICommand DeleteLayerCommand
        {
            get
            {
                if (deleteLayerCommand == null)
                    deleteLayerCommand = CreateCommand(p =>
                      {
                          var layer = p as LayerDesignerItem;
                          if (layer != null)
                          {
                              if (MessageDialog.Show("Are you sure you want to delete this layer?",
                                                    "Confirm delete",
                                                    XMessageBoxButton.YesNo) == XMessageBoxResult.Yes)
                              {
                                  try { board.LayerItems.Remove(layer); } catch { }
                                  layer.PropertyChanged -= Layer_PropertyChanged;
                                  OnPropertyChanged(nameof(StackLayers));
                                  OnPropertyChanged(nameof(StackupTotalThickness));
                              }
                          }
                      },
                      p =>
                      {
                          var layer = p as LayerDesignerItem;
                          if (layer != null)
                          {
                              var cannotDelete = new[] { LayerConstants.SignalTopLayerId,
                                                         LayerConstants.SignalBottomLayerId,
                                                         LayerConstants.SilkscreenTopLayerId,
                                                         LayerConstants.SilkscreenBottomLayerId,
                                                         LayerConstants.SolderTopLayerId,
                                                         LayerConstants.SolderBottomLayerId,
                                                        };
                              return !cannotDelete.Contains(layer.LayerId);
                          }
                          return false;
                      });

                return deleteLayerCommand;
            }
        }

        LayerStackupSpec layerDisplay = LayerStackupSpec.All;
        public LayerStackupSpec LayerDisplay
        {
            get { return layerDisplay; }
            set
            {
                layerDisplay = value;
                OnPropertyChanged(nameof(LayerDisplay));
                OnPropertyChanged(nameof(StackLayers));
            }
        }

        //ListCollectionView stackLayersView;

        //public ListCollectionView StackLayers
        //{
        //    get
        //    {
        //        stackLayersView.Refresh();
        //        return stackLayersView;

        //    }
        //}

        public IList<ILayerDesignerItem> StackLayers
        {
            get
            {
                switch (layerDisplay)
                {
                    case LayerStackupSpec.All:
                        return board.LayerItems;
                    case LayerStackupSpec.Stackup:
                        {
                            var stackUpLayers = new List<LayerType>
                                {
                                     LayerType.Signal,
                                     LayerType.Plane,
                                     //LayerType.SolderMask,
                                     //LayerType.SilkScreen,
                                     LayerType.Dielectric
                                };

                            return board.LayerItems.Where(l => stackUpLayers.Contains((l as LayerDesignerItem).LayerType)).ToList();
                        }

                }

                return board.LayerItems;
            }
        }

        //void InitStackView()
        //{
        //    stackLayersView = new ListCollectionView((IList)board.LayerItems);
        //    stackLayersView.Filter = l =>
        //      {
        //          switch (layerDisplay)
        //          {
        //              case LayerStackupSpec.All:
        //                  return true;
        //              case LayerStackupSpec.Stackup:
        //                  {
        //                      var stackUpLayers = new List<LayerType>
        //                        {
        //                             LayerType.Signal,
        //                             LayerType.Plane,
        //                             //LayerType.SolderMask,
        //                             //LayerType.SilkScreen,
        //                             LayerType.Dielectric
        //                        };

        //                      return stackUpLayers.Contains((l as LayerDesignerItem).LayerType);
        //                  }
        //          }

        //          return true;
        //      };
        //}

        LayerDesignerItem selectedStackLayer;
        public LayerDesignerItem SelectedStackLayer
        {
            get { return selectedStackLayer; }
            set
            {
                selectedStackLayer = value;
                OnPropertyChanged(nameof(SelectedStackLayer));
            }
        }

        public double StackupTotalThickness
        {
            get
            {
                var validLayers = new List<LayerType>
                                {
                                     LayerType.Signal,
                                     LayerType.Plane,
                                     LayerType.Dielectric,
                                     //LayerType.SolderMask,
                                     LayerType.SilkScreen
                                };
                return board.LayerItems.Where(l => validLayers.Contains(l.LayerType)).Select(l => l.Thickness).Sum();
            }
        }

        public IEnumerable<ILayerDesignerItem> AvailableDrillLayers
        {
            get
            {
                var validLayers = new List<LayerType>
                                {
                                     LayerType.Signal,
                                     LayerType.Plane,
                                };
                return board.LayerItems.Where(l => validLayers.Contains(l.LayerType)).ToList();
            }
        }

        public IEnumerable<ILayerDesignerItem> AvailableLayerPairsLayers
        {
            get
            {
                var validLayers = new List<LayerType>
                                {
                                     LayerType.Mechanical,
                                };
                return board.LayerItems.Where(l => validLayers.Contains(l.LayerType)).ToList();
            }
        }

        //used for via definition
        public IList<ILayerPairModel> DrillPairs { get; set; } = new ObservableCollection<ILayerPairModel>();

        public IList<ILayerPairModel> LayerPairs { get; set; } = new ObservableCollection<ILayerPairModel>();

        void ReloadLayers()
        {
            // InitStackView();

            OnPropertyChanged(nameof(StackLayers));
        }

        #endregion Layer Stack

        #region Layer groups

        LayerGroupDesignerItem selectedLayerGroup;
        public LayerGroupDesignerItem SelectedLayerGroup
        {
            get { return selectedLayerGroup; }
            set
            {
                selectedLayerGroup = value;
                OnPropertyChanged(nameof(SelectedLayerGroup));
                OnPropertyChanged(nameof(LayerGroupsAvailableLayers));
            }
        }

        ICommand addLayerGroupCommand;

        public ICommand AddLayerGroupCommand
        {
            get
            {
                if (addLayerGroupCommand == null)
                {
                    addLayerGroupCommand = CreateCommand(p =>
                      {
                          var newG = new LayerGroupDesignerItem
                          {
                              Name = "New group",
                              IsReadOnly = false
                          };
                          board.LayerGroups.Add(newG);
                          SelectedLayerGroup = newG;
                      });
                }

                return addLayerGroupCommand;
            }
        }

        //RemoveLayerGroupCommand

        ICommand removeLayerGroupCommand;
        public ICommand RemoveLayerGroupCommand
        {
            get
            {
                if (removeLayerGroupCommand == null)
                {
                    removeLayerGroupCommand = CreateCommand(p =>
                    {
                        board.LayerGroups.Remove(SelectedLayerGroup);
                    },
                    p => SelectedLayerGroup != null && SelectedLayerGroup.IsReadOnly == false
                     );
                }

                return removeLayerGroupCommand;
            }
        }


        ICommand addLayersToGroupCommand;
        public ICommand AddLayersToGroupCommand
        {
            get
            {
                if (addLayersToGroupCommand == null)
                {
                    addLayersToGroupCommand = CreateCommand(p =>
                    {
                        var selectedLayers = p as IList;
                        if (selectedLayers == null)
                            return;

                        if (selectedLayerGroup == null)
                            return;

                        //foreach (var layer in selectedLayers)
                        selectedLayerGroup.Layers.AddRange(selectedLayers.Cast<LayerDesignerItem>());
                        GetSortableLayers(selectedLayerGroup.Layers).SortAscending(l => l.LayerId);
                        OnPropertyChanged(nameof(LayerGroupsAvailableLayers));
                    },
                    p => SelectedLayerGroup != null && SelectedLayerGroup.IsReadOnly == false
                     );
                }

                return addLayersToGroupCommand;
            }
        }


        ICommand removeLayersFromGroupCommand;
        public ICommand RemoveLayersFromGroupCommand
        {
            get
            {
                if (removeLayersFromGroupCommand == null)
                {
                    removeLayersFromGroupCommand = CreateCommand(p =>
                    {
                        var selectedLayers = p as IList;
                        if (selectedLayers == null)
                            return;

                        if (selectedLayerGroup == null)
                            return;

                        foreach (var layer in selectedLayers.Cast<LayerDesignerItem>().ToList())
                            selectedLayerGroup.Layers.Remove(layer);

                        GetSortableLayers(selectedLayerGroup.Layers).SortAscending(l => l.LayerId);
                        OnPropertyChanged(nameof(LayerGroupsAvailableLayers));
                    },
                    p => SelectedLayerGroup != null && SelectedLayerGroup.IsReadOnly == false
                     );
                }

                return removeLayersFromGroupCommand;
            }
        }

        SortableObservableCollection<ILayerDesignerItem> GetSortableLayers(IList<ILayerDesignerItem> layers)
        {
            return layers as SortableObservableCollection<ILayerDesignerItem>;
        }


        public IList<ILayerDesignerItem> LayerGroupsAvailableLayers
        {
            get
            {
                if (selectedLayerGroup == null)
                    return null;

                return board.LayerItems.Except(selectedLayerGroup.Layers).ToList();
            }
        }
        #endregion

        public void Show()
        {
            ReloadLayers();
            ReloadRules();
            ReloadBom();
            PreviewBom();
            PreviewAssembly();
        }

        void ReloadBom()
        {
            PartsBOMViewModel.LoadFromCurrentBoard(board);
        }


        void PreviewBom()
        {
            buildOptions.PreviewBom();
        }

        void PreviewAssembly()
        {
            buildOptions.PreviewAssembly();
        }

        public IList<IBoardRuleModel> Rules { get; set; } = new ObservableCollection<IBoardRuleModel>();
        public ISolutionProjectNodeModel ParentProject { get; set; }



        #region Board Rules

        AbstractBoardRule currentRuleNode;
        public AbstractBoardRule CurrentRuleNode
        {
            get { return currentRuleNode; }
            set
            {
                currentRuleNode = value;
                OnPropertyChanged(nameof(CurrentRuleNode));
            }
        }

        #region Board.Rules.Commands

        ICommand removeRuleNodeCommand;

        public ICommand RemoveRuleNodeCommand
        {
            get
            {
                if (removeRuleNodeCommand == null)
                    removeRuleNodeCommand = CreateCommand(p =>
                    {
                        RemoveBoardRuleNode(CurrentRuleNode);
                    });

                return removeRuleNodeCommand;
            }
        }

        ICommand addGroupRuleCommand;

        public ICommand AddGroupRuleCommand
        {
            get
            {
                if (addGroupRuleCommand == null)
                    addGroupRuleCommand = CreateCommand(p =>
                    {
                        var newGroup = new GroupRuleModel();// { Id = LibraryItem.GetNextId(), Name = "New group" };
                        AddBoardRule(newGroup);
                    });

                return addGroupRuleCommand;
            }
        }

        ICommand addElectricalClearanceRuleCommand;

        public ICommand AddElectricalClearanceRuleCommand
        {
            get
            {
                if (addElectricalClearanceRuleCommand == null)
                    addElectricalClearanceRuleCommand = CreateCommand(p =>
                    {
                        var newRule = new ElectricalClearanceRuleModel();
                        AddBoardRule(newRule);
                    });

                return addElectricalClearanceRuleCommand;
            }
        }

        ICommand addTrackWidthRuleCommand;

        public ICommand AddTrackWidthRuleCommand
        {
            get
            {
                if (addTrackWidthRuleCommand == null)
                    addTrackWidthRuleCommand = CreateCommand(p =>
                    {
                        var newRule = new TrackWidthRuleModel();
                        AddBoardRule(newRule);
                    });

                return addTrackWidthRuleCommand;
            }
        }

        ICommand addViaDefinitionRuleCommand;

        public ICommand AddViaDefinitionRuleCommand
        {
            get
            {
                if (addViaDefinitionRuleCommand == null)
                    addViaDefinitionRuleCommand = CreateCommand(p =>
                    {
                        var newRule = new ViaDefinitionRuleModel();
                        AddBoardRule(newRule);
                    });

                return addViaDefinitionRuleCommand;
            }
        }

        ICommand addMinAnnularRingRuleCommand;

        public ICommand AddMinAnnularRingRuleCommand
        {
            get
            {
                if (addMinAnnularRingRuleCommand == null)
                    addMinAnnularRingRuleCommand = CreateCommand(p =>
                    {
                        //var newRule = new ManufacturingMinAnnularRingRuleModel();
                        //AddBoardRule(CurrentRuleNode as GroupRuleModel, newRule);
                    });

                return addMinAnnularRingRuleCommand;
            }
        }

        ICommand addHoleSizeRuleCommand;

        public ICommand AddHoleSizeRuleCommand
        {
            get
            {
                if (addHoleSizeRuleCommand == null)
                    addHoleSizeRuleCommand = CreateCommand(p =>
                    {
                        var newRule = new ManufacturingHoleSizeRuleModel();
                        AddBoardRule(newRule);
                    });

                return addHoleSizeRuleCommand;
            }
        }

        ICommand addMaskExpansionCommand;

        public ICommand AddMaskExpansionCommand
        {
            get
            {
                if (addMaskExpansionCommand == null)
                    addMaskExpansionCommand = CreateCommand(p =>
                    {
                        var newRule = new MaskExpansionRuleModel();
                        AddBoardRule(newRule);
                    });

                return addMaskExpansionCommand;
            }
        }

        ICommand addManufacturingClearanceRuleCommand;

        public ICommand AddManufacturingClearanceRuleCommand
        {
            get
            {
                if (addManufacturingClearanceRuleCommand == null)
                    addManufacturingClearanceRuleCommand = CreateCommand(p =>
                    {
                        var newRule = new ManufacturingClearanceRuleModel();
                        AddBoardRule(newRule);
                    });

                return addManufacturingClearanceRuleCommand;
            }
        }

        ICommand addMatchedLengthsRuleCommand;

        public ICommand AddMatchedLengthsRuleCommand
        {
            get
            {
                if (addMatchedLengthsRuleCommand == null)
                    addMatchedLengthsRuleCommand = CreateCommand(p =>
                    {
                        //var newRule = new MatchLengthsRuleModel();
                        //AddBoardRule(CurrentRuleNode as GroupRuleModel, newRule);
                    });

                return addMatchedLengthsRuleCommand;
            }
        }

        void AddBoardRule(AbstractBoardRule boardRule)
        {
            boardRule.Load(board);
            var parentGroup = currentRuleNode as GroupRuleModel;

            if (parentGroup != null)
                parentGroup.AddChild(boardRule);
            else
                Rules.Add(boardRule);
        }

        void RemoveBoardRuleNode(AbstractBoardRule rule)
        {
            if (rule.Parent != null)
                rule.Parent.RemoveChild(rule);
            else
                Rules.Remove(rule);
        }

        #endregion Rules.Commands

        void ReloadRules()
        {
            foreach (var r in Rules)
                r.Load(board);
        }




        #endregion Board Rules

        #region Output Options

        BoardBuildOptionsViewModel buildOptions;

        public BoardBuildOptionsViewModel BuildOptions
        {
            get { return buildOptions; }
        }

        void LoadBuildOptions(BoardDocument boardDoc)
        {
            buildOptions.LoadFrom(boardDoc);

            //if (boardDoc.OutputOptions != null)
            //{
            //    //gerber
            //    buildOptions.GerberFormatBeforeDecimal = boardDoc.OutputOptions.GerberFormatBeforeDecimal;
            //    buildOptions.GerberFormatAfterDecimal = boardDoc.OutputOptions.GerberFormatAfterDecimal;
            //    buildOptions.GerberUnits = boardDoc.OutputOptions.GerberUnits;
            //    buildOptions.GerberPlotBoardOutlineOnAllLayers = boardDoc.OutputOptions.GerberPlotBoardOutlineOnAllLayers;
            //    buildOptions.GerberCreateZipFile = boardDoc.OutputOptions.GerberCreateZipFile;
            //    buildOptions.GerberVersion = boardDoc.OutputOptions.GerberVersion;

            //    //ND Drills
            //    buildOptions.NCDrillFormatBeforeDecimal = boardDoc.OutputOptions.NCDrillFormatBeforeDecimal;
            //    buildOptions.NCDrillFormatAfterDecimal = boardDoc.OutputOptions.NCDrillFormatAfterDecimal;
            //    buildOptions.NCDrillUnits = boardDoc.OutputOptions.NCDrillUnits;

            //    buildOptions.Bom.lo
            //}
        }

        void SaveBuildOptions(BoardDocument boardDoc)
        {
            buildOptions.SaveTo(boardDoc);
        }

        #endregion

        public void LoadFrom(BoardDocument boardDoc)
        {
            //general
            BoardUnits = boardDoc.BoardUnits;
            if (boardDoc.Description != null)
                Description = boardDoc.Description.Value;
            SchematicReference = boardDoc.SchematicReference;

            LoadBuildOptions(boardDoc);

            ((INotifyCollectionChanged)board.LayerItems).CollectionChanged += LayerItems_CollectionChanged;
            foreach (var layer in board.LayerItems)
                layer.PropertyChanged += Layer_PropertyChanged;
        }

        public void SaveTo(BoardDocument board)
        {
            //general
            board.BoardUnits = BoardUnits;
            board.Description = new Description
            {
                Value = Description
            };
            board.SchematicReference = SchematicReference;


            SaveBuildOptions(board);
        }
    }

    public enum LayerStackupSpec
    {
        All,
        Stackup
    }

    public class NTuple<T> : IEquatable<NTuple<T>>
    {
        public NTuple(IEnumerable<T> values)
        {
            Values = values.ToArray();
        }

        public readonly T[] Values;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            return Equals(obj as NTuple<T>);
        }

        public bool Equals(NTuple<T> other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other == null)
                return false;
            var length = Values.Length;
            if (length != other.Values.Length)
                return false;
            for (var i = 0; i < length; ++i)
                if (!Equals(Values[i], other.Values[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            var hc = 17;
            foreach (var value in Values)
                hc = hc * 37 + (!ReferenceEquals(value, null) ? value.GetHashCode() : 0);
            return hc;
        }
    }

    public class CustomType : CustomTypeDescriptor
    {
        private readonly ICollection<PropertyDescriptor> _propertyDescriptors = new List<PropertyDescriptor>();


        public CustomType(IEnumerable<PropertyNameDisplayMapping> propertyNames, object targetObject)
        {
            Owner = targetObject;

            var allProps = TypeDescriptor.GetProperties(targetObject).Cast<PropertyDescriptor>().ToList();

            foreach (var propName in propertyNames)
            {
                var prop = allProps.FirstOrDefault(p => p.Name == propName.PropertyName);
                if (prop != null)
                    _propertyDescriptors.Add(new CustomPropertyDescriptor(prop, targetObject, propName.DisplayName));
            }

        }

        public object Owner { get; set; }

        //public ReadOnlyCollection<ItemPropertyInfo> ItemProperties => _propertyDescriptors.Select(p=>
        //    new ItemPropertyInfo(p.Name,p.PropertyType,null)
        //).ToList().AsReadOnly();

        //void AddProperty(string name)
        //{
        //    _propertyDescriptors.Add(new CustomPropertyDescriptor(name));
        //}

        public override PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(_propertyDescriptors.ToArray());
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return Owner;
        }

        public override EventDescriptorCollection GetEvents()
        {
            return null;
        }

        public override EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return null;
        }

        //public string GetListName(PropertyDescriptor[] listAccessors)
        //{
        //    return "awesome";
        //}

        //public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        //{
        //    return GetProperties();
        //}


    }

    public class PropertyNameDisplayMapping
    {
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
    }

    class CustomPropertyDescriptor : PropertyDescriptor
    {

        public CustomPropertyDescriptor(MemberDescriptor descriptor, object owner, string displayName)
            : base(descriptor)
        {
            Owner = owner;
            originalDescriptor = descriptor;

            _displayName = displayName;
            _propertyName = descriptor.Name;
        }

        //public CustomPropertyDescriptor(string name) : base(name, null)
        //{

        //}

        public object Owner { get; set; }

        MemberDescriptor originalDescriptor;
        private readonly string _displayName;
        private readonly string _propertyName;

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get
            {
                if (Owner != null)
                    return Owner.GetType();

                return typeof(object);
            }
        }

        public override object GetValue(object component)
        {
            //need base.Name to take the real name of the property
            var pi = Owner.GetType().GetProperty(_propertyName, PropertyType);
            return pi?.GetValue(Owner);

        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get
            {
                if (originalDescriptor != null)
                    return (originalDescriptor as PropertyDescriptor).PropertyType;

                return typeof(object);
            }
        }

        //public override string Name => _displayName?? base.Name;//datagrid displays this header

        public override string DisplayName => _displayName;

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }

    }

    public class DynamicList : List<CustomType>, ITypedList
    {
        public DynamicList(IEnumerable<PropertyNameDisplayMapping> propertyNames, IList<CustomType> list)
        {
            var c = list.FirstOrDefault();
            var allProps = TypeDescriptor.GetProperties(c).Cast<PropertyDescriptor>().ToList();

            foreach (var propName in propertyNames)
            {
                var prop = allProps.FirstOrDefault(p => p.Name == propName.PropertyName);
                if (prop != null)
                    propertyDescriptors.Add(new CustomPropertyDescriptor(prop, null, propName.DisplayName));
            }

            AddRange(list);
        }

        private readonly ICollection<PropertyDescriptor> propertyDescriptors = new List<PropertyDescriptor>();

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return new PropertyDescriptorCollection(propertyDescriptors.ToArray());
            //return TypeDescriptor.GetProperties(typeof(CustomType));
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return null;
        }
    }
}
