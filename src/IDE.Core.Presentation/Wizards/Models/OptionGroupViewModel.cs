using System.Collections.ObjectModel;

namespace IDE.Core.Wizards
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOption">Type of options that will be in the collection: regular or route options</typeparam>
    /// <typeparam name="TValue">The type of each option</typeparam>
    public abstract class OptionGroupViewModel<TOption, TValue>
    {
        private readonly string _displayName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName">If it's blank, then the display name will be the TValue type (if not bool)</param>
        public OptionGroupViewModel( string displayName )
        {
            if ( displayName == string.Empty )
            {
                _displayName = typeof( TValue ) == typeof( bool ) ? "" : typeof( TValue ).Name.SplitCamelCase();
            }
            else
            {
                _displayName = displayName;
            }
        }

        public string DisplayName { get { return _displayName; } }
        public abstract ReadOnlyCollection<TOption> OptionModels { get; set; }
    }

    public class RegularOptionGroupViewModel<TValue> : OptionGroupViewModel<OptionViewModel<TValue>, TValue>
    {
        public RegularOptionGroupViewModel( string displayName = "" )
            : base( displayName )
        {

        }
        public override ReadOnlyCollection<OptionViewModel<TValue>> OptionModels { get; set; }
    }

    public class RouteOptionGroupViewModel<TValue> : OptionGroupViewModel<RouteOptionViewModel<TValue>, TValue>
    {
        public RouteOptionGroupViewModel( string displayName = "" )
            : base( displayName )
        {

        }
        public override ReadOnlyCollection<RouteOptionViewModel<TValue>> OptionModels { get; set; }
    }

}
