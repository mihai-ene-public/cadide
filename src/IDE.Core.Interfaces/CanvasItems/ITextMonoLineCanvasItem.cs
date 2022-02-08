using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDE.Core.Interfaces
{
    public interface ITextMonoLineCanvasItem : ITextBaseCanvasItem
    {
        IList<ILetterItem> LetterItems { get; }
    }

    public interface ILetterItem
    {
        double FontSize { get; set; }

        IList<ISelectableItem> Items { get; set; }
    }
}
