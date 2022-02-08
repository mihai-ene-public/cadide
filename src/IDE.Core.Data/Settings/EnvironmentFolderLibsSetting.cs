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

        //public IReadOnlyList<string> LibraryFolders
        //{
        //    get
        //    {
        //        var list = new List<string>();
        //        list.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Modern PCB Studio\Libraries");
        //        list.Add("Libraries");//start up folder of IDE.exe
        //        list.AddRange(Folders.Distinct());

        //        return list.AsReadOnly();
        //    }
        //}

    }
}
