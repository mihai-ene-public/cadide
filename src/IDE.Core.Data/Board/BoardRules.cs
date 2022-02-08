using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;



namespace IDE.Core.Storage
{
    #region Rules
    public abstract class BoardRule : IBoardRuleData
    {
        public BoardRule()
        {
            Priority = 1;
            IsEnabled = true;
        }

        [XmlAttribute("id")]
        public long Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("comment")]
        public string Comment { get; set; }

        [XmlAttribute("isEnabled")]
        public bool IsEnabled { get; set; }

        [XmlAttribute("priority")]
        public int Priority { get; set; }

        //constraints are defined as properties in classes that inherit from these
    }

    #region Electrical Rules
    public class ElectricalClearanceRule : BoardRule
    {
        public ElectricalClearanceRule()
        {
            LayerSpecs = new List<LayerMinMaxDefaultSpec>();
        }

        /// <summary>
        /// Minimum clearance between any of track, pad, via, polygon in mm
        /// </summary>
        [XmlAttribute("min")]
        public double Min { get; set; } = 0.254;

        [XmlAttribute("max")]
        public double Max { get; set; }

        [XmlAttribute("default")]
        public double Default { get; set; } = 0.254;

        [XmlAttribute("layerFilter")]
        public RuleLayerFilter LayerFilter { get; set; }

        [XmlElement("ruleFilterObject1")]
        public RuleFilterData RuleFilterObject1 { get; set; } = new RuleFilterData();

        [XmlElement("ruleFilterObject2")]
        public RuleFilterData RuleFilterObject2 { get; set; } = new RuleFilterData();

        [XmlArray("layerSpecs")]
        [XmlArrayItem("layer")]
        public List<LayerMinMaxDefaultSpec> LayerSpecs { get; set; }

    }

    #endregion


    public class TrackWidthRule : BoardRule
    {
        public TrackWidthRule()
        {
            LayerSpecs = new List<LayerMinMaxDefaultSpec>();
        }

        [XmlAttribute("min")]
        public double Min { get; set; } = 0.254;

        [XmlAttribute("default")]
        public double Default { get; set; } = 0.254;

        [XmlAttribute("max")]
        public double Max { get; set; }

        [XmlAttribute("layerFilter")]
        public RuleLayerFilter LayerFilter { get; set; }

        [XmlElement("ruleFilterObject")]
        public RuleFilterData RuleFilterObject { get; set; } = new RuleFilterData();

        [XmlArray("layerSpecs")]
        [XmlArrayItem("layer")]
        public List<LayerMinMaxDefaultSpec> LayerSpecs { get; set; }

    }

    public class ViaDefinitionRule : BoardRule
    {
        public ViaDefinitionRule()
        {

            DiameterMin = 0.7;
            DiameterDefault = 0.7;
            DiameterMax = 0.7;

            DrillMin = 0.3;
            DrillDefault = 0.3;
            DrillMax = 0.3;
        }

        [XmlAttribute("diameterMin")]
        public double DiameterMin { get; set; } = 0.7;

        [XmlAttribute("diameterMax")]
        public double DiameterMax { get; set; } = 0.8 + 0.2 * 2;

        [XmlAttribute("diameterDefault")]
        public double DiameterDefault { get; set; } = 0.7;


        [XmlAttribute("drillMin")]
        public double DrillMin { get; set; } = 0.3;

        [XmlAttribute("drillMax")]
        public double DrillMax { get; set; } = 0.8;

        [XmlAttribute("drillDefault")]
        public double DrillDefault { get; set; } = 0.3;

        [XmlElement("ruleFilterObject")]
        public RuleFilterData RuleFilterObject { get; set; } = new RuleFilterData { ObjectType = RuleFilterObjectType.DrillPair };

    }


    #region Mask Rules

    public class MaskExpansionRule : BoardRule
    {
        public MaskExpansionRule()
        {
            Expansion = 0.1;
        }

        [XmlAttribute("expansion")]
        public double Expansion { get; set; } = 0.1;

        [XmlAttribute("maskRuleType")]
        public MaskExpansionRuleType MaskRuleType { get; set; }

    }

    public enum MaskExpansionRuleType
    {
        Unknown,
        SolderMask,
        PasteMask
    }

    //public class MaskPasteMaskExpansionRule : BoardRule
    //{
    //    public MaskPasteMaskExpansionRule()
    //    {
    //        Category = RuleCategory.Mask;

    //        Expansion = 0;
    //    }

    //    public override string RuleType
    //    {
    //        get
    //        {
    //            return "Paste Mask Expansion";
    //        }
    //    }

    //    public double Expansion { get; set; }

    //    public override AbstractBoardRule CreateRuleItem()
    //    {
    //        return new MaskPasteMaskExpansionRuleModel
    //        {
    //            Expansion = Expansion
    //        };
    //    }
    //}

    #endregion

    #region Plane Rules

    /*
    public class PlanePowerPlaneConnectStyleRule : BoardRule
    {
        public PlanePowerPlaneConnectStyleRule()
        {
            Category = RuleCategory.Plane;

            Expansion = 0.5;
            AirGap = 0.254;
            ConductorWidth = 0.254;
        }

        public override string RuleType
        {
            get
            {
                return "Power Plane Connect Style";
            }
        }

        public double Expansion { get; set; }

        public double AirGap { get; set; }

        public double ConductorWidth { get; set; }

        public override AbstractBoardRule CreateRuleItem()
        {
            return new PlanePowerPlaneConnectStyleRuleModel
            {
                Expansion = Expansion,
                AirGap = AirGap,
                ConductorWidth = ConductorWidth
            };
        }
    }

    public class PlanePowerPlaneClearanceRule : BoardRule
    {
        public PlanePowerPlaneClearanceRule()
        {
            Category = RuleCategory.Plane;

            Clearance = 0.5;
        }

        public override string RuleType
        {
            get
            {
                return "Power Plane Clearance";
            }
        }

        public double Clearance { get; set; }

        public override AbstractBoardRule CreateRuleItem()
        {
            return new PlanePowerPlaneClearanceRuleModel
            {
                Clearance = Clearance,
            };
        }
    }

    public class PlanePolygonConnectStyleRule : BoardRule
    {
        public PlanePolygonConnectStyleRule()
        {
            Category = RuleCategory.Plane;

            AirGapWidth = 0.254;
        }

        public override string RuleType
        {
            get
            {
                return "Polygon Connect Style";
            }
        }

        public double AirGapWidth { get; set; }


        public override AbstractBoardRule CreateRuleItem()
        {
            return new PlanePolygonConnectStyleRuleModel
            {
                AirGapWidth = AirGapWidth,
            };
        }
    }
    */

    #endregion

    #region Manufacturing Rules

    /*
public class ManufacturingMinAnnularRingRule : BoardRule
{
    public ManufacturingMinAnnularRingRule()
    {
        //Category = RuleCategory.Manufacturing;

        Value = 0.2;//mm
    }

    //public override string RuleType
    //{
    //    get
    //    {
    //        return "Minimum Annular Ring";
    //    }
    //}

    [XmlAttribute("value")]
    public double Value { get; set; }

    public override AbstractBoardRule CreateRuleItem()
    {
        return new ManufacturingMinAnnularRingRuleModel
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
}
*/

    public class ManufacturingHoleSizeRule : BoardRule
    {
        public ManufacturingHoleSizeRule()
        {
            Min = 0.3;
            Max = 6;
        }


        [XmlAttribute("min")]
        public double Min { get; set; } = 0.3;

        [XmlAttribute("max")]
        public double Max { get; set; } = 6;

        //public override AbstractBoardRule CreateRuleItem()
        //{
        //    return new ManufacturingHoleSizeRuleModel
        //    {
        //        //abstract
        //        Id = Id,
        //        Name = Name,
        //        Comment = Comment,
        //        IsEnabled = IsEnabled,
        //        Priority = Priority,

        //        Min = Min,
        //        Max = Max,
        //    };
        //}
    }

    public class ManufacturingClearanceRule : BoardRule
    {
        public ManufacturingClearanceRule()
        {
            Value = 0.254;
        }

        [XmlAttribute("clearanceType")]
        public ManufacturingClearanceRuleType ClearanceType { get; set; } = ManufacturingClearanceRuleType.HoleToHoleClearance;

        [XmlAttribute("value")]
        public double Value { get; set; } = 0.254;

        //public override AbstractBoardRule CreateRuleItem()
        //{
        //    return new ManufacturingClearanceRuleModel
        //    {
        //        //abstract
        //        Id = Id,
        //        Name = Name,
        //        Comment = Comment,
        //        IsEnabled = IsEnabled,
        //        Priority = Priority,

        //        Value = Value,
        //        ClearanceType = ClearanceType
        //    };
        //}
    }

    public enum ManufacturingClearanceRuleType
    {
        Unknown,
        HoleToHoleClearance,
        SolderMaskClearance,
        SilkToSolderMaskClearance,
        SilkToSilkClearance,
        BoardOutlineClearance
    }



    #endregion

    /*
    public class MatchLengthsRule : BoardRule
    {
        public MatchLengthsRule()
        {
            // Category = RuleCategory.Routing;
        }

        /// <summary>
        /// tolerance value to match in milimeters
        /// </summary>
        [XmlAttribute("tolerance")]
        public double Tolerance { get; set; }

        public RuleFilterData RuleFilterObject { get; set; } = new RuleFilterData();

        public override AbstractBoardRule CreateRuleItem()
        {
            var r = new MatchLengthsRuleModel
            {
                //abstract
                Id = Id,
                Name = Name,
                Comment = Comment,
                IsEnabled = IsEnabled,
                Priority = Priority,
                Tolerance = Tolerance,
            };

            r.Filter.RuleFilterObject = RuleFilterObject;
            return r;
        }
    }
    */

    public class GroupRule : BoardRule
    {
        //Electrical
        [XmlArrayItem("electricalClearance", typeof(ElectricalClearanceRule))]
        //Routing
        [XmlArrayItem("trackWidth", typeof(TrackWidthRule))]
        [XmlArrayItem("viaDefinition", typeof(ViaDefinitionRule))]
        //Mask
        //[XmlArrayItem("MaskExpansion", typeof(MaskExpansionRule))]
        //Manufacturing
        //[XmlArrayItem("ManufacturingMinAnnularRing", typeof(ManufacturingMinAnnularRingRule))]
        [XmlArrayItem("manufacturingHoleSize", typeof(ManufacturingHoleSizeRule))]
        [XmlArrayItem("manufacturingClearance", typeof(ManufacturingClearanceRule))]
        //[XmlArrayItem("MatchLengths", typeof(MatchLengthsRule))]
        [XmlArrayItem("group", typeof(GroupRule))]
        public List<BoardRule> Children { get; set; } = new List<BoardRule>();

        [XmlElement("ruleFilterObject")]
        public RuleFilterData RuleFilterObject { get; set; } = new RuleFilterData();

    }

    #region Placement Rules

    /*
    public class PlacementComponentClearanceRule : BoardRule
    {
        public PlacementComponentClearanceRule()
        {
            Category = RuleCategory.Placement;

            MinVerticalClearance = 0.254;
            MinHorizontalClearance = 0.254;
        }

        public override string RuleType
        {
            get
            {
                return "ComponentClearance";
            }
        }

        public double MinVerticalClearance { get; set; }
        public double MinHorizontalClearance { get; set; }

        public override AbstractBoardRule CreateRuleItem()
        {
            return new PlacementComponentClearanceRuleModel
            {
                MinVerticalClearance = MinVerticalClearance,
                MinHorizontalClearance = MinHorizontalClearance,
            };
        }
    }

    public class PlacementHeightRule : BoardRule
    {
        public PlacementHeightRule()
        {
            Category = RuleCategory.Placement;

            Min = 0;
            Preferred = 12.7;
            Max = 25.4;
        }

        public override string RuleType
        {
            get
            {
                return "Height";
            }
        }

        public double Min { get; set; }
        public double Max { get; set; }

        public double Preferred { get; set; }

        public override AbstractBoardRule CreateRuleItem()
        {
            return new PlacementHeightRuleModel
            {
                Min = Min,
                Max = Max,
                Preferred = Preferred,
            };
        }
    }
    */

    #endregion

    #endregion Rules

    public enum RuleLayerFilter
    {
        All,
        PerLayer
    }

    public class LayerMinMaxDefaultSpec : BaseViewModel
    {
        long layerId;

        [XmlAttribute("layerId")]
        public long LayerId
        {
            get { return layerId; }
            set
            {
                layerId = value;
                OnPropertyChanged(nameof(LayerId));
            }
        }

        string layerName;

        [XmlIgnore]
        public string LayerName
        {
            get { return layerName; }
            set
            {
                layerName = value;
                OnPropertyChanged(nameof(LayerName));
            }
        }

        double min = 0.254;

        [XmlAttribute("min")]
        public double Min
        {
            get { return min; }
            set
            {
                min = value;
                OnPropertyChanged(nameof(Min));
            }
        }

        double max = 0.254;

        [XmlAttribute("max")]
        public double Max
        {
            get { return max; }
            set
            {
                max = value;
                OnPropertyChanged(nameof(Max));
            }
        }

        double _default = 0.254;

        [XmlAttribute("default")]
        public double Default
        {
            get { return _default; }
            set
            {
                _default = value;
                OnPropertyChanged(nameof(Default));
            }
        }
    }

    public class RuleFilterData : BaseViewModel
    {
        RuleFilterObjectType objectType;
        [XmlAttribute("objectType")]
        public RuleFilterObjectType ObjectType
        {
            get { return objectType; }
            set
            {
                objectType = value;
                OnPropertyChanged(nameof(ObjectType));
            }
        }

        long objectId;
        [XmlAttribute("objectId")]
        public long ObjectId
        {
            get { return objectId; }
            set
            {
                objectId = value;
                OnPropertyChanged(nameof(ObjectId));
            }
        }

        string objectName;

        [XmlAttribute("objectName")]
        public string ObjectName
        {
            get { return objectName; }
            set
            {
                objectName = value;
                OnPropertyChanged(nameof(ObjectName));
            }
        }
    }

    public enum RuleFilterObjectType
    {
        All,
        AllFiltered,
        Net,
        NetClass,
        NetClassGroup,
        DrillPair
    }
}
