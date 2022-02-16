namespace IDE.Documents.Views
{
    using Core.ViewModels;
    using IDE.Core;
    using IDE.Core.Interfaces;
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Implementation is based on Output Tool Window from Gemini project:
    /// https://github.com/tgjones/gemini
    /// </summary>
    //[Export(typeof(IOutput))]
    public class OutputViewModel : ToolViewModel, IOutputToolWindow
    {
        #region fields
      //  private readonly OutputWriter outputWriter;
        private readonly StringBuilder textModel;

        public const string ToolContentId = "<OutputToolWindow>";
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public OutputViewModel()
         : base("Output")
        {
            //outputWriter = new OutputWriter(this);

            textModel = new StringBuilder();

            ContentId = OutputViewModel.ToolContentId;
        }
        #endregion constructors

        #region properties
      

        ///// <summary>
        ///// Implements the <seealso cref="IOutput"/> interface.
        ///// </summary>
        //public TextWriter Writer { get { return outputWriter; } }

        public string Text
        {
            get
            {
                return textModel.ToString();
            }
            set { }//seems silly but it is needed for SimpleTextEditor binding
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Bottom; }
        }
        #endregion properties

        #region methods

        public void Clear()
        {
            textModel.Clear();
            OnTextChanged();
        }

        private void OnTextChanged()
        {
            OnPropertyChanged(nameof(Text));
        }

        public void AppendLine(string text)
        {
            textModel.AppendLine(text);
            OnTextChanged();
        }

        public void Append(string text)
        {
            textModel.Append(text);
            OnTextChanged();
        }

        public void RegisterDocumentType(IDocumentTypeManager docTypeManager)
        {

        }

        #endregion methods
    }
}