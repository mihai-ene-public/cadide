namespace IDE.Core.Interfaces
{
    public interface IPadRef
    {
        long FootprintInstanceId { get; set; }
        string PadNumber { get; set; }
    }

    //public interface IPadRefDesignerItem : ISignalPrimitiveCanvasItem
    //{
    //    IPadRef Pad { get; set; }

    //    IPadCanvasItem PadDesignerItem { get; }
    //}
}