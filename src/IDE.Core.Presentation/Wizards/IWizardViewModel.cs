using System.Windows.Input;

namespace IDE.Core.Wizards
{
    public interface IWizardViewModel
    {
        ICommand MoveNextCommand { get; }
        ICommand MovePreviousCommand { get; }
        bool IsOnLastStep { get; }
        bool ExecutingFinalAction { get; set; }
        void Cancel();
    }
}
