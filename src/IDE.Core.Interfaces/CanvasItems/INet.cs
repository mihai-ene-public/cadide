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
        long Id { get; set; }
        string Name { get; set; }
    }


}