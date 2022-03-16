using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.ObjectFinding;
using IDE.Core.Storage;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class PartsBOMViewModel : BaseViewModel
    {

        public ObservableCollection<PartBomItemDisplay> BomItems { get; set; } = new ObservableCollection<PartBomItemDisplay>();

        PartBomItemDisplay bomSelectedItem;
        public PartBomItemDisplay BomSelectedItem
        {
            get { return bomSelectedItem; }
            set
            {
                bomSelectedItem = value;
                OnPropertyChanged(nameof(BomSelectedItem));
            }
        }

        PartBomItemDisplay bomEditingItem;
        public PartBomItemDisplay BomEditingItem
        {
            get { return bomEditingItem; }
            set
            {
                bomEditingItem = value;
                OnPropertyChanged(nameof(BomEditingItem));
                OnPropertyChanged(nameof(IsBomEditing));

#if DEBUG
                OnPropertyChanged(nameof(IsBomSearchEnabled));
#endif

            }
        }

        public bool IsBomEditing => BomEditingItem != null;

#if DEBUG
        public bool IsBomSearchEnabled => !IsBomEditing;
#endif

        private ICommand addBomCommand;

        public ICommand AddBomCommand
        {
            get
            {
                if (addBomCommand == null)
                    addBomCommand = CreateCommand(p =>
                    {
                        var newBomItem = new PartBomItemDisplay();
                        BomItems.Add(newBomItem);
                        BomEditingItem = newBomItem;

                    });

                return addBomCommand;
            }
        }

        private ICommand editBomCommand;

        public ICommand EditBomCommand
        {
            get
            {
                if (editBomCommand == null)
                    editBomCommand = CreateCommand(p =>
                    {
                        BomEditingItem = BomSelectedItem;
                    });

                return editBomCommand;
            }
        }

        private ICommand backToBomCommand;

        public ICommand BackToBomCommand
        {
            get
            {
                if (backToBomCommand == null)
                    backToBomCommand = CreateCommand(p =>
                    {
                        BomEditingItem = null;
                    });

                return backToBomCommand;
            }
        }

        public void LoadFromCurrentSchematic(SchematicDesignerViewModel schematicModel)
        {
            BomItems.Clear();

            var parts = schematicModel.GetSchematicParts();

            foreach (var p in parts)
            {
                var bomItem = p.GetBomItem(schematicModel.ProjectNode);
                BomItems.Add(bomItem);
            }
        }





        public void SaveToSchematic(SchematicDocument schematic)
        {

        }

        public void LoadFromCurrentBoard(BoardDesignerFileViewModel board)
        {
            BomItems.Clear();

            var bomHelper = new BomHelper();
            BomItems.AddRange(bomHelper.GetBomFromBoard(board));
        }


    }

    public class BomHelper
    {


        public List<PartBomItemDisplay> GetBomFromBoard(BoardDesignerFileViewModel board)
        {
            var items = new List<PartBomItemDisplay>();

            var project = board.ProjectNode;

            var objectFinder = ServiceProvider.Resolve<IObjectFinder>();
            var schematic = objectFinder.FindObject<SchematicDocument>(project.Project, null, board.BoardProperties.SchematicReference.schematicId);

            var schParts = GetPartsWithGates(schematic);

            var parts = board.GetBoardParts();

            foreach (var p in parts)
            {
                var bomItem = p.GetBomItem(project);

                if (bomItem != null)
                {

                    var schPart = schParts.FirstOrDefault(sp => sp.Part.Id == p.PartId);

                    if (schPart != null)
                        bomItem.Comment = GetComment(schPart);

                    items.Add(bomItem);
                }

            }

            return items;
        }


        public Task<DynamicList> GetOutputData(BoardDesignerFileViewModel board, IList<BomOutputColumn> columns, IList<BomOutputColumn> groupColumns)
        {
            return Task.Run((() =>
            {
                var bomSource = GetBomFromBoard(board);

                //items not grouped
                var outputResult = bomSource.Select(p => new PartBomOutputItemDisplay(p)
                {
                    Quantity = 1
                });

                //build grouped view
                if (groupColumns.Count > 0)
                {
                    outputResult = outputResult.GroupBy(b => new NTuple<object>(from column in groupColumns
                                                                                select b.GetPropertyValue<object>(column.ColumnName)))
                                               .Select(g => GetPartBomItem(g));
                }

                var propertyNames = columns.Where(c => c.Show).Select(c => new PropertyNameDisplayMapping
                {
                    PropertyName = c.ColumnName,
                    DisplayName = c.ColumnName//c.Header
                }).ToList();

                var result = outputResult.Select(b => new CustomType(propertyNames, b))
                                         .ToList();

                return new DynamicList(propertyNames, result);
            }));
        }

        PartBomOutputItemDisplay GetPartBomItem(IGrouping<NTuple<object>, PartBomOutputItemDisplay> g)
        {
            var groupItems = g.Cast<PartBomOutputItemDisplay>();

            return new PartBomOutputItemDisplay
            {
                PartName = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.PartName)),
                Comment = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Comment)),
                Component = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Component)),
                Supplier = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Supplier)),
                Sku = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Sku)),
                Manufacturer = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Manufacturer)),
                MPN = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.MPN)),
                Description = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Description)),
                RoHS = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.RoHS)),
                Package = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Package)),
                Packaging = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Packaging)),
                Stock = GetAggregateValue<int>(groupItems, nameof(PartBomOutputItemDisplay.Stock)),
                Currency = GetAggregateValue<string>(groupItems, nameof(PartBomOutputItemDisplay.Currency)),
                // Properties = bomItem.Properties,
                //Prices = (int)GetAggregateValue(groupItems, nameof(PartBomOutputItemDisplay.pr)),
                Quantity = GetAggregateValue<int>(groupItems, nameof(PartBomOutputItemDisplay.Quantity)),
                Price = GetAggregateValue<double>(groupItems, nameof(PartBomOutputItemDisplay.Price))
            };
        }

        T GetAggregateValue<T>(IEnumerable<PartBomOutputItemDisplay> parts, string propName)
        {
            var pi = typeof(PartBomOutputItemDisplay).GetProperty(propName);
            if (pi == null)
                return default(T);

            switch (propName)
            {
                case nameof(PartBomOutputItemDisplay.PartName):
                case nameof(PartBomOutputItemDisplay.Comment):
                case nameof(PartBomOutputItemDisplay.Component):
                case nameof(PartBomOutputItemDisplay.Supplier):
                case nameof(PartBomOutputItemDisplay.Sku):
                case nameof(PartBomOutputItemDisplay.Manufacturer):
                case nameof(PartBomOutputItemDisplay.MPN):
                case nameof(PartBomOutputItemDisplay.Description):
                case nameof(PartBomOutputItemDisplay.RoHS):
                case nameof(PartBomOutputItemDisplay.Package):
                case nameof(PartBomOutputItemDisplay.Packaging):
                case nameof(PartBomOutputItemDisplay.Currency):
                    {
                        object aggString = string.Join(", ", parts.Select(p => (string)pi.GetValue(p)).Where(p => p != null).Distinct().ToArray());
                        return (T)aggString;
                    }
                case nameof(PartBomOutputItemDisplay.Stock):
                    {
                        var stocks = parts.Select(p => pi.GetValue(p)).Where(p => p != null).Distinct().ToList();
                        object aggStock = 0;
                        if (stocks.Count == 1)
                            aggStock = stocks.FirstOrDefault();
                        return (T)aggStock;
                    }
                case nameof(PartBomOutputItemDisplay.Quantity):
                    object aggQty = parts.Count(); //parts.Sum(p => p.Quantity);
                    return (T)aggQty;
                case nameof(PartBomOutputItemDisplay.Price):
                    var totalQuantity = parts.Sum(p => p.Quantity);
                    var price = (from p in parts
                                 from pp in p.Prices
                                 where totalQuantity >= pp.Number
                                 orderby pp.Number
                                 select pp.Price).FirstOrDefault();

                    object aggPrice = Math.Round(totalQuantity * price, 4);//parts.Sum(p => p.Quantity * price);
                    return (T)aggPrice;
            }

            return default(T);
        }

        //IEnumerable<object> GetGroupBy(PartBomItemDisplay part)
        //{
        //    return from column in GroupColumns
        //           select part.GetPropertyValue<object>(column.ColumnName);
        //}



        string GetComment(PartWithGates p)
        {
            var gate = p.PartGates.FirstOrDefault();
            return gate?.Comment;
        }

        List<PartWithGates> GetPartsWithGates(SchematicDocument schematic)
        {
            if (schematic != null && schematic.Parts != null && schematic.Sheets != null)
            {
                var partsWithGates = from sheet in schematic.Sheets
                                     from partGate in sheet.Instances
                                     group partGate by partGate.PartId into g
                                     select new PartWithGates
                                     {
                                         Part = schematic.Parts.FirstOrDefault(p => p.Id == g.Key),
                                         PartGates = g.Cast<Instance>().ToList()
                                     };

                return partsWithGates.ToList();

            }

            return new List<PartWithGates>();
        }

        class PartWithGates
        {
            public Part Part { get; set; }

            public List<Instance> PartGates { get; set; }
        }
    }

    public static class PartExtensions
    {
        public static ComponentDocument GetComponent(this Part part, ISolutionProjectNodeModel project)
        {
            var objectFinder = ServiceProvider.Resolve<IObjectFinder>();
            var comp = objectFinder.FindObject<ComponentDocument>(project.Project, part.ComponentLibrary, part.ComponentId);

            return comp;
        }

        public static ComponentDocument GetComponent(this BoardComponentInstance part, ISolutionProjectNodeModel project)
        {
            var objectFinder = ServiceProvider.Resolve<IObjectFinder>();
            var comp = objectFinder.FindObject<ComponentDocument>(project.Project, part.ComponentLibrary, part.ComponentId);

            return comp;
        }

        public static void GetBomItem(this ComponentDocument component, PartBomItemDisplay newBomItem)
        {
            newBomItem.Component = component.Name;

            var bomItem = component.BomItems?.FirstOrDefault();
            if (bomItem != null)
            {

                newBomItem.Supplier = bomItem.Supplier;
                newBomItem.Sku = bomItem.Sku;
                newBomItem.Manufacturer = bomItem.Manufacturer;
                newBomItem.MPN = bomItem.MPN;
                newBomItem.Description = bomItem.Description;
                newBomItem.Package = bomItem.Package;
                newBomItem.Packaging = bomItem.Packaging;
                newBomItem.Stock = bomItem.Stock;
                newBomItem.Currency = bomItem.Currency;
                newBomItem.Properties = bomItem.Properties;
                newBomItem.Prices = bomItem.Prices;
                newBomItem.Documents = bomItem.Documents;
            }
        }

        public static PartBomItemDisplay GetBomItem(this Part part, ISolutionProjectNodeModel project)
        {
            var newBomItem = new PartBomItemDisplay { PartName = part.Name };

            var component = part.GetComponent(project);

            if (component != null)
            {
                component.GetBomItem(newBomItem);
            }

            return newBomItem;
        }

        public static PartBomItemDisplay GetBomItem(this BoardComponentInstance part, ISolutionProjectNodeModel project)
        {
            var newBomItem = new PartBomItemDisplay { PartName = part.PartName };

            var component = part.GetComponent(project);
            if (component != null)
            {
                component.GetBomItem(newBomItem);
            }

            return newBomItem;
        }

        public static void SplitName(this Part part, out string Prefix, out int index)
        {
            part.Name.SplitName(out Prefix, out index);
        }
    }

    public class IndexedNameComparer : IComparer<string>
    {
        public int Compare(string part1, string part2)
        {
            part1.SplitName(out string prefix1, out int index1);
            part2.SplitName(out string prefix2, out int index2);

            if (prefix1 == null)
                return -1;

            var prefixCompare = prefix1.CompareTo(prefix2);
            if (prefixCompare == 0)
            {
                return index1.CompareTo(index2);
            }

            return prefixCompare;
        }
    }

    public class PartNameComparer : IComparer<Part>
    {
        public int Compare(Part part1, Part part2)
        {
            part1.SplitName(out string prefix1, out int index1);
            part2.SplitName(out string prefix2, out int index2);

            if (prefix1 == null)
                return -1;

            var prefixCompare = prefix1.CompareTo(prefix2);
            if (prefixCompare == 0)
            {
                return index1.CompareTo(index2);
            }

            return prefixCompare;
        }
    }

    public class BoardPartNameComparer : IComparer<BoardComponentInstance>
    {
        public int Compare(BoardComponentInstance part1, BoardComponentInstance part2)
        {
            part1.PartName.SplitName(out string prefix1, out int index1);
            part2.PartName.SplitName(out string prefix2, out int index2);

            if (prefix1 == null)
                return -1;

            var prefixCompare = prefix1.CompareTo(prefix2);
            if (prefixCompare == 0)
            {
                return index1.CompareTo(index2);
            }

            return prefixCompare;
        }
    }
}
