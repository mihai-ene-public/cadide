using IDE.Core.Interfaces;
using System;

namespace IDE.Documents.Views;

public class ToolBoxItem : IToolboxItem
{
    public string Name { get; set; }

    public string TooltipText { get; set; }

    /// <summary>
    /// Type of placement tool
    /// This will get initialized with Activator.CreateInstance
    /// </summary>
    public Type PlacementToolType { get; set; }

    public Type Type { get; set; }
}
