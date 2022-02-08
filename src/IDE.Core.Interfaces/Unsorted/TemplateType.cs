using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public enum TemplateType
    {
        //Project templates
        /// <summary>
        /// When creating a new solution
        /// </summary>
        Solution,

        /// <summary>
        /// when we create a Project to the current solution
        /// </summary>
        Project,
        SampleProject,

        //item templates
        Symbol,
        Footprint,
        Model,
        Component,
        Schematic,
        Board,
        Misc
    }
}
