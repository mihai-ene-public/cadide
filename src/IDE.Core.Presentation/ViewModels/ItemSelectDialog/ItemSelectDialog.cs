using IDE.Core;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.ObjectFinding;
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
            var objectFinder = ServiceProvider.Resolve<IObjectFinder>();

            var libraries = new List<LibraryDisplay>();
            var libraryItems = ProjectModel.LoadObjects(null, TemplateType);

            if (TemplateType == TemplateType.Component)
            {
                var project = ProjectModel as SolutionProjectNodeModel;
                objectFinder.LoadCache<Symbol>(project.Project);
                objectFinder.LoadCache<Footprint>(project.Project);
            }

            foreach (var item in libraryItems)
            {
                var lib = libraries.FirstOrDefault(l => l.Name == item.Library);
                if (lib == null)
                {
                    lib = new LibraryDisplay { Name = item.Library };
                    libraries.Add(lib);
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
                        //var symbol = ProjectModel.FindObject(TemplateType.Symbol, gate.symbolId) as Symbol;
                        var symbol = objectFinder.FindCachedObject<Symbol>(gate.symbolId);

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
                                    //var fp = ProjectModel.FindObject(TemplateType.Footprint, footprintRef.footprintId) as Footprint;
                                    var fp = objectFinder.FindCachedObject<Footprint>(footprintRef.footprintId);

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

                    lib.Items.Add(docDisplay);
                }
            }

            fullSourceLibraries = libraries;

            ApplyFromSource(fullSourceLibraries);

            objectFinder.ClearCache<Symbol>();
            objectFinder.ClearCache<Footprint>();
        }

        private Task PreviewSelectedItem()
        {
            return Task.Run(() =>
            {
                selectedItem?.PreviewDocument();
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
