using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace STPConverter.Implementation.Entity
{
    public interface IConvertable
    {
        IList<Vector3D> Points { get; }
        IList<int> Indices { get; }
    }
}
