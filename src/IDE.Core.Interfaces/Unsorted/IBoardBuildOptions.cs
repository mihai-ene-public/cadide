using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IBoardBuildOptions
    {
        #region NC Drill files
        OutputUnits NCDrillUnits { get; }
        int NCDrillFormatBeforeDecimal { get; }
        int NCDrillFormatAfterDecimal { get; }
        #endregion


        #region Gerber

        OutputUnits GerberUnits { get; }
        int GerberFormatBeforeDecimal { get; }
        int GerberFormatAfterDecimal { get; }

        bool GerberPlotBoardOutlineOnAllLayers { get; }

        bool GerberCreateZipFile { get; }

        IList<ILayerDesignerItem> GerberPlotLayers { get; }
        #endregion

    }

    public enum OutputUnits
    {
        mm,
        inch
    }

    public enum GerberVersion
    {
        X1,
        X2
    }
}
