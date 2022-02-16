using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Presentation.Infrastructure
{
    public interface IServiceProviderHelper
    {
        IEnumerable<T> GetServices<T>();
    }
}
