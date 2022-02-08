

using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Gerber
{
    /// <summary>
    /// Represents the layer polarity.  That is, whether the filled polygons represent
    /// the clear or dark part of the photomask.
    /// </summary>
    public enum Polarity
    {
        /// <summary>
        /// The layer represents clear geometry.
        /// </summary>
        Clear,
        /// <summary>
        /// The layer represents dark geometry.
        /// </summary>
        Dark
    }
}
