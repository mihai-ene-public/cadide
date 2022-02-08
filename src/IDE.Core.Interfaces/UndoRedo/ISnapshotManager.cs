/*See 'NOTICE.txt' for license */
using IDE.Core.Interfaces;

//http://wpfundoredo.codeplex.com

namespace IDE.Core.Interfaces
{
    public interface ISnapshotManager
    {
        ISavedState CreateSnapshot();

        void RestoreFromSnapshot(ISavedState state);
    }
}
