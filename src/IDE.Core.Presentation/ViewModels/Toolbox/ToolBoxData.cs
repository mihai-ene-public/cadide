using IDE.Core.Interfaces;
using System;

namespace IDE.Documents.Views
{


    public class ToolBoxItem : IToolboxItemType
    {

        public string Name { get; set; }

        public string TooltipText { get; set; }

        /// <summary>
        /// Type of placement tool
        /// This will get initialized with Activator.CreateInstance
        /// </summary>
        public Type PlacementToolType { get; set; }

        ///// <summary>
        ///// type of canvas item
        ///// </summary>
        //public Type CanvasItemType { get; set; }

        public Type Type { get; set; }
    }


    public class PrimitiveItem : ToolBoxItem
    {

    }


    //public class PartToolboxItem : ToolBoxItem
    //{
    //    public ComponentItemDisplay Component { get; set; }
    //}

    //public class ToolPlacementItem : ToolBoxItem
    //{

    //}
}
