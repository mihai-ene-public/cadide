using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ISolutionExplorerNodeModel : INotifyPropertyChanged
    {
        string Name { get; set; }

        object Document { get; set; }

        bool IsExpanded { get; set; }

        string FileName { get; set; }

        IList<ISolutionExplorerNodeModel> Children { get; }

        ISolutionExplorerNodeModel ParentNode { get; set; }

        ISolutionProjectNodeModel ProjectNode { get; }

        string GetItemFullPath();

        string GetItemFolderFullPath();
        void Load(string p);
    }
}
