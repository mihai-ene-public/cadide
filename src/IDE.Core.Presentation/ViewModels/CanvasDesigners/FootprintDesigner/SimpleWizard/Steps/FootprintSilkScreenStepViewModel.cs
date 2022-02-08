using IDE.Core.Wizards;

namespace IDE.Documents.Views
{
    public class FootprintSilkScreenStepViewModel : WizardStepViewModelBase<FootprintWizardItem>
    {
        public FootprintSilkScreenStepViewModel(FootprintWizardItem c)
            : base( c )
        {
        }

        public override string DisplayName
        {
            get { return "Silkscreen"; }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
