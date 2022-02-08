using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IDE.Core.Wizards
{

    public class BinaryDecisionHelper
    {

        private RouteOptionGroupViewModel<bool> _binaryDecisionGroup;

        /// <summary>
        /// This is for use with a simple yes/no decision step
        /// </summary>
        public void ConfigureBinaryDecision( string displayName = "", bool? defaultChoice = null, Action<object, PropertyChangedEventArgs> actionOnSelection = null )
        {
            var list = new List<RouteOptionViewModel<bool>>();
            list.Add( new RouteOptionViewModel<bool>( true, 0 ) );
            /// If they choose no, we skip the steps passed in for yes.  The StepManager puts these steps back in if they choose yes.
            /// I guess I haven't yet come across the need to skip steps for a no selection.  Could add that param.
            list.Add( new RouteOptionViewModel<bool>( false, 1 ) );
            if ( defaultChoice.HasValue )
            {
                list.ForEach( ro =>
                {
                    if ( ro.GetValue() == defaultChoice.Value )
                    {
                        ro.IsSelected = true;
                    }
                    if ( actionOnSelection != null )
                    {
                        ro.PropertyChanged += ( s, e ) => actionOnSelection( s, e );
                    }
                } );
            }
            _binaryDecisionGroup = new RouteOptionGroupViewModel<bool>( displayName ) { OptionModels = new ReadOnlyCollection<RouteOptionViewModel<bool>>( list ) };
        }

        /// <summary>
        /// Pre: SimpleYesNoDecisionHasBeenMade() has returned true
        /// </summary>
        /// <returns></returns>
        public bool GetValueOfBinaryDecision()
        {
            return (bool)_binaryDecisionGroup.OptionModels.Where( rovm => rovm.IsSelected ).First().GetValue();
        }

        public bool BinaryDecisionHasBeenMade()
        {
            return _binaryDecisionGroup.OptionModels.Where( rovm => rovm.IsSelected ).FirstOrDefault() != null;
        }

        public RouteOptionGroupViewModel<bool> BinaryDecisionGroup
        {
            get
            {
                return _binaryDecisionGroup;
            }
        }

    }

}
