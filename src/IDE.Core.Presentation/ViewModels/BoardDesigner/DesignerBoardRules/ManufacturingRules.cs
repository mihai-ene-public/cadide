using IDE.Core.Storage;
using IDE.Core.Wizards;
using System;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Presentation;

namespace IDE.Core.Designers
{
    //this is scheduled, but not yet supported

    /*ManufacturingMinAnnularRingRuleModel
    public class ManufacturingMinAnnularRingRuleModel : AbstractBoardRule
    {
    public ManufacturingMinAnnularRingRuleModel()
    {
        //Category = RuleCategory.Manufacturing;

        Value = 0.2;//mm
    }

    public override string RuleType
    {
        get
        {
            return "Minimum annular ring";
        }
    }

    double _Value;
    public double Value
    {
        get
        {
            return _Value;
        }
        set
        {
            _Value = value;
            OnPropertyChanged(nameof(Value));
        }
    }
    public override BoardRule SaveToBoardRule()
    {
        return new ManufacturingMinAnnularRingRule
        {
            //abstract
            Id = Id,
            Name = Name,
            Comment = Comment,
            IsEnabled = IsEnabled,
            Priority = Priority,

            Value = Value,
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
    }
    */

    public class ManufacturingHoleSizeRuleModel : AbstractBoardRule
    {
        public ManufacturingHoleSizeRuleModel()
        {
            // Category = RuleCategory.Manufacturing;

            Min = 0.3;
            Max = 6;
        }

        public override string RuleType
        {
            get
            {
                return "Hole size";
            }
        }

        double min;
        public double Min
        {
            get
            {
                return min;
            }
            set
            {
                min = value;
                OnPropertyChanged(nameof(Min));
            }
        }

        double max;
        public double Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
                OnPropertyChanged(nameof(Max));
            }
        }
        public override void Load(ILayeredViewModel doc)
        {

        }

        public override BoardRule SaveToBoardRule()
        {
            return new ManufacturingHoleSizeRule
            {
                //abstract
                Id = Id,
                Name = Name,
                Comment = Comment,
                IsEnabled = IsEnabled,
                Priority = Priority,

                Min = Min,
                Max = Max,
            };
        }

        public override bool RuleAppliesToItem(ISelectableItem item)
        {
            //todo: pad hole
            return item is IHoleCanvasItem || item is IViaCanvasItem;
        }

        public override bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2)
        {
            return false;
        }

        public override bool IsPairedRule()
        {
            return false;
        }

        public override bool CheckItem(ISelectableItem item, RuleCheckResult result)
        {
            if (result == null)
                throw new ArgumentException("result");

            if (item is IHoleCanvasItem)
            {
                var hole = item as IHoleCanvasItem;
                var checks = hole.Drill >= min && hole.Drill <= max;

                if (!checks)
                {
                    result.Message = $"{item} drill is not betweeen {min} and {max}.";
                    result.Location = new Errors.CanvasLocation
                    {
                        Location = hole.GetBoundingRectangle()
                    };
                }

                return checks;
            }


            return true;
        }

        public override void LoadFromData(IBoardRuleData rule)
        {
            var r = rule as ManufacturingHoleSizeRule;

            Id = r.Id;
            Name = r.Name;
            Comment = r.Comment;
            IsEnabled = r.IsEnabled;
            Priority = r.Priority;

            Min = r.Min;
            Max = r.Max;
        }
    }

    public class ManufacturingClearanceRuleModel : AbstractBoardRule
    {
        public ManufacturingClearanceRuleModel()
        {
            //Category = RuleCategory.Manufacturing;

            Value = 0.254;
        }

        public override string RuleType
        {
            get
            {
                return "Hole to hole clearance";
            }
        }

        double _Value;
        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        ManufacturingClearanceRuleType clearanceType = ManufacturingClearanceRuleType.HoleToHoleClearance;
        public ManufacturingClearanceRuleType ClearanceType
        {
            get
            {
                return clearanceType;
            }
            set
            {
                clearanceType = value;
                OnPropertyChanged(nameof(ClearanceType));
            }
        }

        IList<OptionViewModel<ManufacturingClearanceRuleType>> clearanceTypes;
        public IList<OptionViewModel<ManufacturingClearanceRuleType>> ClearanceTypes
        {
            get
            {
                if (clearanceTypes == null)
                {
                    var list = new List<OptionViewModel<ManufacturingClearanceRuleType>>()
                    {
                        new OptionViewModel<ManufacturingClearanceRuleType>(ManufacturingClearanceRuleType.HoleToHoleClearance, 0, null, "Hole to hole clearance"),
                        new OptionViewModel<ManufacturingClearanceRuleType>(ManufacturingClearanceRuleType.SilkToSilkClearance, 0, null, "Silk to silk clearance"),
                        new OptionViewModel<ManufacturingClearanceRuleType>(ManufacturingClearanceRuleType.SilkToSolderMaskClearance, 0, null, "Silk to solder mask clearance"),
                        new OptionViewModel<ManufacturingClearanceRuleType>(ManufacturingClearanceRuleType.SolderMaskClearance, 0, null, "Solder mask to solder mask clearance"),
                        new OptionViewModel<ManufacturingClearanceRuleType>(ManufacturingClearanceRuleType.BoardOutlineClearance, 0, null, "Board outline clearance"),
                    };

                    clearanceTypes = list.AsReadOnly();
                }
                return clearanceTypes;
            }
        }
        public override void Load(ILayeredViewModel doc)
        {

        }
        public override BoardRule SaveToBoardRule()
        {
            return new ManufacturingClearanceRule
            {
                //abstract
                Id = Id,
                Name = Name,
                Comment = Comment,
                IsEnabled = IsEnabled,
                Priority = Priority,

                Value = Value,
                ClearanceType = ClearanceType
            };
        }


        public override bool RuleAppliesToItem(ISelectableItem item)
        {
            switch (clearanceType)
            {
                case ManufacturingClearanceRuleType.HoleToHoleClearance:
                    return item is IHoleCanvasItem || item is IViaCanvasItem || item is PadThtCanvasItem;

                case ManufacturingClearanceRuleType.SolderMaskClearance:
                    //single layer item on solder mask layer or pad
                    return (item is SingleLayerBoardCanvasItem && new[] { LayerConstants.SolderTopLayerId, LayerConstants.SolderBottomLayerId }.Contains(((SingleLayerBoardCanvasItem)item).LayerId))
                            || item is IPadCanvasItem;

                case ManufacturingClearanceRuleType.SilkToSolderMaskClearance:
                    return false;

                case ManufacturingClearanceRuleType.SilkToSilkClearance:
                    return (item is SingleLayerBoardCanvasItem && new[] { LayerConstants.SilkscreenTopLayerId, LayerConstants.SilkscreenBottomLayerId }.Contains(((SingleLayerBoardCanvasItem)item).LayerId));

                case ManufacturingClearanceRuleType.BoardOutlineClearance:
                    return false;
            }

            return false;
        }

        public override bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2)
        {
            return RuleAppliesToItem(item1) && RuleAppliesToItem(item2);
        }

        public override bool IsPairedRule()
        {
            return true;
        }

        public override bool CheckItems(ISelectableItem item1, ISelectableItem item2, RuleCheckResult result)
        {
            //var g1 = GetGeometryItem(item1);
            //var minVal = Value;
            ////we substract a small value so that it will not intersect and allow for a fixed clearance
            //var g1Min = g1.GetWidenedPathGeometry(new Pen(Brushes.Transparent, 2 * minVal - ClearanceTolerance), 1e-3, ToleranceType.Absolute);

            //var g2 = GetGeometryItem(item2);

            ////check min clearance
            //if (GeometryHelper.Intersects(g1Min, g2))
            //{
            //    result.Message = $"Minimum clearance violation between {item1} and {item2}";
            //    var intersection = (Geometry)GeometryHelper.GetIntersection(g1Min, g2);
            //    var bounds = intersection.Bounds;
            //    result.Location = new Errors.CanvasLocation
            //    {
            //        Geometry = new GeometryWrapper(intersection),
            //        Location = new Types.Media.XRect(bounds.X, bounds.Y, bounds.Width, bounds.Height)
            //    };
            //    return false;
            //}

            var clearance = 2 * Value - ClearanceTolerance;
            var location = GeometryHelper.CheckClearance(item1, item2, clearance);
            if (location != null)
            {
                result.Message = $"Minimum clearance violation between {item1} and {item2}";
                result.Location = location;

                return false;
            }

            return true;
        }

        public override void LoadFromData(IBoardRuleData rule)
        {
            var r = rule as ManufacturingClearanceRule;

            Id = r.Id;
            Name = r.Name;
            Comment = r.Comment;
            IsEnabled = r.IsEnabled;
            Priority = r.Priority;

            Value = r.Value;
            ClearanceType = r.ClearanceType;
        }
    }

    //public class ManufacturingMinimumSolderMaskSilverRuleModel : AbstractBoardRule
    //{
    //    public ManufacturingMinimumSolderMaskSilverRuleModel()
    //    {
    //        Category = RuleCategory.Manufacturing;

    //        Value = 0.15;
    //    }

    //    public override string RuleType
    //    {
    //        get
    //        {
    //            return "Minimum solder mask silver";
    //        }
    //    }

    //    double _Value;
    //    public double Value
    //    {
    //        get
    //        {
    //            return _Value;
    //        }
    //        set
    //        {
    //            _Value = value;
    //            OnPropertyChanged(nameof(Value));
    //        }
    //    }
    //}

    //public class ManufacturingSilkToSolderMaskClearanceRuleModel : AbstractBoardRule
    //{
    //    public ManufacturingSilkToSolderMaskClearanceRuleModel()
    //    {
    //        Category = RuleCategory.Manufacturing;

    //        Value = 0.15;
    //    }

    //    public override string RuleType
    //    {
    //        get
    //        {
    //            return "Silk to solder mask clearance";
    //        }
    //    }

    //    double _Value;
    //    public double Value
    //    {
    //        get
    //        {
    //            return _Value;
    //        }
    //        set
    //        {
    //            _Value = value;
    //            OnPropertyChanged(nameof(Value));
    //        }
    //    }
    //}

    //public class ManufacturingSilkToSilkClearanceRuleModel : AbstractBoardRule
    //{
    //    public ManufacturingSilkToSilkClearanceRuleModel()
    //    {
    //        Category = RuleCategory.Manufacturing;

    //        Value = 0.15;
    //    }

    //    public override string RuleType
    //    {
    //        get
    //        {
    //            return "Silk to silk clearance";
    //        }
    //    }

    //    double _Value;
    //    public double Value
    //    {
    //        get
    //        {
    //            return _Value;
    //        }
    //        set
    //        {
    //            _Value = value;
    //            OnPropertyChanged(nameof(Value));
    //        }
    //    }
    //}

    //public class ManufacturingBoardOutlineClearanceRuleModel : AbstractBoardRule
    //{
    //    public ManufacturingBoardOutlineClearanceRuleModel()
    //    {
    //        Category = RuleCategory.Manufacturing;

    //        Value = 0.254;
    //    }

    //    public override string RuleType
    //    {
    //        get
    //        {
    //            return "Board outline clearance";
    //        }
    //    }

    //    double _Value;
    //    public double Value
    //    {
    //        get
    //        {
    //            return _Value;
    //        }
    //        set
    //        {
    //            _Value = value;
    //            OnPropertyChanged(nameof(Value));
    //        }
    //    }
    //}
}
