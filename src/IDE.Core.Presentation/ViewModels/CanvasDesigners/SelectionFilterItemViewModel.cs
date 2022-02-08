using System;
using IDE.Core;

namespace IDE.Documents.Views
{
    public class SelectionFilterItemViewModel : BaseViewModel
    {
        public string DisplayName { get; set; }

        public string TooltipText { get; set; }

        public Type Type { get; set; }

        bool canSelect = true;
        public bool CanSelect
        {
            get { return canSelect; }
            set
            {
                canSelect = value;
                OnPropertyChanged(nameof(CanSelect));
            }
        }
    }
}
