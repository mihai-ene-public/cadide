using IDE.Core.Controls;
using System;
using System.Windows.Input;

namespace IDE.Core.Designers
{
    public class EditBoxModel : BaseViewModel, IEditBox
    {
        /// <summary>
        /// Expose an event that is triggered when the viewmodel requests its view to
        /// start the editing mode for rename this item.
        /// </summary>
        public event RequestEditEventHandler RequestEdit;

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }


        ICommand renameCommand;

        public ICommand RenameCommand
        {
            get
            {

                if (renameCommand == null)
                    renameCommand = CreateCommand(it =>
                    {
                        var tuple = it as Tuple<string, object>;

                        if (tuple != null)
                        {
                            var newName = tuple.Item1;

                            if (string.IsNullOrEmpty(newName))
                                return;

                            Name = newName;
                        }
                    });

                return renameCommand;
            }
        }

        ICommand beginRenameCommand;
        public ICommand BeginRenameCommand
        {
            get
            {
                if (beginRenameCommand == null)
                {
                    beginRenameCommand = CreateCommand(it =>
                    {
                        RequestEditMode(RequestEditEvent.StartEditMode);
                    }
                    );
                }

                return beginRenameCommand;
            }
        }

        public void RequestEditMode(RequestEditEvent request)
        {
            if (RequestEdit != null)
                RequestEdit(this, new RequestEdit(request));
        }
    }
}
