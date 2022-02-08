using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public enum ProjectOutputType
    {
        /// <summary>
        /// output will be a compiled library in the form of xml: LibraryDocument class
        /// </summary>
        Library,

        /// <summary>
        /// output will be a list of gerber files (includes drill files, appertures, tool lists, etc)
        /// <para>There is a set of files for every board</para>
        /// </summary>
        Board
    }
}
