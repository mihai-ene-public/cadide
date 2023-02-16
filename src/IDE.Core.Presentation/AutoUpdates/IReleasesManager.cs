using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Presentation.AutoUpdates;
public interface IReleasesManager
{
    Task<ReleaseInfo> GetLatestRelease(int majorVersion, bool includePrerelease);
    Task<string> DownloadRelease(string downloadUrl, string assetFileName);
}
