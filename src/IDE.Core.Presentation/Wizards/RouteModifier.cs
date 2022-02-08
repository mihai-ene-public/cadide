using System;
using System.Collections.Generic;

namespace IDE.Core.Wizards
{

    public class RouteModifier
    {
        public List<Type> ExcludeViewModelTypes { get; set; }
        public List<Type> IncludeViewModelTypes { get; set; }

        /// <summary>
        /// If you need to jump to a different *subsequent* step, you would just add the appropriate view types to ExcludeViewTypes.
        /// </summary>
        public Type JumpToPriorStepViewType { get; set; }

        public void AddToExcludeViews( List<Type> viewTypes )
        {
            if ( ExcludeViewModelTypes == null )
            {
                ExcludeViewModelTypes = viewTypes;
            }
            else
            {
                ExcludeViewModelTypes.AddRange( viewTypes );
            }
        }

        public void AddToExcludeViews( Type viewType )
        {
            if ( ExcludeViewModelTypes == null )
            {
                ExcludeViewModelTypes = new List<Type> { viewType };
            }
            else
            {
                ExcludeViewModelTypes.Add( viewType );
            }
        }

        public void AddToIncludeViews( List<Type> viewTypes )
        {
            if ( IncludeViewModelTypes == null )
            {
                IncludeViewModelTypes = viewTypes;
            }
            else
            {
                IncludeViewModelTypes.AddRange( viewTypes );
            }
        }

        public void AddToIncludeViews( Type viewType )
        {
            if ( IncludeViewModelTypes == null )
            {
                IncludeViewModelTypes = new List<Type> { viewType };
            }
            else
            {
                IncludeViewModelTypes.Add( viewType );
            }
        }

    }

}
