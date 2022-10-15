using System.Windows.Input;
using IDE.Core.Interfaces;
using IDE.Core.Types.Input;

namespace IDE.Core.Commands
{
    public class NullCommandFactory : ICommandFactory
    {
        public ICommand CreateCommand(Action<object> execute)
        {
            throw new NotImplementedException();
        }

        public ICommand CreateCommand(Action<object> action, Predicate<object> canExecute)
        {
            throw new NotImplementedException();
        }

        public ICommand CreateUICommand(string text, string name, Type ownerType)
        {
            throw new NotImplementedException();
        }

        public ICommand CreateUICommand(string text, string name, Type ownerType, XKey key, XModifierKeys modifierKeys)
        {
            throw new NotImplementedException();
        }

        public ICommand CreateUICommand(string text, string name, Type ownerType, XKey key)
        {
            throw new NotImplementedException();
        }

        public ICommand CreateUICommand()
        {
            throw new NotImplementedException();
        }
    }
}
