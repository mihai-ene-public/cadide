namespace IDE.Core
{
    using Commands;
    using IDE.Core.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Utilities;
    using System.Threading.Tasks;

    /// <summary>
    /// ViewModel base class to support dialog based views
    /// (or views in general that support OK/Cancel functions)
    /// </summary>
    public class DialogViewModel : BaseViewModel, IDialogViewModel
    {
        #region fields

        bool? dialogCloseResult;

        bool shutDownInProgress;
        bool isReadyToClose;

        ICommand cancelCommand;
        ICommand oKCommand;

        #endregion fields

        #region constructor
        /// <summary>
        /// constructor
        /// </summary>
        public DialogViewModel()
        {
            InitializeDialogState();
        }

        #endregion constructor

        #region delegates

        /// <summary>
        /// This type of method delegation is used when user input is evaluated.
        /// 
        /// The user input is available in the class that instantiates the <seealso cref="DialogViewModel"/> class.
        /// Therefore, the <seealso cref="DialogViewModel"/> class calls a delegate method to retrieve whether input
        /// is OK or not plus a list messages describing problem details.
        /// </summary>
        public delegate bool EvaluateInput(out List<Msg> outMsg);

        #endregion delegates

        #region event
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;
        #endregion event

        #region properties
        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        public bool? WindowCloseResult
        {
            get
            {
                return dialogCloseResult;
            }

            protected set
            {
                if (dialogCloseResult != value)
                {
                    dialogCloseResult = value;
                    OnPropertyChanged(nameof(WindowCloseResult));
                }
            }
        }

        /// <summary>
        /// Get/set proprety to determine whether application is ready to close
        /// (the setter is public here to bind it to a checkbox - in a normal
        /// application that setter would more likely be private and be set via
        /// a corresponding method call that manages/overrides the properties' value).
        /// </summary>
        public bool IsReadyToClose
        {
            get
            {
                return isReadyToClose;
            }

            set
            {
                if (isReadyToClose != value)
                {
                    isReadyToClose = value;
                    OnPropertyChanged(nameof(IsReadyToClose));
                }
            }
        }

        /// <summary>
        /// This property can be used to delegate the test of user input to the class that instantiates this class.
        /// User input is then, based on the external method, evaluated whenver a user executes the Cancel or OK command.
        /// 
        /// The user input is available in the class that instantiates the <seealso cref="DialogViewModel"/> class.
        /// Therefore, the <seealso cref="DialogViewModel"/> class calls a delegate method to retrieve whether input
        /// is OK or not plus a list messages describing problem details.
        /// </summary>
        public EvaluateInput EvaluateInputData
        {
            get;
            set;
        }

        /// <summary>
        /// Execute the cancel command (occurs typically when a user clicks cancel in the dialog)
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                    cancelCommand = CreateCommand(p =>
                    {
                        OnRequestClose(false);
                    });

                return cancelCommand;
            }
        }

        /// <summary>
        /// Execute the OK command (occurs typically when a user clicks OK in the dialog)
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (oKCommand == null)
                    oKCommand = CreateCommand(p =>
                    {
                        // Check user input and perform exit if data input is OK
                        PerformInputDataEvaluation();

                        if (IsReadyToClose == true)
                        {
                            OnRequestClose(true);
                        }
                    });

                return oKCommand;
            }
        }

        #endregion properties

        #region methods
        /// <summary>
        /// Reset the viewmodel such that opening a view (dialog) is realized with known states.
        /// </summary>
        public void InitializeDialogState()
        {

            EvaluateInputData = null;

            isReadyToClose = true;
            shutDownInProgress = false;
            dialogCloseResult = null;

            RequestClose = null;

        }

        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        public void OnRequestClose(bool setWindowCloseResult)
        {
            try
            {
                if (shutDownInProgress == false)
                {
                    WindowCloseResult = setWindowCloseResult;

                    shutDownInProgress = true;

                    if (RequestClose != null)
                        RequestClose(this, EventArgs.Empty);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Exception occurred in OnRequestClose\n{0}", exp.ToString());
                shutDownInProgress = false;
            }
        }


        /// <summary>
        /// Determine whether Dialog is ready to close down or
        /// whether close down should be cancelled - and cancel it.
        /// </summary>
        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WindowCloseResult == null) // Process window close button as Cancel (IsReadyToClose is not evaluated)
                return;

            if (WindowCloseResult == false)
            {
                e.Cancel = false;
                return;
            }

            e.Cancel = !IsReadyToClose; // Cancel close down request if ViewModel is not ready, yet
        }

        /// <summary>
        /// Call the external method delegation (if any) to verify whether user input is valid or not.
        /// </summary>
        private void PerformInputDataEvaluation()
        {
            if (this.EvaluateInputData != null)
            {
                List<Msg> msgs;
                var bResult = EvaluateInputData(out msgs);
                
                IsReadyToClose = bResult;
            }
        }

        protected virtual void LoadData() { }

        public bool? ShowDialog()
        {
            var mapper = ServiceProvider.Resolve<IDialogModelToWindowMapper>();
            if (mapper == null)
                throw new Exception($"Mapper service {nameof(IDialogModelToWindowMapper)} was not defined");

            var window = mapper.GetWindow(this);
            if (window == null)
                throw new Exception($"There is no window mapped for {this}");

            //owner?
            window.DataContext = this;

            Task.Run(() => LoadData());

            return window.ShowDialog();
        }
        #endregion methods
    }
}
