using IDE.Controls;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class SpatialItemsSource : ObservableCollection<ISelectableItem>
                                    , ISpatialItemsSource
    {
        public event EventHandler ExtentChanged;
        public event EventHandler QueryInvalidated;

        public SpatialItemsSource()
        {

        }

        public SpatialItemsSource(IEnumerable<ISelectableItem> collection)
        {
            this.AddRange(collection);
        }

        public XRect Extent
        {
            get
            {
                var sqrt = Math.Sqrt(Count);
                return new XRect(0, 0,
                    100 * (Math.Ceiling(sqrt)),
                    100 * (Math.Round(sqrt)));
            }
        }

        public IEnumerable<int> Query(XRect rectangle)
        {
            //all
            for (int i = 0; i < Count; i++)
                yield return i;
        }

        void OnExtendChanged()
        {
            ExtentChanged?.Invoke(this, EventArgs.Empty);
        }

        void OnQueryInvalidated()
        {
            QueryInvalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}
