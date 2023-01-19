using IDE.Core;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Presentation.Solution;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class ItemSelectDialogViewModel : DialogViewModel
    {
        public ItemSelectDialogViewModel(TemplateType templateType, ProjectInfo projectInfo)
        {
            _objectFinder = ServiceProvider.Resolve<IObjectFinder>();
            _objectRepository = ServiceProvider.Resolve<IObjectRepository>();
            _solutionRepository = ServiceProvider.Resolve<ISolutionRepository>();

            PropertyChanged += ItemSelectDialogViewModel_PropertyChanged;

            TemplateType = templateType;
            _projectInfo = projectInfo;
        }

        private readonly IObjectFinder _objectFinder;
        private readonly IObjectRepository _objectRepository;
        private readonly ISolutionRepository _solutionRepository;
        private readonly ProjectInfo _projectInfo;

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

        IList<LibraryDisplay> fullSourceLibraries;

        IList<LibraryDisplay> libraries = new List<LibraryDisplay>();
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

        ICommand clearSearchFilterCommand;

        public ICommand ClearSearchFilterCommand
        {
            get
            {
                if (clearSearchFilterCommand == null)
                    clearSearchFilterCommand = CreateCommand(p => { SearchItemsFilter = null; });

                return clearSearchFilterCommand;
            }
        }

        protected override async Task LoadData(object args)
        {
            await LoadItems();
        }

        private IList<LibraryItem> LoadObjects(ProjectInfo project)
        {
            switch (TemplateType)
            {
                case TemplateType.Symbol:
                    return _objectRepository.LoadObjects<Symbol>(project).Cast<LibraryItem>().ToList();

                case TemplateType.Footprint:
                    return _objectRepository.LoadObjects<Footprint>(project).Cast<LibraryItem>().ToList();

                case TemplateType.Component:
                    return _objectRepository.LoadObjects<ComponentDocument>(project).Cast<LibraryItem>().ToList();

                case TemplateType.Schematic:
                    return _objectRepository.LoadObjects<SchematicDocument>(project).Cast<LibraryItem>().ToList();

                case TemplateType.Model:
                    return _objectRepository.LoadObjects<ModelDocument>(project).Cast<LibraryItem>().ToList();
            }

            return new List<LibraryItem>();
        }

        private Task LoadItems()
        {

            var libraries = new List<LibraryDisplay>();
            var project = _projectInfo;

            var libraryItems = LoadObjects(project);

            if (TemplateType == TemplateType.Component)
            {
                _objectFinder.LoadCache<Symbol>(project);
                _objectFinder.LoadCache<Footprint>(project);
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
                        var symbol = _objectFinder.FindCachedObject<Symbol>(gate.symbolId);

                        //todo if the symbol is not solved we should show something and log to output
                        if (symbol == null)
                        {

                        }
                        else
                        {
                            var symbolItem = new LibraryItemDisplay(project)
                            {
                                Name = symbol.Name,
                                LibraryName = symbol.Library,
                                ItemType = TemplateType.Symbol,
                                Document = symbol
                            };

                            LibraryItemDisplay footprintItem = null;
                            if (compDoc.Footprint != null)
                            {
                                var footprintRef = compDoc.Footprint;
                                {
                                    var fp = _objectFinder.FindCachedObject<Footprint>(footprintRef.footprintId);

                                    if (fp == null)
                                    {
                                        //footprint not solved
                                    }
                                    else
                                    {
                                        footprintItem = new LibraryItemDisplay(project)
                                        {
                                            Name = fp.Name,
                                            LibraryName = fp.Library,
                                            ItemType = TemplateType.Footprint,
                                            Document = fp
                                        };

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
                                    Footprint = new LibraryItemDisplay(project),
                                    Description = compDoc.Description
                                };
                                lib.Items.Add(compItem);
                            }

                        }
                    }
                }
                else
                {
                    var docDisplay = new LibraryItemDisplay(project)
                    {
                        Name = docName,
                        LibraryName = lib.Name,
                        ItemType = TemplateType,
                        Document = item,
                    };

                    lib.Items.Add(docDisplay);
                }
            }

            fullSourceLibraries = libraries;

            ApplyFromSource(fullSourceLibraries);

            _objectFinder.ClearCache<Symbol>();
            _objectFinder.ClearCache<Footprint>();

            return Task.CompletedTask;
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
                    var filteredItems = lib.Items.Where(li => li.Name != null && li.Name.Contains(searchItemsFilter, StringComparison.OrdinalIgnoreCase))
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
