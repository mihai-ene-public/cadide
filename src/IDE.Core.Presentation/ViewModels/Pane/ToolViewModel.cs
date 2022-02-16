namespace IDE.Core.ViewModels
{
    using IDE.Core.Interfaces;

    /// <summary>
    /// AvalonDock base class viewmmodel to support tool window views.
    /// </summary>
    public abstract class ToolViewModel : PaneViewModel, IToolWindow
    {
        private bool isVisible = false;
        private bool canHide = false;

        #region constructors
        /// <summary>
        /// Base constructor from name of tool window item
        /// </summary>
        /// <param name="name">Name of tool window displayed in GUI</param>
        public ToolViewModel(string name)
        {
            Name = name;
            Title = name;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a displayable name of this item.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        public bool IsVisible
        {
            get
            {
                return isVisible;
            }

            set
            {
                //if (this.mIsVisible != value)
                {
                    isVisible = value;

                    if (value)         // Switching visibility on should switch hide mode off
                        canHide = false;

                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        public bool CanHide
        {
            get
            {
                return canHide;
            }

            set
            {
                //if (this.mCanHide != value)
                {
                    canHide = value;
                    OnPropertyChanged(nameof(CanHide));
                }
            }
        }

        public abstract PaneLocation PreferredLocation { get; }

        public virtual double PreferredWidth
        {
            get { return 250; }
        }

        public virtual double PreferredHeight
        {
            get { return 200; }
        }

        #endregion properties

       
    }
}
