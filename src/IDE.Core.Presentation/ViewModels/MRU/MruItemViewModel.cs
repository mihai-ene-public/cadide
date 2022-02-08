using System.IO;

namespace IDE.Core.MRU
{
    public class MruItemViewModel:BaseViewModel
    {

        public MruItemViewModel()
        {

        }

        public MruItemViewModel(MruItemViewModel copySource)
        {
            PathFileName = copySource.PathFileName;
            IsPinned = copySource.IsPinned;
        }

        string pathFileName;

        public string PathFileName
        {
            get
            {
                return pathFileName;
            }

            set
            {
                if (pathFileName != value)
                {
                    pathFileName = value;
                    OnPropertyChanged(nameof(PathFileName));
                    OnPropertyChanged(nameof(DisplayPathFileName));
                }
            }
        }

        public string FileName
        {
            get
            {
                if (PathFileName != null)
                    return Path.GetFileNameWithoutExtension(PathFileName);

                return null;
            }
        }

        public string DisplayPathFileName
        {
            get
            {
                if (PathFileName == null)
                    return string.Empty;

                int n = 32;
                return (PathFileName.Length > n ? PathFileName.Substring(0, 3) +
                                                "... " + PathFileName.Substring(PathFileName.Length - n)
                                                : PathFileName);
            }
        }

        bool isPinned;

        public bool IsPinned
        {
            get
            {
                return isPinned;
            }

            set
            {
                if (isPinned != value)
                {
                    isPinned = value;
                    OnPropertyChanged(nameof(IsPinned));
                }
            }
        }
    }
}
