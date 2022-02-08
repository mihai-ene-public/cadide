using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IPathNode
    {
        int X { get; }

        int Y { get; }

        bool IsWalkable(object inContext);

        bool CanWalkTo(IPathNode node);

        void BuildArcs(IPathNode[] neighbors);
    }
}
