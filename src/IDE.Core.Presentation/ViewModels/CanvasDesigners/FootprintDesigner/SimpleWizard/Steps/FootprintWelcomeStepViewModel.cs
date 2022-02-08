using IDE.Core.Wizards;

namespace IDE.Documents.Views
{
    public class FootprintWelcomeStepViewModel: WizardStepViewModelBase<FootprintWizardItem>
    {
        public FootprintWelcomeStepViewModel(FootprintWizardItem c)
            : base( c )
        {
        }

        public override string DisplayName
        {
            get { return "Welcome"; }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
