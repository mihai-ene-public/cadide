using System.Collections.ObjectModel;

namespace IDE.Documents.Views
{
    public class LibraryDisplay
    {
        public LibraryDisplay()
        {
            Items = new ObservableCollection<ItemDisplay>();
        }
        public string Name { get; set; }

        /// <summary>
        /// items in the library
        /// </summary>
        public ObservableCollection<ItemDisplay> Items { get; set; }
    }
}
