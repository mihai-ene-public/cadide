namespace IDE.Core.Model.Gerber.Primitives.Attributes;

public enum PartFileGerberAttributeValue
{
    /// <summary>
    /// Single PCB
    /// </summary>
    Single,

    /// <summary>
    /// Customer panel, assembly panel, shipping panel, biscuit
    /// </summary>
    Array,

    /// <summary>
    /// Working panel, production panel
    /// </summary>
    FabricationPanel,

    /// <summary>
    /// A test coupon
    /// </summary>
    Coupon,

    Other
}
