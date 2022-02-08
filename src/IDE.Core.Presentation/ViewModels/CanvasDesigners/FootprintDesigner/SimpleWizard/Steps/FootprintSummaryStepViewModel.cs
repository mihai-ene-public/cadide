using IDE.Core.Wizards;

namespace IDE.Documents.Views
{
    public class FootprintSummaryStepViewModel : WizardStepViewModelBase<FootprintWizardItem>
    {
        public FootprintSummaryStepViewModel(FootprintWizardItem c)
            : base( c )
        {
        }

        public override string DisplayName
        {
            get { return "Summary"; }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
