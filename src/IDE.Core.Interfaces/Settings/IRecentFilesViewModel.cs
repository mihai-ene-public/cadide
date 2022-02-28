using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IRecentFilesViewModel
    {
        bool ContainsEntry(string filePathName);

        void AddMRUEntry(string newEntry);

        void AddNewEntryIntoMRU(string filePath);

        bool RemoveEntry(string fileName);

        void TogglePinnedForEntry(string pathFilename);
    }
}
