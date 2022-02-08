/*See 'NOTICE.txt' for license */

//http://wpfundoredo.codeplex.com

namespace IDE.Core.Interfaces
{
    public interface ISavedStateManager
    {
        void Backup();
        void Undo();

        void Redo();
    }
}
