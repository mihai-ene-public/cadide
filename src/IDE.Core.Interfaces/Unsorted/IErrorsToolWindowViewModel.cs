using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Interfaces
{
    public interface IErrorsToolWindow : IToolWindow, IRegisterable
    {

        ObservableCollection<IErrorMessage> Errors { get; }


        void Clear();
        void AddErrorMessage(IErrorMessage message);

        void AddError(string message, string fileName, string projectName);
    }
}
