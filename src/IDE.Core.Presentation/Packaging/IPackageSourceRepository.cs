using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Presentation.Packaging;
public interface IPackageSourceRepository
{
    IList<PackageSource> GetPackageSources();
}
