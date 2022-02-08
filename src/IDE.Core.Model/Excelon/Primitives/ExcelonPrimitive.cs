using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Excelon
{
    public class ExcelonPrimitive
    {


        protected List<NCTool> cachedTools = new List<NCTool>();

        //for any primitive we should have a single tool
        public NCTool Tool { get; private set; }

        public List<NCTool> GetTools()
        {
            if (cachedTools.Count == 0)
                CreateTools();

            if (cachedTools.Count > 0)
                Tool = cachedTools[0];

            return cachedTools;
        }

        protected virtual void CreateTools()
        {
        }

        public virtual void WriteExcelon(NCDrillFileWriter excelonWriter) { }
    }
}
