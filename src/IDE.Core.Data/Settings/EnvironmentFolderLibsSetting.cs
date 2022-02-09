using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Settings
{
    public class EnvironmentFolderLibsSettingData : BasicSetting
    {
        //todo: a list of folder paths; could be a string: one per line

        public List<string> Folders { get; set; } = new List<string>();


    }
}
