using IDE.Core.Interfaces;
using IDE.Core.Presentation;
using IDE.Core.Storage;
using IDE.Core.Wizards;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IDE.Core.Designers
{

    public class ElectricalClearanceRuleModel : AbstractBoardRule
    {
        public ElectricalClearanceRuleModel()
        {
            //Category = RuleCategory.Electrical;
        }

        public override string RuleType
        {
            get
            {
                return "Electrical clearance";
            }
        }

        double minClearance = 0.254;

        public double MinClearance
        {
            get { return minClearance; }
            set
            {
                minClearance = value;
                OnPropertyChanged(nameof(MinClearance));
            }
        }


        double maxClearance = 0;

        public double MaxClearance
        {
            get { return maxClearance; }
            set
            {
                maxClearance = value;
                OnPropertyChanged(nameof(MaxClearance));
            }
        }

        double defaultClearance = 0.254;

        public double DefaultClearance
        {
            get { return defaultClearance; }
            set
            {
                defaultClearance = value;
                OnPropertyChanged(nameof(DefaultClearance));
            }
        }

        RuleLayerFilter layerFilter;
        public RuleLayerFilter LayerFilter
        {
            get { return layerFilter; }
            set
            {
                layerFilter = value;
                OnPropertyChanged(nameof(LayerFilter));
            }
        }

        RegularOptionGroupViewModel<RuleLayerFilter> ruleLayerFilters;
        public RegularOptionGroupViewModel<RuleLayerFilter> RuleLayerFilters
        {
            get
            {
                if (ruleLayerFilters == null)
                {
                    var list = new List<OptionViewModel<RuleLayerFilter>>()
                    {
                        new OptionViewModel<RuleLayerFilter>(RuleLayerFilter.All, 0, null, "All layers"),
                        new OptionViewModel<RuleLayerFilter>(RuleLayerFilter.PerLayer, 0, null, "Specify for layers"),
                    };

                    foreach (var option in list)
                    {
                        if (option.GetValue() == LayerFilter)
                            option.IsSelected = true;
                        option.PropertyChanged += (s, e) =>
                        {
                            var o = s as OptionViewModel<RuleLayerFilter>;
                            if (option.IsSelected)
                                LayerFilter = option.GetValue();
                        };
                    }

                    ruleLayerFilters = new RegularOptionGroupViewModel<RuleLayerFilter>("Layers") { OptionModels = list.AsReadOnly() };
                }
                return ruleLayerFilters;
            }
        }

        //RuleFilterData ruleFilterObject1 = new RuleFilterData();
        //public RuleFilterData RuleFilterObject1
        //{
        //    get { return ruleFilterObject1; }
        //    set
        //    {
        //        ruleFilterObject1 = value;
        //        OnPropertyChanged(nameof(RuleFilterObject1));
        //    }
        //}

        //RuleFilterData ruleFilterObject2 = new RuleFilterData();
        //public RuleFilterData RuleFilterObject2
        //{
        //    get { return ruleFilterObject2; }
        //    set
        //    {
        //        ruleFilterObject2 = value;
        //        OnPropertyChanged(nameof(RuleFilterObject2));
        //    }
        //}

        IList<OptionViewModel<RuleFilterObjectType>> objectTypes;
        public IList<OptionViewModel<RuleFilterObjectType>> ObjectTypes
        {
            get
            {
                if (objectTypes == null)
                {
                    var list = new List<OptionViewModel<RuleFilterObjectType>>()
                    {
                        new OptionViewModel<RuleFilterObjectType>(RuleFilterObjectType.All, 0, null, "All"),
                        new OptionViewModel<RuleFilterObjectType>(RuleFilterObjectType.AllFiltered, 0, null, "All filtered"),
                        new OptionViewModel<RuleFilterObjectType>(RuleFilterObjectType.Net, 0, null, "Net"),
                        new OptionViewModel<RuleFilterObjectType>(RuleFilterObjectType.NetClass, 0, null, "Net class"),
                        //removed temporarly
                        //new OptionViewModel<RuleFilterObjectType>(RuleFilterObjectType.NetClassGroup, 0, null, "Net class group"),
                    };

                    objectTypes = list.AsReadOnly();
                }
                return objectTypes;
            }
        }

        public BoardRuleFilterObjectViewModel Filter1 { get; set; } = new BoardRuleFilterObjectViewModel();

        public BoardRuleFilterObjectViewModel Filter2 { get; set; } = new BoardRuleFilterObjectViewModel();

        //loaded from board signal layers (left join with current layerSpecs rules)
        public ObservableCollection<LayerMinMaxDefaultSpec> LayerSpecs { get; set; } = new ObservableCollection<LayerMinMaxDefaultSpec>();

        public override void Load(ILayeredViewModel doc)
        {
            Filter1.Document = doc;
            Filter2.Document = doc;

            foreach (var boardLayer in doc.LayerItems.Cast<LayerDesignerItem>().Where(l => l.LayerType == LayerType.Signal || l.LayerType == LayerType.Plane))
            {
                var l = LayerSpecs.FirstOrDefault(ls => ls.LayerId == boardLayer.LayerId);
                if (l != null)
                {
                    if (l.LayerName == null)
                        l.LayerName = boardLayer.LayerName;
                }
                else
                {
                    LayerSpecs.Add(new LayerMinMaxDefaultSpec
                    {
                        LayerId = boardLayer.LayerId,
                        LayerName = boardLayer.LayerName,
                        Default = 0.254,
                        Min = 0.254,
                        Max = 0
                    });
                }
            }
        }

        public override BoardRule SaveToBoardRule()
        {
            return new ElectricalClearanceRule
            {
                //abstract
                Id = Id,
                Name = Name,
                Comment = Comment,
                IsEnabled = IsEnabled,
                Priority = Priority,

                //specific
                Min = MinClearance,
                Max = MaxClearance,
                Default = DefaultClearance,
                LayerFilter = LayerFilter,
                LayerSpecs = LayerSpecs.ToList(),
                RuleFilterObject1 = Filter1.RuleFilterObject,
                RuleFilterObject2 = Filter2.RuleFilterObject
            };
        }

        public override bool RuleAppliesToItem(ISelectableItem item)
        {
            if (Filter1 == null || Filter2 == null)
                return false;
            if (item is ISignalPrimitiveCanvasItem)
                return Filter1.IsItemFiltered(item) || Filter2.IsItemFiltered(item);

            //todo: investigate what happens when one item is already passed by filter1 and we need the second item to be passed by filter2
            return false;
        }

        public override bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2)
        {
            return Filter1.IsItemFiltered(item1) && Filter2.IsItemFiltered(item2)
                || Filter1.IsItemFiltered(item2) && Filter2.IsItemFiltered(item1);
        }

        public override bool IsPairedRule()
        {
            return true;
        }

        public override bool CheckItems(ISelectableItem item1, ISelectableItem item2, RuleCheckResult result)
        {

            var signalItem1 = item1 as ISignalPrimitiveCanvasItem;
            var signalItem2 = item2 as ISignalPrimitiveCanvasItem;

            if (signalItem1 == null || signalItem2 == null || signalItem1.Signal == null || signalItem2.Signal == null)
                return true;

            //if same signal, then it's fine
            if (signalItem1.Signal.Name.ToLower() == signalItem2.Signal.Name.ToLower())
                return true;

            //we don't check clearance for different layers
            var sp1 = item1 as SingleLayerBoardCanvasItem;
            var sp2 = item2 as SingleLayerBoardCanvasItem;
            if (sp1 != null && sp2 != null && sp1.LayerId != sp2.LayerId)
                return true;

            //item1 must be filtered by Filter1 and item2 by Filter2
            if (Filter1.IsItemFiltered(item1) && Filter2.IsItemFiltered(item2)
                || Filter1.IsItemFiltered(item2) && Filter2.IsItemFiltered(item1))
            {
                var minVal = minClearance;
                var maxVal = maxClearance;
                if (layerFilter == RuleLayerFilter.PerLayer)
                {
                    var layerSpec = LayerSpecs.FirstOrDefault(l => l.LayerId == sp1.LayerId);
                    if (layerSpec != null)
                    {
                        minVal = layerSpec.Min;
                        maxVal = layerSpec.Max;
                    }
                }

                var minGap = 2 * minVal - ClearanceTolerance;
                var maxGap = 2 * maxVal + ClearanceTolerance;

                //var g1 = GetGeometryItem(item1);
                ////we substract a small value so that it will not intersect and allow for a fixed clearance
                //var g1Min = g1.GetWidenedPathGeometry(new Pen(Brushes.Transparent, 2 * minVal - ClearanceTolerance), 1e-3, ToleranceType.Absolute);
                //var g1Max = g1.GetWidenedPathGeometry(new Pen(Brushes.Transparent, 2 * maxVal + ClearanceTolerance), 1e-3, ToleranceType.Absolute);
                //var g2 = GetGeometryItem(item2);

                ////check min clearance
                //if (GeometryHelper.Intersects(g1Min, g2))
                //{
                //    result.Message = $"Minimum clearance violation ({minVal}) between {item1} and {item2}";
                //    var intersection = (Geometry)GeometryHelper.GetIntersection(g1Min, g2);
                //    result.Location = new Errors.CanvasLocation
                //    {
                //        Geometry = new GeometryWrapper(intersection),
                //        Location = GeometryHelper.GetGeometryBounds(intersection)
                //    };
                //    return false;
                //}

                //check min clearance
                var location = GeometryHelper.CheckClearance(item1, item2, minGap);
                if (location != null)
                {
                    result.Message = $"Minimum clearance violation ({minVal}) between {item1} and {item2}";
                    result.Location = location;

                    return false;
                }

                //max clearance
                //if (maxVal > 0)
                //    if (!GeometryHelper.Intersects(g1Max, g2))
                //    {
                //        result.Message = $"Maximum clearance violation ({maxVal}) between {item1} and {item2}";
                //        var intersection = (Geometry)GeometryHelper.GetIntersection(g1Max, g2);
                //        result.Location = new Errors.CanvasLocation
                //        {
                //            Geometry = new GeometryWrapper(intersection),
                //            Location = GeometryHelper.GetGeometryBounds(intersection)
                //        };
                //        return false;
                //    }
                if (maxVal > 0)
                {
                    location = GeometryHelper.CheckClearance(item1, item2, maxGap);
                    if (location != null)
                    {
                        result.Message = $"Maximum clearance violation ({maxVal}) between {item1} and {item2}";
                        result.Location = location;

                        return false;
                    }
                }

            }
            else
            {
                result.Message = "Rule is not filtered correctly";
                return false;
            }

            return true;
        }



        public override void LoadFromData(IBoardRuleData rule)
        {
            var r = rule as ElectricalClearanceRule;

            Id = r.Id;
            Name = r.Name;
            Comment = r.Comment;
            IsEnabled = r.IsEnabled;
            Priority = r.Priority;

            //specific
            MinClearance = r.Min;
            MaxClearance = r.Max;
            DefaultClearance = r.Default;
            LayerFilter = r.LayerFilter;
            LayerSpecs = new ObservableCollection<LayerMinMaxDefaultSpec>(r.LayerSpecs);

            Filter1.RuleFilterObject = r.RuleFilterObject1;
            Filter2.RuleFilterObject = r.RuleFilterObject2;

        }
    }

}
