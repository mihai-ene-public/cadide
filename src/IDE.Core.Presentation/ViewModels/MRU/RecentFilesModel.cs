using IDE.Core.Interfaces;
using IDE.Core.Settings;
using IDE.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IDE.Core.MRU
{
    public class RecentFilesModel : BaseViewModel, IRecentFilesViewModel
    {

        const int MaxMruEntryCount = 50;

        public RecentFilesModel(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        ISettingsManager _settingsManager;

        IList<MruItemViewModel> mruList = new ObservableCollection<MruItemViewModel>();

        public IList<MruItemViewModel> MruList
        {
            get
            {
                if (mruList.Count == 0)
                {
                    var profile = (Profile)_settingsManager.SessionData;
                    mruList.AddRange(profile.MruList.Select(m => new MruItemViewModel
                    {
                        IsPinned = m.IsPinned,
                        PathFileName = m.FilePath
                    }));
                }

                return mruList;
            }
            set
            {
                if (mruList != value)
                {
                    mruList = value;

                    OnPropertyChanged(nameof(MruList));
                }
            }
        }

        public void AddNewEntryIntoMRU(string filePath)
        {
            if (!ContainsEntry(filePath))
            {
                var e = new MruItemViewModel() { IsPinned = false, PathFileName = filePath };

                AddMRUEntry(e);

                OnPropertyChanged(nameof(MruList));
            }
        }

        public void AddMRUEntry(string newEntry)
        {
            if (newEntry == null || newEntry == string.Empty)
                return;

            AddMRUEntry(new MruItemViewModel() { IsPinned = false, PathFileName = newEntry });
        }

        public void AddMRUEntry(MruItemViewModel newEntry)
        {
            if (newEntry == null)
                return;

            // Remove all entries that point to the path we are about to insert
            var e = mruList.SingleOrDefault(item => newEntry.PathFileName == item.PathFileName);

            if (e != null)
            {
                // Do not change an entry that has already been pinned -> its pinned in place :)
                if (e.IsPinned)
                    return;

                mruList.Remove(e);
            }

            // Remove last entry if list has grown too long
            if (MaxMruEntryCount <= mruList.Count)
                mruList.RemoveAt(mruList.Count - 1);

            // Add model entry in ViewModel collection (First pinned entry or first unpinned entry)
            if (newEntry.IsPinned == true)
                mruList.Insert(0, new MruItemViewModel(newEntry));
            else
            {
                mruList.Insert(CountPinnedEntries(), new MruItemViewModel(newEntry));
            }
        }

        public bool ContainsEntry(string filePath)
        {
            return mruList.Any(mru => mru.PathFileName == filePath);
        }

        public void PinUnpinEntry(bool bPinOrUnPinMruEntry, string pathFilename)
        {
            var pinnedMruEntryCount = CountPinnedEntries();

            // pin an MRU entry into the next available pinned mode spot
            if (bPinOrUnPinMruEntry)
            {
                var e = mruList.Single(mru => mru.IsPinned == false && mru.PathFileName == pathFilename);

                mruList.Remove(e);

                e.IsPinned = true;

                mruList.Insert(pinnedMruEntryCount, e);

                pinnedMruEntryCount += 1;
            }
            else
            {
                // unpin an MRU entry into the next available unpinned spot
                var e = mruList.Single(mru => mru.IsPinned == true && mru.PathFileName == pathFilename);

                mruList.Remove(e);

                e.IsPinned = false;
                pinnedMruEntryCount -= 1;

                mruList.Insert(pinnedMruEntryCount, e);
            }
        }

       

        public bool RemoveEntry(string fileName)
        {
            var e = mruList.Single(mru => mru.PathFileName == fileName);
            mruList.Remove(e);

            return true;
        }

        private int CountPinnedEntries()
        {
            return mruList.Count(mru => mru.IsPinned);
        }

    }
}
