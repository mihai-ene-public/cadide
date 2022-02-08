namespace IDE.Core
{
    using IDE.Core.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Windows.Input;

    public class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        public BaseViewModel()
        {
            PropertyChanged += PropertyChangedHandleInternal;

        }

        ICommandFactory commandFactory
        {
            get
            {
                if (_commandFactory == null)
                    _commandFactory = ServiceProvider.Resolve<ICommandFactory>();

                return _commandFactory;
            }
        }
        ICommandFactory _commandFactory;

        public event PropertyChangedEventHandler PropertyChanged;

        protected ICommand CreateCommand(Action<object> execute)
        {
            return commandFactory.CreateCommand(execute);
        }
        protected ICommand CreateCommand(Action<object> action, Predicate<object> canExecute)
        {
            return commandFactory.CreateCommand(action, canExecute);
        }

      
        public virtual void OnPropertyChanged(string propertyName)
        {
            try
            {
                var h = PropertyChanged;

                h?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch { }
        }


        protected virtual void PropertyChangedHandleInternal(object sender, PropertyChangedEventArgs e)
        {

        }

        public virtual void Dispose()
        {
            PropertyChanged -= PropertyChangedHandleInternal;
        }
    }

}
