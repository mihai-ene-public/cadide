using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Data.Packages;
public class PackageMetadata
{
    //Id should be something meaningfull and unique
    //ex: "System.Libraries", "STM32.Microcontrollers"
    public string Id { get; set; }

    //having a title could make users have Id as a guid and a title the same as an existing library
    //and it would make more difficult to differentiate between projects
    //Therefore no title
    //public string Title { get; set; }
    public string Version { get; set; }

    //could be a list of usernames or Company
    public string Authors { get; set; }

    //license type: CC, MIT, etc
    public string License { get; set; }
    public string LicenseUrl { get; set; }
    public string Icon { get; set; }
    public string ProjectUrl { get; set; }
    public string Description { get; set; }
    public string Copyright { get; set; }
    public string Tags { get; set; }
    public PackageMetadataRepository Repository { get; set; }
}
