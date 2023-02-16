using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Presentation.AutoUpdates;

namespace IDE.Core.Presentation.Tests.Integration;
public class GithubReleasesManagerIntegrationTests
{
    [Fact]
    public async Task GetLatestRelease()
    {
        var manager = new GithubReleasesManager();

        var release = await manager.GetLatestRelease(majorVersion: 0, includePrerelease: true);

        Assert.NotNull(release);
        Assert.NotNull(release.ReleaseAssetDownloadUrl);
    }
}
