using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;

namespace IDE.Core.Designers
{
    public abstract class AbstractBoardRule : BaseViewModel, IBoardRuleModel
    {
        public AbstractBoardRule()
        {
            GeometryHelper = ServiceProvider.Resolve<IGeometryHelper>();
            Id = LibraryItem.GetNextId();
        }

        protected IGeometryHelper GeometryHelper;

        public const double ClearanceTolerance = 1e-3;

        /// <summary>
        /// Current board FileViewModel
        /// </summary>
        public ILayeredViewModel Document { get; set; }

        public IGroupBoardRuleModel Parent { get; set; }

        long id;
        public long Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        string name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    name = RuleType;

                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }


        string comment;
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        bool isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        int priority = 1;
        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }

        public abstract string RuleType { get; }



        public abstract void Load(ILayeredViewModel doc);

        public abstract BoardRule SaveToBoardRule();

        public abstract bool RuleAppliesToItem(ISelectableItem item);

        public abstract bool RuleAppliesToItemsPair(ISelectableItem item1, ISelectableItem item2);

        public abstract bool IsPairedRule();

        public abstract void LoadFromData(IBoardRuleData rule);

        public virtual bool CheckItem(ISelectableItem item, RuleCheckResult result)
        {
            return true;

        }

        public virtual bool CheckItems(ISelectableItem item1, ISelectableItem item2, RuleCheckResult result)
        {
            return true;
        }

       

       
    }




}
