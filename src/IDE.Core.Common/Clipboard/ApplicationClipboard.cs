using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core
{
    public class ApplicationClipboard
    {

        /// <summary>
        /// a cached list of original selected items that were references from canvasModel.SelectedItems
        /// </summary>
        public static List<object> Items { get; set; } = new List<object>();
    }
}
