using IDE.Core.Interfaces;

namespace IDE.Core.Interfaces
{
    public interface IProperty
    {
        string Name { get; set; }
        PropertyType Type { get; set; }
        string Value { get; set; }
    }
}