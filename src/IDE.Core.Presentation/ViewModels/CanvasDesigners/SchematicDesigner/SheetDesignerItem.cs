using IDE.Core.Interfaces;
using IDE.Core.UserNotification;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class SheetDesignerItem : EditBoxModel, ISheetDesignerItem
    {

        //plain: primitives
        //symbols
        //busses
        //nets

        private IList<ISelectableItem> items = new SpatialItemsSource();

        //it will need some items that are not selectable
        public IList<ISelectableItem> Items
        {
            get { return items; }
        }
    }
}
