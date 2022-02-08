using IDE.Core.UserNotification;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Designers
{
    public class SheetDesignerItem : EditBoxModel
    {

        //plain: primitives
        //symbols
        //busses
        //nets

        SpatialItemsSource items = new SpatialItemsSource();

        //it will need some items that are not selectable
        public SpatialItemsSource Items
        {
            get { return items; }
        }
    }
}
