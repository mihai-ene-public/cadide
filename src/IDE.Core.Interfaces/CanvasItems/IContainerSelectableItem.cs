using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    /// <summary>
    /// an object that can have other items as children: Footprint, Symbol
    /// <para>used for select an item inside this container</para>
    /// </summary>
    public interface IContainerSelectableItem
    {
        double X { get; set; }
        double Y { get; set; }
        double Rot { get; set; }

        double ScaleX { get; set; }
        double ScaleY { get; set; }

        bool IsMirrored();
        //bool IsMirroredX();
        //bool IsMirroredY();

        IList<ISelectableItem> Items { get; }
    }

}
