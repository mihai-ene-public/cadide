using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public enum LayerType
    {
        Unknown = -1,
        Signal = 0,//stackup
        Plane = 100,//stackup
        Mechanical = 200,
        PasteMask = 300,
        SolderMask = 400,//stackup
        SilkScreen = 500,//stackup
        Dielectric = 600,//stackup

        MultiLayer = 700,//Id = LayerConstants.MechanicalMillingId,
        //DrillGuide,
        Keepout = 800,
        //DrillDrawing
        Generic = 900,
        BoardOutline = 1000
    }
}
