using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces;

public interface ISheetDesignerItem : INotifyPropertyChanged
{
    IList<ISelectableItem> Items { get; }
    string Name { get; }
}
