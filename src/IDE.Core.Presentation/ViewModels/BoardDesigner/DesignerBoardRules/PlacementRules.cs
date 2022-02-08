namespace IDE.Core.Designers
{
    /* we remove these for now with the purpose to be re-added later (maybe version 2)
    public class PlacementComponentClearanceRuleModel : AbstractBoardRule
    {
        public PlacementComponentClearanceRuleModel()
        {
            Category = RuleCategory.Placement;

            MinVerticalClearance = 0.254;
            MinHorizontalClearance = 0.254;
        }

        public override string RuleType
        {
            get
            {
                return "Component clearance";
            }
        }

        double minVerticalClearance;
        public double MinVerticalClearance
        {
            get
            {
                return minVerticalClearance;
            }
            set
            {
                minVerticalClearance = value;
                OnPropertyChanged(nameof(MinVerticalClearance));
            }
        }

        double minHorizontalClearance;
        public double MinHorizontalClearance
        {
            get
            {
                return minHorizontalClearance;
            }
            set
            {
                minHorizontalClearance = value;
                OnPropertyChanged(nameof(MinHorizontalClearance));
            }
        }
    }

    public class PlacementHeightRuleModel : AbstractBoardRule
    {
        public PlacementHeightRuleModel()
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

        double preferred;
        public double Preferred
        {
            get
            {
                return preferred;
            }
            set
            {
                preferred = value;
                OnPropertyChanged(nameof(Preferred));
            }
        }
    }
    */
}
