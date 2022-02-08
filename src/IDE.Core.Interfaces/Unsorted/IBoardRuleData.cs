using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IBoardRuleData
    {
        long Id { get; set; }

        string Name { get; set; }

        string Comment { get; set; }

        bool IsEnabled { get; set; }

        int Priority { get; set; }
    }
}
