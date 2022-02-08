using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IPosition : INotifyPropertyChanged
    {
        double X { get; set; }

        double Y { get; set; }

        double Rotation { get; set; }
    }
}
