using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.ViewModels;

public abstract class UniqueNameManager<T> : IUniqueNameManager<T> where T : IUniqueName
{
    List<T> elements = new List<T>();
    public IList<T> Elements => elements.AsReadOnly();

    protected abstract string GetPrefix();

    protected abstract T CreateInstance();

    public T AddNew()
    {
        var names = elements.Select(n => n.Name).ToList();
        var newName = UniqueNameHelper.GetNextUniqueName(names, GetPrefix());

        var newElement = AddNew(newName);

        return newElement;
    }

    private T AddNew(string name)
    {
        var element = CreateInstance();
        element.Id = LibraryItem.GetNextId();
        element.Name = name;

        elements.Add(element);
        OnElementAdded(element);

        return element;
    }

    protected virtual void OnElementAdded(T element)
    {

    }

    public T Add(T element)
    {
        var item = elements.FirstOrDefault(n => n.Name == element.Name);

        if (item == null)
        {
            item = element;
            elements.Add(element);
            OnElementAdded(element);
        }

        return item;
    }

    public T Add(string name)
    {
        var element = elements.FirstOrDefault(n => n.Name == name);
        if (element == null)
        {
            element = AddNew(name);
        }

        return element;
    }

    public T Get(string name)
    {
        return elements.FirstOrDefault(n => n.Name == name);
    }

    public void Remove(T element)
    {
        elements.Remove(element);
        OnElementRemoved(element);
    }

    protected virtual void OnElementRemoved(T element) { }
}
