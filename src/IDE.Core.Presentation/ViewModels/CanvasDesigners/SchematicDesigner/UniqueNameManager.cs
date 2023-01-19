using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core.ViewModels
{
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

            var newElement = CreateInstance();
            newElement.Id = LibraryItem.GetNextId();
            newElement.Name = newName;

            elements.Add(newElement);

            return newElement;
        }


        public T Add(T element)
        {
            var searchNet = elements.FirstOrDefault(n => n.Name == element.Name);

            if (searchNet == null)
            {
                elements.Add(element);
            }
            //else
            //{
            //    if (searchNet.Id <= element.Id)
            //    {
            //        //searchNet is older
            //        return searchNet;
            //    }
            //    else
            //    {
            //        //remove the existing and newer, but add the older one
            //        elements.Remove(searchNet);
            //        elements.Add(element);
            //    }
            //}

            return element;
        }

        public T Add(string name)
        {
            var element = elements.FirstOrDefault(n => n.Name == name);
            if (element == null)
            {
                element = CreateInstance();
                element.Id = LibraryItem.GetNextId();
                element.Name = name;

                elements.Add(element);
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
        }
    }
}
