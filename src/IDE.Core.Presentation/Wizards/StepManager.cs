using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace IDE.Core.Wizards
{

    public class StepManager<WizardBusinessObject>
    {
        private LinkedListNode<CompleteStep<WizardBusinessObject>> _currentLinkedListStep;
        private bool _reconfiguringRoute;
        private List<CompleteStep<WizardBusinessObject>> _steps;
        private LinkedList<CompleteStep<WizardBusinessObject>> _linkedSteps;

        public LinkedListNode<CompleteStep<WizardBusinessObject>> CurrentLinkedListStep
        {
            get
            {
                return _currentLinkedListStep;
            }
            set
            {
                _currentLinkedListStep = value;
                if ( ( _linkedSteps.First == _currentLinkedListStep ) && !_reconfiguringRoute )
                {
                    ResetRoute();
                }
            }
        }

        public List<CompleteStep<WizardBusinessObject>> Steps { get { return _steps; } }

        public LinkedListNode<CompleteStep<WizardBusinessObject>> FirstStep
        {
            get
            {
                return _linkedSteps == null ? null : _linkedSteps.First;
            }
        }

        public void ProvideSteps( List<CompleteStep<WizardBusinessObject>> steps )
        {
            _steps = steps;
            _linkedSteps = new LinkedList<CompleteStep<WizardBusinessObject>>( _steps );
            CurrentLinkedListStep = _linkedSteps.First;
        }

        public void ReworkListBasedOn( RouteModifier rm )
        {
            if ( rm == null )
            {
                return;
            }
            _reconfiguringRoute = true;
            ReorganizeLinkedList( rm );
            ResetListRelevancy();
            _reconfiguringRoute = false;
        }

        /// <summary>
        /// Each step in the wizard may modify the route, but it's assumed that if the user goes back to step one, the route initializes back to the way it
        /// was when it was created.
        /// </summary>
        private void ResetRoute()
        {
            var allStepViewTypes = _linkedSteps.ToList().ConvertAll( s => s.ViewModel.GetType() );
            ReworkListBasedOn( new RouteModifier() { IncludeViewModelTypes = allStepViewTypes } );
        }

        /// <summary>
        /// At this point, if a step is in the linked list, it's relevant; if not, it's not.
        /// </summary>
        private void ResetListRelevancy()
        {
            _steps.ForEach( s => s.Relevant = false );
            var linkedStep = _linkedSteps.First;
            while ( linkedStep != null )
            {
                linkedStep.Value.Relevant = true;
                linkedStep = linkedStep.Next;
            }
        }

        /// <summary>
        /// Re-create the linked list to reflect the new "workflow."
        /// </summary>
        /// <param name="nextStep"></param>
        private void ReorganizeLinkedList( RouteModifier rm )
        {
            var cacheCurrentStep = CurrentLinkedListStep.Value;
            var newSubList = CreateNewStepList( rm );

            /// Re-create linked list.
            _linkedSteps = new LinkedList<CompleteStep<WizardBusinessObject>>( newSubList );
            ResetCurrentLinkedListStepTo( cacheCurrentStep );
        }

        private List<CompleteStep<WizardBusinessObject>> CreateNewStepList( RouteModifier rm )
        {
            var result = new List<CompleteStep<WizardBusinessObject>>( _linkedSteps );

            EnsureNotModifyingCurrentStep( rm );

            if ( rm.ExcludeViewModelTypes != null )
            {
                rm.ExcludeViewModelTypes.ForEach( t => result.RemoveAll( step => step.ViewModel.GetType().Equals( t ) ) );
            }
            if ( rm.IncludeViewModelTypes != null )
            {
                AddBack( result, rm.IncludeViewModelTypes );
            }

            return result;
        }

        private void EnsureNotModifyingCurrentStep( RouteModifier rm )
        {
            Func<Type, bool> currentStepCondition = t => t == CurrentLinkedListStep.Value.ViewModel.GetType();
            if ( rm.ExcludeViewModelTypes != null )
            {
                Contract.Ensures( rm.ExcludeViewModelTypes.FirstOrDefault( currentStepCondition ) == null );
            }
            if ( rm.IncludeViewModelTypes != null )
            {
                Contract.Ensures( rm.IncludeViewModelTypes.FirstOrDefault( currentStepCondition ) == null );
            }
        }

        /// <summary>
        /// OMG, if the user chooses an option that changes the route through the wizard, then goes back and chooses a different option,
        /// we need to add the appropriate step(s) back into the workflow.
        /// </summary>
        /// <param name="workingStepList"></param>
        /// <param name="viewTypes"></param>
        private void AddBack( List<CompleteStep<WizardBusinessObject>> workingStepList, List<Type> viewTypes )
        {
            foreach ( var vt in viewTypes )
            {
                // Find the step to add back in the main list of steps.
                var stepToAddBack = _steps.Where( s => s.ViewModel.GetType() == vt ).FirstOrDefault();
                if ( !workingStepList.Contains( stepToAddBack ) )
                {
                    // Re-insert the step into our working list (which will become the wizard's new linked list).
                    if ( stepToAddBack != null )
                    {
                        int indexOfStepToAddBack = _steps.IndexOf( stepToAddBack );
                        // If it belongs at the head of the list, add it there.
                        if ( indexOfStepToAddBack == 0 )
                        {
                            workingStepList.Insert( 0, stepToAddBack );
                            continue;
                        }
                        else
                        {
                            /// Otherwise we have to find the previous step in the main list, find that step in our working list and add in
                            /// the step after that step.
                            var stepReinserted = false;
                            var countOfStepsToPreviousFoundStep = 1;
                            while ( !stepReinserted )
                            {
                                var previousStep = _steps[indexOfStepToAddBack - countOfStepsToPreviousFoundStep];
                                for ( int i = 0; i < workingStepList.Count; i++ )
                                {
                                    if ( workingStepList[i].ViewModel.GetType() == previousStep.ViewModel.GetType() )
                                    {
                                        workingStepList.Insert( i + 1, stepToAddBack );
                                        stepReinserted = true;
                                    }
                                }
                                // The previous step wasn't found; continue to the next previous step.
                                countOfStepsToPreviousFoundStep++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Must maintain the current step reference (this re-creating of the linked list happens when the user makes a selection on
        /// the current step).
        /// After recreating the list, our CurrentLinkedListStep reference would be referring to an item in the old linked list.
        /// </summary>
        /// <param name="cacheCurrentStep"></param>
        private void ResetCurrentLinkedListStepTo( CompleteStep<WizardBusinessObject> cacheCurrentStep )
        {
            CurrentLinkedListStep = _linkedSteps.First;
            while ( CurrentLinkedListStep.Value != cacheCurrentStep )
            {
                if ( CurrentLinkedListStep.Next == null )
                {
                    throw new Exception( "Error resetting current step after reorganizing steps." );
                }
                CurrentLinkedListStep = CurrentLinkedListStep.Next;
            }
        }

    }

}
