using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using IDE.Core.Commands;

namespace IDE.Core.Wizards
{

    public delegate void NextEventHandler(object currentStep);

    /// <summary>
    /// The main ViewModel class for the wizard.  This class contains the various steps shown in the workflow and provides navigation between the steps.
    /// </summary>
    /// <typeparam name="WizardBusinessObject">The object the wizard models.  Must have parameterless constructor because we will create it within.</typeparam>
    public class WizardViewModel<WizardBusinessObject> : DialogViewModel, IWizardViewModel where WizardBusinessObject : IWizardBusinessObject, new()
    {

        private readonly WizardBusinessObject _businessObject;
        private readonly StepManager<WizardBusinessObject> _stepManager;
        private ICommand _moveNextCommand;
        private ICommand _movePreviousCommand;
        private ICommand _cancelCommand;

        //public event NextEventHandler NextEvent;
        // public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This is here so UI can disable stuff when the wizard is doing whatever it's intended to do, e.g., Running an import.
        /// </summary>
        public bool ExecutingFinalAction { get; set; }

        /// <summary>
        /// Referenced only in xaml
        /// </summary>
        public ReadOnlyCollection<CompleteStep<WizardBusinessObject>> Steps
        {
            get
            {
                return new ReadOnlyCollection<CompleteStep<WizardBusinessObject>>(_stepManager.Steps);
            }
        }

        /// <summary>
        /// Returns the step ViewModel that the user is currently viewing.
        /// </summary>
        /// <summary>
        /// Returns the business object the wizard is building.  If this returns null, the user canceled.
        /// </summary>
        public WizardBusinessObject BusinessObject
        {
            get { return _businessObject; }
        }

        public LinkedListNode<CompleteStep<WizardBusinessObject>> CurrentLinkedListStep
        {
            get { return _stepManager.CurrentLinkedListStep; }
            private set
            {
                if (value == _stepManager.CurrentLinkedListStep)
                {
                    return;
                }

                ActionsOnCurrentLinkedListStep(value);

                OnPropertyChanged("CurrentLinkedListStep");
                OnPropertyChanged("IsOnLastStep");
            }
        }

        public WizardViewModel()
        {
            _stepManager = new StepManager<WizardBusinessObject>();
            _businessObject = Activator.CreateInstance<WizardBusinessObject>();
            ExecutingFinalAction = false;
        }

        public void ProvideSteps(List<CompleteStep<WizardBusinessObject>> steps)
        {
            _stepManager.ProvideSteps(steps);
            ActionsOnCurrentLinkedListStep(_stepManager.FirstStep);
        }

        /// <summary>
        /// Business object may need to release resources or something.
        /// </summary>
        public void Cancel()
        {
            _businessObject.Cancel();
        }

        /// <summary>
        /// Returns the command which, when executed, cancels the order and causes the Wizard to be removed from the user interface.
        /// </summary>
        public new ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = CreateCommand((p) => Cancel());
                }
                return _cancelCommand;
            }
        }

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentLinkedListStep
        /// property to reference the previous step in the workflow.
        /// </summary>
        public ICommand MovePreviousCommand
        {
            get
            {
                if (_movePreviousCommand == null)
                {
                    _movePreviousCommand = CreateCommand((p) => MoveToPreviousStep(), (p) => CanMoveToPreviousStep);
                }
                return _movePreviousCommand;
            }
        }

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentLinkedListStep property to reference the next step in the workflow.  If the user
        /// is viewing the last step in the workflow, this causes the Wizard to finish and be removed from the user interface.
        /// </summary>
        public ICommand MoveNextCommand
        {
            get
            {
                if (_moveNextCommand == null)
                {
                    _moveNextCommand = CreateCommand((p) => MoveToNextStep(), (p) => CanMoveToNextStep);
                }
                return _moveNextCommand;
            }
        }

        bool CanMoveToPreviousStep
        {
            get { return CurrentLinkedListStep.Previous != null; }
        }

        void MoveToPreviousStep()
        {
            if (CanMoveToPreviousStep)
            {
                CurrentLinkedListStep = CurrentLinkedListStep.Previous;
                //CurrentLinkedListStep.Value.ViewModel.BeforeShow();
            }
        }

        bool CanMoveToNextStep
        {
            get
            {
                var step = CurrentLinkedListStep;
                return (step != null) && (step.Value.ViewModel.IsValid()) && (step.Next != null);
            }
        }

        /// <summary>
        /// Note that currently, the step OnNext handler is only called when moving next; not when moving previous.
        /// </summary>
        void MoveToNextStep()
        {
            if (CanMoveToNextStep)
            {
                var routeModifier = CurrentLinkedListStep.Value.ViewModel.OnNext();
                _stepManager.ReworkListBasedOn(routeModifier);
                CurrentLinkedListStep = GetCurrentLinkedListStep(routeModifier);
                //CurrentLinkedListStep.Value.ViewModel.BeforeShow();
                CurrentLinkedListStep.Value.Visited = true;
            }
        }

        private LinkedListNode<CompleteStep<WizardBusinessObject>> GetCurrentLinkedListStep(RouteModifier rm)
        {
            if ((rm == null) || (rm.JumpToPriorStepViewType == null))
            {
                return CurrentLinkedListStep.Next;
            }
            else
            {
                var previousStep = CurrentLinkedListStep.Previous;
                while (previousStep != null)
                {
                    if (previousStep.Value.ViewModel.GetType() == rm.JumpToPriorStepViewType)
                    {
                        return previousStep;
                    }
                    previousStep = previousStep.Previous;
                }
                throw new Exception("Wizard was directed to non-existent prior step");
            }
        }

        private void ActionsOnCurrentLinkedListStep(LinkedListNode<CompleteStep<WizardBusinessObject>> step)
        {
            if (CurrentLinkedListStep != null)
            {
                CurrentLinkedListStep.Value.ViewModel.IsCurrentStep = false;
            }

            _stepManager.CurrentLinkedListStep = step;

            if (step != null)
            {
                step.Value.ViewModel.IsCurrentStep = true;
                step.Value.ViewModel.BeforeShow();
            }
        }

        /// <summary>
        /// Returns true if the user is currently viewing the last step in the workflow.
        /// </summary>
        public bool IsOnLastStep
        {
            get { return CurrentLinkedListStep.Next == null; }
        }

        //private void OnPropertyChanged(string propertyName)
        //{
        //    var handler = PropertyChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

    }

}
