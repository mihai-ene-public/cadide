namespace IDE.Core.Interfaces
{
    public interface INet : IUniqueName
    {
    }

    public interface IBus : IUniqueName
    {
    }

    public interface IUniqueName
    {
        string Id { get; set; }
        string Name { get; set; }
    }


}