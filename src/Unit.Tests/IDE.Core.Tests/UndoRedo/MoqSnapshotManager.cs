using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.UndoRedoFramework;
using IDE.Core.Interfaces;

namespace IDE.Core.Tests
{
    public class MoqSnapshotManager : ISnapshotManager
    {

        public string Data { get; set; }

        public ISavedState CreateSnapshot()
        {
            return new StringSavedState
            {
                Data = Data
            };
        }

        public void RestoreFromSnapshot(ISavedState state)
        {
            var s = state as StringSavedState;
            Data = s.Data;
        }
    }
}
