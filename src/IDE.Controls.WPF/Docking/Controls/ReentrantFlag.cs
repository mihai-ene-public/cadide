using System;

namespace IDE.Controls.WPF.Docking.Controls;

internal class ReentrantFlag
{
    #region Members

    private bool _flag = false;

    #endregion

    #region Properties

    public bool CanEnter
    {
        get
        {
            return !_flag;
        }
    }

    #endregion

    #region Public Methods

    public _ReentrantFlagHandler Enter()
    {
        if (_flag)
            throw new InvalidOperationException();
        return new _ReentrantFlagHandler(this);
    }

    #endregion

    #region Internal Classes

    public class _ReentrantFlagHandler : IDisposable
    {
        private ReentrantFlag _owner;
        public _ReentrantFlagHandler(ReentrantFlag owner)
        {
            _owner = owner;
            _owner._flag = true;
        }

        public void Dispose()
        {
            _owner._flag = false;
        }
    }

    #endregion
}
