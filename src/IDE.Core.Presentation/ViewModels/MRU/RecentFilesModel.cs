using IDE.Core.Interfaces;
using IDE.Core.Settings;
using IDE.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

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

        ICommand togglePinnedForEntryCommand;
        public ICommand TogglePinnedForEntryCommand
        {
            get
            {
                if (togglePinnedForEntryCommand == null)
                    togglePinnedForEntryCommand = CreateCommand(
                    (p) =>
                    {
                        var mru = p as MruItemViewModel;
                        TogglePinnedForEntry(mru.PathFileName);
                    }
                    );

                return togglePinnedForEntryCommand;
            }
        }

        ICommand removeEntryCommand;
        public ICommand RemoveEntryCommand
        {
            get
            {
                if (removeEntryCommand == null)
                    removeEntryCommand = CreateCommand(
                    (p) =>
                    {
                        var mru = p as MruItemViewModel;
                        RemoveEntry(mru.PathFileName);
                    }
                    );

                return removeEntryCommand;
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

        public void TogglePinnedForEntry(string pathFilename)
        {
            var pinnedEntryCount = CountPinnedEntries();
            var entry = mruList.FirstOrDefault(mru => mru.PathFileName == pathFilename);
            if (entry == null)
                return;

            entry.IsPinned = !entry.IsPinned;
            mruList.Remove(entry);
            var insertPosition = pinnedEntryCount;

            if (!entry.IsPinned)
            {
                insertPosition--;
            }
            mruList.Insert(insertPosition, entry);

            //// pin an MRU entry into the next available pinned mode spot
            //if (bPinOrUnPinMruEntry)
            //{
            //    var e = mruList.Single(mru => mru.IsPinned == false && mru.PathFileName == pathFilename);

            //    mruList.Remove(e);

            //    e.IsPinned = true;

            //    mruList.Insert(pinnedEntryCount, e);

            //    pinnedEntryCount += 1;
            //}
            //else
            //{
            //    // unpin an MRU entry into the next available unpinned spot
            //    var e = mruList.Single(mru => mru.IsPinned == true && mru.PathFileName == pathFilename);

            //    mruList.Remove(e);

            //    e.IsPinned = false;
            //    pinnedEntryCount -= 1;

            //    mruList.Insert(pinnedEntryCount, e);
            //}
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
