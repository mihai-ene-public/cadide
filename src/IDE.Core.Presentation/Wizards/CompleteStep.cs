using System.ComponentModel;

namespace IDE.Core.Wizards
{

    /// <summary>
    /// For our StepTemplateConverter
    /// </summary>
    public interface IProvideViewType
    {
        //Type ViewType { get; }
    }

    public class CompleteStep<WizardBusinessObject> : INotifyPropertyChanged, IProvideViewType
    {

        private bool _relevant = true;
        private bool _visited;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Relevant
        {
            get
            {
                return _relevant;
            }
            set
            {
                if ( _relevant != value )
                {
                    _relevant = value;
                    OnPropertyChanged( "Relevant" );
                }
            }
        }

        public bool Visited
        {
            get
            {
                return _visited;
            }
            set
            {
                if ( _visited != value )
                {
                    _visited = value;
                    OnPropertyChanged( "Visited" );
                }
            }
        }

        public WizardStepViewModelBase<WizardBusinessObject> ViewModel { get; set; }

        ///// <summary>
        ///// The class type of the actual xaml view to be used for this step
        ///// </summary>
        //public Type ViewType { get; set; }

        private void OnPropertyChanged( string propertyName )
        {
            var handler = PropertyChanged;
            if ( handler != null )
            {
                handler( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }

    }

}
