using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace IDE.Core.Designers;

/// <summary>
/// net on a canvas
/// </summary>
public class SchematicNet : BaseViewModel, ISchematicNet
{
    public string Id { get; set; }

    string name;
    public string Name
    {
        get { return name; }
        set
        {
            if (name == value)
                return;
            name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    public string ClassId { get; set; }

    bool isHighlighted;
    public bool IsHighlighted
    {
        get { return isHighlighted; }
        set
        {
            if (isHighlighted == value) return;
            isHighlighted = value;
            OnPropertyChanged(nameof(IsHighlighted));
        }
    }

    public bool IsNamed()
    {
        return name != null && !name.StartsWith("Net");
    }

    //segments (netlabel, junction, pinref, wire)
    public IList<NetSegmentCanvasItem> NetItems { get; set; } = new List<NetSegmentCanvasItem>();

    public void HighlightNet(bool newHighlight)
    {
        IsHighlighted = newHighlight;
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(Name))
            return "not assigned";
        return Name;
    }

}

public interface ISchematicNet : IUniqueName, INotifyPropertyChanged
{
    bool IsHighlighted { get; set; }
    IList<NetSegmentCanvasItem> NetItems { get; }
    void HighlightNet(bool newHighlight);
}


