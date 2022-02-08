using IDE.Core;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class ItemSelectDialogViewModel : DialogViewModel
    {
        public ItemSelectDialogViewModel()
        {
            dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();

            PropertyChanged += ItemSelectDialogViewModel_PropertyChanged;
        }

        private async void ItemSelectDialogViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SearchItemsFilter):
                    ApplyFilter();
                    break;

                case nameof(SelectedItem):
                  await PreviewSelectedItem();
                    break;
            }

        }



        IDispatcherHelper dispatcher;

        public string WindowTitle
        {
            get
            {
                return "Select Item";
            }
        }
        ItemDisplay selectedItem;
        public ItemDisplay SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public TemplateType TemplateType { get; set; }

        public ISolutionProjectNodeModel ProjectModel { get; set; }

        IList<LibraryDisplay> fullSourceLibraries;

        IList<LibraryDisplay> libraries = new ObservableCollection<LibraryDisplay>();
        public IList<LibraryDisplay> Libraries
        {
            get { return libraries; }
            set
            {
                libraries = value;
                OnPropertyChanged(nameof(Libraries));
            }
        }

        string searchItemsFilter;

        public string SearchItemsFilter
        {
            get { return searchItemsFilter; }
            set
            {
                searchItemsFilter = value;
                OnPropertyChanged(nameof(SearchItemsFilter));
            }
        }

        ICommand searchItemsFilterCommand;

        public ICommand SearchItemsFilterCommand
        {
            get
            {
                if (searchItemsFilterCommand == null)
                    searchItemsFilterCommand = CreateCommand(p => { SearchItemsFilter = null; });

                return searchItemsFilterCommand;
            }
        }

        protected override void LoadData()
        {
            LoadItems();
        }

        void LoadItems()
        {
            var libraries = new List<LibraryDisplay>();
            var libraryItems = ProjectModel.LoadObjects(null, TemplateType);

            if (TemplateType == TemplateType.Component)
            {
                var project = ProjectModel as SolutionProjectNodeModel;
                project.CreateCacheItems(TemplateType.Symbol);
                project.CreateCacheItems(TemplateType.Footprint);
            }

            foreach (var item in libraryItems)
            {
                var lib = libraries.FirstOrDefault(l => l.Name == item.Library);
                if (lib == null)
                {
                    lib = new LibraryDisplay { Name = item.Library };
                    libraries.Add(lib);
                    //dispatcher.RunOnDispatcher(() =>
                    //{
                    //    Libraries.Add(lib);
                    //    OnPropertyChanged(nameof(Libraries));
                    //});
                }

                var docName = item.Name;

                if (item is ComponentDocument compDoc)
                {
                    if (compDoc.Gates != null)
                    {
                        //display only the 1st gate
                        var gate = compDoc.Gates.FirstOrDefault();
                        if (gate == null)
                            continue;

                        //solve symbol
                        var symbol = ProjectModel.FindObject(TemplateType.Symbol, gate.symbolId) as Symbol;

                        //todo if the symbol is not solved we should show something and log to output
                        if (symbol == null)
                        {

                        }
                        else
                        {
                            var symbolItem = new LibraryItemDisplay
                            {
                                Name = symbol.Name,
                                LibraryName = symbol.Library,
                                ItemType = TemplateType.Symbol,
                                Document = symbol
                            };

                            //symbolItem.Preview.PreviewDocument(symbol, ProjectModel);

                            LibraryItemDisplay footprintItem = null;
                            if (compDoc.Footprint != null)
                            {
                                var footprintRef = compDoc.Footprint;
                                {
                                    var fp = ProjectModel.FindObject(TemplateType.Footprint, /*footprintRef.LibraryName,*/ footprintRef.footprintId) as Footprint;

                                    if (fp == null)
                                    {
                                        //footprint not solved
                                    }
                                    else
                                    {
                                        footprintItem = new LibraryItemDisplay
                                        {
                                            Name = fp.Name,
                                            LibraryName = fp.Library,
                                            ItemType = TemplateType.Footprint,
                                            Document = fp
                                        };

                                        //footprintItem.Preview.PreviewDocument(fp, ProjectModel);

                                        //if we have footprints add for each
                                        var compItem = new ComponentItemDisplay
                                        {
                                            Name = $"{docName} ({fp.Name})",
                                            LibraryName = lib.Name,
                                            ItemType = TemplateType,
                                            Document = item,
                                            Symbol = symbolItem,
                                            Footprint = footprintItem,
                                            Description = compDoc.Description
                                        };
                                        lib.Items.Add(compItem);
                                    }
                                }
                            }

                            //we add to the component list even if we don't have a footprint attached
                            if (footprintItem == null)
                            {
                                var compItem = new ComponentItemDisplay
                                {
                                    Name = $"{docName} (no footprint)",
                                    LibraryName = lib.Name,
                                    ItemType = TemplateType,
                                    Document = item,
                                    Symbol = symbolItem,
                                    Footprint = new LibraryItemDisplay(),
                                    Description = compDoc.Description
                                };
                                lib.Items.Add(compItem);
                            }

                        }
                    }
                }
                else
                {
                    var docDisplay = new LibraryItemDisplay
                    {
                        Name = docName,
                        LibraryName = lib.Name,
                        ItemType = TemplateType,
                        Document = item,
                        ProjectModel = ProjectModel
                    };

                    // docDisplay.Preview.PreviewDocument((LibraryItem)item, ProjectModel);

                    lib.Items.Add(docDisplay);
                }
            }

            fullSourceLibraries = libraries;

            //    dispatcher.RunOnDispatcher(() =>
            //{
            //    Libraries.Clear();
            //    Libraries.AddRange(libraries);
            //});

            ApplyFromSource(fullSourceLibraries);

            var p = ProjectModel as SolutionProjectNodeModel;
            p.ClearCachedItems();
        }

        private Task PreviewSelectedItem()
        {
            return Task.Run(() =>
            {
                selectedItem?.PreviewDocument();

                //if (selectedItem != null)
                //{
                //    //switch (selectedItem)
                //    //{
                //    //    case ComponentItemDisplay comp:
                //    //        {
                //    //            //if (comp.Symbol != null)
                //    //            //{
                //    //            //    var symbolDoc = comp.Symbol.Document as Symbol;
                //    //            //    comp.Symbol.Preview.PreviewDocument(symbolDoc, ProjectModel);
                //    //            //}
                //    //            //if (comp.Footprint != null)
                //    //            //{
                //    //            //    var fpDoc = comp.Footprint.Document as Footprint;
                //    //            //    comp.Footprint.Preview.PreviewDocument(fpDoc, ProjectModel);
                //    //            //}
                //    //            com

                //    //            break;
                //    //        }

                //    //    case LibraryItemDisplay libItem:
                //    //        libItem.Preview.PreviewDocument((LibraryItem)libItem.Document, ProjectModel);
                //    //        break;

                //    //}


                //}
            });
        }

        void ApplyFromSource(IList<LibraryDisplay> source)
        {
            Libraries = source.ToList();
        }

        void ApplyFilter()
        {
            var theNewSource = fullSourceLibraries;

            if (!string.IsNullOrEmpty(searchItemsFilter))
            {
                theNewSource = new List<LibraryDisplay>();

                foreach (var lib in fullSourceLibraries)
                {
                    var filteredItems = lib.Items.Where(li => li.Name != null && li.Name.Contains(searchItemsFilter, System.StringComparison.OrdinalIgnoreCase))
                                                 .ToList();

                    if (filteredItems.Count > 0)
                    {
                        var newLib = new LibraryDisplay
                        {
                            Name = lib.Name,
                            Items = new ObservableCollection<ItemDisplay>(filteredItems)
                        };

                        theNewSource.Add(newLib);
                    }
                }
            }

            ApplyFromSource(theNewSource);
        }
    }
}
