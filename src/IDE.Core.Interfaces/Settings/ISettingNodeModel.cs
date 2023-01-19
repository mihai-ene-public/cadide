using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface ISettingModel
    {
        string Name { get; }

        ISettingData ToData();

        void LoadFromData(ISettingData settingData);
    }
}
