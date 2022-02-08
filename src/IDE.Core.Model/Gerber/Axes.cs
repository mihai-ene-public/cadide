
using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Gerber
{
    /// <summary>
    /// Specifies either the x-axis or the y-axis.
    /// This enum is used by the GerberFile.AxisSelect method.
    /// </summary>
    public enum Axes
    {
        /// <summary>
        /// The x-axis.
        /// </summary>
        X,
        /// <summary>
        /// The y-axis.
        /// </summary>
        Y
    }
}
