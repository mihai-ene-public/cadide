namespace IDE.Core.Wizards
{

    /// <summary>
    /// Wizard flow is determined by the RouteModifier object returned from the OnNext method of each step view model.
    /// The main remaining use of this guy is when you have a step that simply asks a yes/no question.  See BinaryDecisionHelper.
    /// </summary>
    public interface IRouteOption
    {
        bool IsSelected { get; set; }
    }

    public class RouteOptionViewModel<TValue> : OptionViewModel<TValue>, IRouteOption
    {
        public RouteOptionViewModel( TValue value ) : base( value ) { }
        public RouteOptionViewModel( TValue value, int sortValue ) : base( value, sortValue ) { }
        public RouteOptionViewModel( TValue value, int sortValue, string tooltip ) : base( value, sortValue, tooltip ) { }
    }

}
