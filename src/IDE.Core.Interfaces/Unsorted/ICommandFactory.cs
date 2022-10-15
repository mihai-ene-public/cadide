using IDE.Core.Types.Input;
using System;
using System.Windows.Input;

namespace IDE.Core.Interfaces
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(Action<object> execute);
        ICommand CreateCommand(Action<object> action, Predicate<object> canExecute);

        ICommand CreateUICommand(string text, string name, Type ownerType);
        ICommand CreateUICommand(string text, string name, Type ownerType, XKey key, XModifierKeys modifierKeys);
        ICommand CreateUICommand(string text, string name, Type ownerType, XKey key);
        ICommand CreateUICommand();
    }
}
