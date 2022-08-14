namespace IDE.Core.Model.GlobalRepresentation
{
    public enum GlobalStandardPrimitiveTag
    {
        //AperFunction
       Role,

        //Object
        NetName,
        PartName,
        PinNumber
    }

    public enum GlobalStandardPrimitiveRole
    {
        PadTht,
        PadSmd,
        Via,
        Track,
        Text,
        BoardOutline,

        //component
        //PartPosition,
        //PinPosition,

        ComponentOutlineBody,
        ComponentOutlineLead2Lead,
        ComponentOutlineFootprint,
        ComponentOutlineCourtyard,
    }
}
