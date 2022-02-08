using IDE.Core.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IGroupFolderItem
    {
        List<ProjectBaseFileRef> Children { get; set; }
    }
}
