using IDE.Core.Interfaces;
using IDE.Core.Types.Input;
using System;
using System.Windows.Input;

namespace IDE.Core.Commands
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand CreateCommand(Action<object> execute)
        {
            return new RelayCommand(execute);
        }

        public ICommand CreateCommand(Action<object> action, Predicate<object> canExecute)
        {
            return new RelayCommand(action, canExecute);
        }

        public ICommand CreateUICommand(string text, string name, Type ownerType)
        {
            return new RoutedUICommand(text, name, ownerType);
        }

        public ICommand CreateUICommand(string text, string name, Type ownerType, XKey key, XModifierKeys modifierKeys)
        {
            var inputs = new InputGestureCollection();

            var keyGesture = new KeyGesture((Key)key, (ModifierKeys)modifierKeys);

            inputs.Add(keyGesture);

            return new RoutedUICommand(text, name, ownerType, inputs);
        }

        public ICommand CreateUICommand(string text, string name, Type ownerType, XKey key)
        {
            var inputs = new InputGestureCollection();

            var keyGesture = new KeyGesture((Key)key);

            inputs.Add(keyGesture);

            return new RoutedUICommand(text, name, ownerType, inputs);
        }

        public ICommand CreateUICommand()
        {
            return new RoutedUICommand();
        }
    }
}
