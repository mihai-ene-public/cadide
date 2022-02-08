using IDE.Core.Storage;
using IDE.Core.Wizards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Common;

namespace IDE.Core.Designers
{
    public class TrackWidthRuleModel : AbstractBoardRule
    {
        public TrackWidthRuleModel()
        {
            //Category = RuleCategory.Routing;

            MinWidth = 0.15;
            DefaultWidth = 0.254;
            // MaxWidth = 1;
        }

        public override string RuleType
        {
            get
            {
                return "Track width";
            }
        }

        double minWidth = 0.254;
        public double MinWidth
        {
            get
            {
                return minWidth;
            }
            set
            {
                minWidth = value;
                OnPropertyChanged(nameof(MinWidth));
            }
        }

        double defaultWidth = 0.254;
        public double DefaultWidth
        {
            get
            {
                return defaultWidth;
            }
            set
            {
                defaultWidth = value;
                OnPropertyChanged(nameof(DefaultWidth));
            }
        }

        double maxWidth;
        public double MaxWidth
        {
            get
            {
                return maxWidth;
            }
            set
            {
                maxWidth = value;
                OnPropertyChanged(nameof(MaxWidth));
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

        //RuleFilterData ruleFilterObject = new RuleFilterData();
        //public RuleFilterData RuleFilterObject
        //{
        //    get { return ruleFilterObject; }
        //    set
        //    {
        //        ruleFilterObject = value;
        //        OnPropertyChanged(nameof(RuleFilterObject));
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

        public BoardRuleFilterObjectViewModel Filter { get; set; } = new BoardRuleFilterObjectViewModel();

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

                    ruleLayerFilters = new RegularOptionGroupViewModel<RuleLayerFilter> { OptionModels = list.AsReadOnly() };
                }
                return ruleLayerFilters;
            }
        }

        //loaded from board signal layers (left join with current layerSpecs rules)
        public ObservableCollection<LayerMinMaxDefaultSpec> LayerSpecs { get; set; } = new ObservableCollection<LayerMinMaxDefaultSpec>();

        public override void Load(ILayeredViewModel doc)
        {
            Filter.Document = doc;

            foreach (var boardLayer in doc.LayerItems.Where(l => l.LayerType == LayerType.Signal))
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
                        Max = 8
                    });
                }
            }
        }

        public override BoardRule SaveToBoardRule()
        {
            return new TrackWidthRule
            {
                //abstract
                Id = Id,
                Name = Name,
                Comment = Comment,
                IsEnabled = IsEnabled,
                Priority = Priority,

                //specific
                Min = MinWidth,
                Max = MaxWidth,
                Default = DefaultWidth,
                LayerFilter = LayerFilter,
                LayerSpecs = LayerSpecs.ToList(),
                RuleFilterObject = Filter.RuleFilterObject
            };
        }

        public override bool RuleAppliesToItem(ISelectableItem item)
        {
            return item is TrackBoardCanvasItem && Filter.IsItemFiltered(item);
        }

        public override bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2)
        {
            return false;
        }

        public override bool IsPairedRule()
        {
            return false;
        }

        public override bool CheckItem(ISelectableItem item1, RuleCheckResult result)
        {
            var valid = false;

            var minVal = minWidth;
            var maxVal = maxWidth;
            var sp1 = item1 as TrackBoardCanvasItem;
            if (layerFilter == RuleLayerFilter.PerLayer)
            {
                var layerSpec = LayerSpecs.FirstOrDefault(l => l.LayerId == sp1.LayerId);
                if (layerSpec != null)
                {
                    minVal = layerSpec.Min;
                    maxVal = layerSpec.Max;
                }
            }

            if (maxVal > 0)
            {
                valid = (item1 as TrackBoardCanvasItem).Width.IsBetween(minVal, maxVal);
                if (!valid)
                {

                    result.Message = $"{item1} is not between [{minVal} - {maxVal}]";
                    result.Location = new Errors.CanvasLocation
                    {
                        Location = item1.GetBoundingRectangle()
                    };
                }
            }
            else
            {
                valid = (item1 as TrackBoardCanvasItem).Width >= minVal;
                if (!valid)
                {
                    result.Message = $"{item1} doesn't have the min width of [{minVal}]";
                    result.Location = new Errors.CanvasLocation
                    {
                        Location = item1.GetBoundingRectangle()
                    };
                }
            }


            return valid;
        }

        public override void LoadFromData(IBoardRuleData rule)
        {
            var r = rule as TrackWidthRule;

            //        //abstract
            Id = r.Id;
            Name = r.Name;
            Comment = r.Comment;
            IsEnabled = r.IsEnabled;
            Priority = r.Priority;

            //specific
            MinWidth = r.Min;
            MaxWidth = r.Max;
            DefaultWidth = r.Default;
            LayerFilter = r.LayerFilter;
            LayerSpecs = new ObservableCollection<LayerMinMaxDefaultSpec>(r.LayerSpecs);

            Filter.RuleFilterObject = r.RuleFilterObject;
        }
    }

    public class ViaDefinitionRuleModel : AbstractBoardRule
    {
        public ViaDefinitionRuleModel()
        {
            //Category = RuleCategory.Routing;
            Filter = new BoardRuleFilterObjectViewModel();
            Filter.RuleFilterObject.ObjectType = RuleFilterObjectType.DrillPair;

            DiameterMin = 0.7;
            DiameterDefault = 0.7;
            DiameterMax = 0.7;

            DrillMin = 0.3;
            DrillDefault = 0.3;
            DrillMax = 0.3;
        }

        public override string RuleType
        {
            get
            {
                return "Via definition";
            }
        }

        double diameterMin;
        public double DiameterMin
        {
            get
            {
                return diameterMin;
            }
            set
            {
                diameterMin = value;
                OnPropertyChanged(nameof(DiameterMin));
            }
        }

        double diameterDefault;
        public double DiameterDefault
        {
            get
            {
                return diameterDefault;
            }
            set
            {
                diameterDefault = value;
                OnPropertyChanged(nameof(DiameterDefault));
            }
        }

        double diameterMax;
        public double DiameterMax
        {
            get
            {
                return diameterMax;
            }
            set
            {
                diameterMax = value;
                OnPropertyChanged(nameof(DiameterMax));
            }
        }

        double drillMin;
        public double DrillMin
        {
            get
            {
                return drillMin;
            }
            set
            {
                drillMin = value;
                OnPropertyChanged(nameof(DrillMin));
            }
        }

        double drillDefault;
        public double DrillDefault
        {
            get
            {
                return drillDefault;
            }
            set
            {
                drillDefault = value;
                OnPropertyChanged(nameof(DrillDefault));
            }
        }

        double drillMax;
        public double DrillMax
        {
            get
            {
                return drillMax;
            }
            set
            {
                drillMax = value;
                OnPropertyChanged(nameof(DrillMax));
            }
        }

        public BoardRuleFilterObjectViewModel Filter { get; set; }

        public override void Load(ILayeredViewModel doc)
        {
            Filter.Document = doc;
        }

        public override BoardRule SaveToBoardRule()
        {
            return new ViaDefinitionRule
            {
                //abstract
                Id = Id,
                Name = Name,
                Comment = Comment,
                IsEnabled = IsEnabled,
                Priority = Priority,

                //specific
                DiameterMin = DiameterMin,
                DiameterDefault = DiameterDefault,
                DiameterMax = DiameterMax,
                DrillMin = DrillMin,
                DrillDefault = DrillDefault,
                DrillMax = DrillMax,
                RuleFilterObject = Filter.RuleFilterObject
            };
        }

        public override bool RuleAppliesToItem(ISelectableItem item)
        {
            return item is IViaCanvasItem && Filter.IsItemFiltered(item);
        }

        public override bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2)
        {
            return false;
        }

        public override bool IsPairedRule()
        {
            return false;
        }

        public override bool CheckItem(ISelectableItem item1, RuleCheckResult result)
        {
            //todo: currently our vias don't have drillpair
            var valid = false;

            var via = item1 as ViaCanvasItem;
            if (via == null)
                return true;

            valid = via.Diameter > via.Drill;
            if (!valid)
            {
                result.Message = $"{item1} diameter ({via.Diameter}) should be larger than the drill ({via.Drill})";
                result.Location = new Errors.CanvasLocation
                {
                    Location = item1.GetBoundingRectangle()
                };
                return false;
            }

            valid = via.Diameter.IsBetween(diameterMin, diameterMax);
            if (!valid)
            {
                result.Message = $"{item1} diameter is not between [{diameterMin} - {diameterMax}]";
                result.Location = new Errors.CanvasLocation
                {
                    Location = item1.GetBoundingRectangle()
                };
                return false;
            }
            valid = via.Drill.IsBetween(drillMin, drillMax);
            if (!valid)
            {
                result.Message = $"{item1} drill is not between [{drillMin} - {drillMax}]";
                result.Location = new Errors.CanvasLocation
                {
                    Location = item1.GetBoundingRectangle()
                };
                return false;
            }

            return true;
        }

        public override void LoadFromData(IBoardRuleData rule)
        {
            var r = rule as ViaDefinitionRule;

            Id = r.Id;
            Name = r.Name;
            Comment = r.Comment;
            IsEnabled = r.IsEnabled;
            Priority = r.Priority;

            //specific
            DiameterMin = r.DiameterMin;
            DiameterDefault = r.DiameterDefault;
            DiameterMax = r.DiameterMax;
            DrillMin = r.DrillMin;
            DrillDefault = r.DrillDefault;
            DrillMax = r.DrillMax;

            Filter.RuleFilterObject = r.RuleFilterObject;
        }
    }

    /*MatchLengthsRuleModel
    public class MatchLengthsRuleModel : AbstractBoardRule
    {
        public MatchLengthsRuleModel()
        {

        }

        public override string RuleType
        {
            get
            {
                return "Match lengths";
            }
        }

        double tolerance = 25.4;
        public double Tolerance
        {
            get
            {
                return tolerance;
            }
            set
            {
                tolerance = value;
                OnPropertyChanged(nameof(Tolerance));
            }
        }

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
                        //new OptionViewModel<RuleFilterObjectType>(RuleFilterObjectType.Net, 0, null, "Net"),
                        new OptionViewModel<RuleFilterObjectType>(RuleFilterObjectType.NetClass, 0, null, "Net class"),
                        new OptionViewModel<RuleFilterObjectType>(RuleFilterObjectType.NetClassGroup, 0, null, "Net class group"),
                    };

                    objectTypes = list.AsReadOnly();
                }
                return objectTypes;
            }
        }

        public BoardRuleFilterObjectViewModel Filter { get; set; } = new BoardRuleFilterObjectViewModel();



        public override BoardRule SaveToBoardRule()
        {
            return new MatchLengthsRule
            {
                //abstract
                Id = Id,
                Name = Name,
                Comment = Comment,
                IsEnabled = IsEnabled,
                Priority = Priority,

                Tolerance = Tolerance,
                RuleFilterObject = Filter.RuleFilterObject
            };
        }

        public override bool RuleAppliesToItem(ISelectableItem item)
        {
            return false;
        }
        public override bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2)
        {
            return false;
        }

        public override bool IsPairedRule()
        {
            return false;
        }

        public override bool CheckItem(ISelectableItem item1, out string message)
        {
            message = null;
            return true;
        }
    }
    */

    public class GroupRuleModel : AbstractBoardRule, IGroupBoardRuleModel
    {
        public GroupRuleModel()
        {

        }

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

        public BoardRuleFilterObjectViewModel Filter { get; set; } = new BoardRuleFilterObjectViewModel();

        public IList<IBoardRuleModel> Children { get; set; } = new ObservableCollection<IBoardRuleModel>();



        public override string RuleType
        {
            get
            {
                return "Group";
            }
        }

        public override void Load(ILayeredViewModel doc)
        {
            Filter.Document = doc;
        }

        public void AddChild(IBoardRuleModel newRule)
        {
            newRule.Parent = this;
            Children.Add(newRule);
        }

        public void RemoveChild(IBoardRuleModel child)
        {
            child.Parent = null;
            Children.Remove(child);
        }

        public override BoardRule SaveToBoardRule()
        {
            var g = new GroupRule
            {
                //abstract
                Id = Id,
                Name = Name,
                Comment = Comment,
                IsEnabled = IsEnabled,
                Priority = Priority,

                RuleFilterObject = Filter.RuleFilterObject
            };
            if (Children != null)
            {
                //add groups first
                g.Children.AddRange(Children.OfType<GroupRuleModel>().Select(c => c.SaveToBoardRule()));
                //rest of rules
                g.Children.AddRange(Children.Where(c => !(c is GroupRuleModel)).Cast<AbstractBoardRule>().Select(c => c.SaveToBoardRule()));

            }

            return g;
        }

        public override bool RuleAppliesToItem(ISelectableItem item)
        {
            //todo: this is more complex; must take into account other parent groups
            return false;
            //return Filter.IsItemFiltered(item);
        }

        public override bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2)
        {
            return false;
        }

        public override bool IsPairedRule()
        {
            return false;
        }

        public override bool CheckItem(ISelectableItem item1, RuleCheckResult result)
        {
            return true;
        }

        public override void LoadFromData(IBoardRuleData rule)
        {
            var r = rule as GroupRule;

            //abstract
            Id = r.Id;
            Name = r.Name;
            Comment = r.Comment;
            IsEnabled = r.IsEnabled;
            Priority = r.Priority;

            Filter.RuleFilterObject = r.RuleFilterObject;
            if (r.Children != null)
            {
                //add groups first
                Children.AddRange(r.Children.OfType<GroupRule>().Select(c => c.CreateRuleItem()));
                //rest of rules
                Children.AddRange(r.Children.Where(c => !(c is GroupRule)).Select(c => c.CreateRuleItem()));
                // g.Children.AddRange(Children.Select(c => c.CreateRuleItem()));

                foreach (var c in Children)
                    c.Parent = this;
            }
        }
    }

    public class BoardObjectData
    {
        public long ObjectId { get; set; }

        public string ObjectName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is BoardObjectData)
                return ObjectId == ((BoardObjectData)obj).ObjectId;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ObjectId.GetHashCode();
        }

        public override string ToString()
        {
            return ObjectName;
        }
    }

    public class BoardRuleFilterObjectViewModel : BaseViewModel
    {
        public BoardRuleFilterObjectViewModel()
        {
            RuleFilterObject = new RuleFilterData();
        }

        /// <summary>
        /// Current board FileViewModel
        /// </summary>
        public ILayeredViewModel Document { get; set; }

        ObservableCollection<BoardObjectData> boardObjects = new ObservableCollection<BoardObjectData>();
        public ObservableCollection<BoardObjectData> BoardObjects
        {
            get
            {
                var bo = LoadBoardObjects();
                boardObjects = new ObservableCollection<BoardObjectData>(bo);
                return boardObjects;
            }
            set
            {
                boardObjects = value;
                OnPropertyChanged(nameof(BoardObjects));
            }
        }

        BoardObjectData selectedBoardObject;
        public BoardObjectData SelectedBoardObject
        {
            get
            {
                if (ruleFilterObject != null)
                {
                    //if (selectedBoardObject == null || selectedBoardObject.ObjectId != ruleFilterObject.ObjectId)
                    selectedBoardObject = BoardObjects.FirstOrDefault(b => b.ObjectId == ruleFilterObject.ObjectId);
                }

                return selectedBoardObject;
            }
            set
            {
                selectedBoardObject = value;
                if (ruleFilterObject != null && selectedBoardObject != null)
                    ruleFilterObject.ObjectId = selectedBoardObject.ObjectId;
                OnPropertyChanged(nameof(SelectedBoardObject));
            }
        }

        public bool ShowObjects
        {
            get
            {
                var s = false;

                if (ruleFilterObject != null)
                {
                    var hideList = new[] { RuleFilterObjectType.All, RuleFilterObjectType.AllFiltered };
                    s = !hideList.Contains(ruleFilterObject.ObjectType);
                }
                return s;
            }
        }

        private IList<BoardObjectData> LoadBoardObjects()
        {
            var objType = ruleFilterObject.ObjectType;
            var list = new List<BoardObjectData>();
            //!!! we need to load from current open board; we should do it from a single place not for every rule!!!
            //we should take care that among object types to have different ids; there is a problem on view when coming to the same view, other entity with not showing in combo

            var board = Document as IBoardDesigner;

            if (board != null)
            {

                switch (objType)
                {
                    case RuleFilterObjectType.DrillPair:
                        var dpId = 1;
                        list.AddRange(board.DrillPairs.Select(dp => new BoardObjectData
                        {
                            ObjectId = dpId++,
                            ObjectName = dp.ToString()
                        }));
                        break;

                    case RuleFilterObjectType.Net:
                        list.AddRange(board.NetList.Where(n => n.IsNamed()).Select(n => new BoardObjectData
                        {
                            ObjectId = n.Id,
                            ObjectName = n.Name
                        }));
                        break;

                    case RuleFilterObjectType.NetClass:
                        //todo: we need to build this list as a linear list; there are groups that contain classes
                        var netClasses = board.NetClasses.OfType<NetClass>();
                        list.AddRange(netClasses.Select(n => new BoardObjectData
                        {
                            ObjectId = n.Id,
                            ObjectName = n.Name
                        }));
                        break;

                    case RuleFilterObjectType.NetClassGroup:
                        //todo: we need to build this list as a linear list; there are groups that contain classes
                        var netGroups = board.NetClasses.OfType<NetGroup>();
                        list.AddRange(netGroups.Select(n => new BoardObjectData
                        {
                            ObjectId = n.Id,
                            ObjectName = n.Name
                        }));
                        break;
                }
            }

            return list;
        }

        RuleFilterData ruleFilterObject;
        public RuleFilterData RuleFilterObject
        {
            get { return ruleFilterObject; }
            set
            {
                if (ruleFilterObject != null)
                    ruleFilterObject.PropertyChanged -= RuleFilterObject_PropertyChanged;

                ruleFilterObject = value;
                //LoadBoardObjects(ruleFilterObject.ObjectType);

                if (ruleFilterObject != null)
                    ruleFilterObject.PropertyChanged += RuleFilterObject_PropertyChanged;

                OnPropertyChanged(nameof(BoardObjects));
                OnPropertyChanged(nameof(RuleFilterObject));
                OnPropertyChanged(nameof(SelectedBoardObject));
                OnPropertyChanged(nameof(ShowObjects));
            }
        }

        void RuleFilterObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(RuleFilterObject.ObjectType):
                    //LoadBoardObjects(ruleFilterObject.ObjectType);
                    OnPropertyChanged(nameof(BoardObjects));
                    OnPropertyChanged(nameof(RuleFilterObject));
                    OnPropertyChanged(nameof(SelectedBoardObject));
                    OnPropertyChanged(nameof(ShowObjects));
                    break;

                case nameof(RuleFilterObject.ObjectId):
                    var bObj = BoardObjects.FirstOrDefault(b => b.ObjectId == ruleFilterObject.ObjectId);
                    if (bObj != null)
                        ruleFilterObject.ObjectName = bObj.ObjectName;

                    break;
            }
        }

        public bool IsItemFiltered(ISelectableItem item)
        {
            switch (ruleFilterObject.ObjectType)
            {
                case RuleFilterObjectType.All:
                    return true;
                case RuleFilterObjectType.AllFiltered:
                    return true;//this is not good
                case RuleFilterObjectType.DrillPair:
                    return item is IHoleCanvasItem || item is IViaCanvasItem;
                case RuleFilterObjectType.Net:
                    {
                        var si = item as ISignalPrimitiveCanvasItem;
                        if (si != null && si.Signal != null)
                        {
                            return si.Signal.Id == ruleFilterObject.ObjectId || si.Signal.Name == ruleFilterObject.ObjectName;
                        }

                        break;
                    }
                case RuleFilterObjectType.NetClass:
                    {
                        var si = item as ISignalPrimitiveCanvasItem;
                        if (si != null && si.Signal != null)
                        {
                            return si.Signal.ClassId == ruleFilterObject.ObjectId;
                        }

                        break;
                    }

                case RuleFilterObjectType.NetClassGroup:
                    {
                        var si = item as ISignalPrimitiveCanvasItem;
                        if (si != null && si.Signal != null)
                        {
                            //todo: must solve for groupId
                            var groupId = -1;
                            return groupId == ruleFilterObject.ObjectId;
                        }

                        break;
                    }
            }

            return false;
        }
    }
}
