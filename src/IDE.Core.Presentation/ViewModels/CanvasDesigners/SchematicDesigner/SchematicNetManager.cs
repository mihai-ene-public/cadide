using System.ComponentModel;
using IDE.Core.Designers;
using IDE.Core.Interfaces;

namespace IDE.Core.ViewModels;

//holds a list of nets that are unique by name
//when adding a new net the one with the lowest id is kept
public class SchematicNetManager : UniqueNameManager<ISchematicNet>, INetManager
{
    private readonly ISchematicDesigner _schematic;

    public SchematicNetManager(ISchematicDesigner schematic)
    {
        _schematic = schematic;
    }
    protected override string GetPrefix()
    {
        return "Net$";
    }

    protected override ISchematicNet CreateInstance()
    {
        return new SchematicNet();
    }

    protected override void OnElementAdded(ISchematicNet element)
    {
        element.PropertyChanged += Element_PropertyChanged;
    }

    private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ISchematicNet.IsHighlighted))
        {
            _schematic.OnHighlightChanged(_schematic, EventArgs.Empty);
        }
    }

    protected override void OnElementRemoved(ISchematicNet element)
    {
        element.PropertyChanged -= Element_PropertyChanged;
    }
}
