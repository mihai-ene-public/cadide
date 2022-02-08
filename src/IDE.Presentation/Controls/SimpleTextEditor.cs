using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace IDE.Controls
{
    public class SimpleTextEditor : TextEditor, INotifyPropertyChanged
    {

        /// <summary>
        /// Default constructor to set up event handlers.
        /// </summary>
        public SimpleTextEditor()
        {
            // Default options.
            FontSize = 12;
            //FontFamily = new FontFamily("Consolas");
            Options = new TextEditorOptions
            {
                IndentationSize = 3,
                ConvertTabsToSpaces = true
            };
        }

        #region Text.
        /// <summary>
        /// Dependancy property for the editor text property binding.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
             DependencyProperty.Register("Text",
                                        typeof(string),
                                        typeof(SimpleTextEditor),
                                        new FrameworkPropertyMetadata(default(string),
                                                                      FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                      TextPropertyChangedCallback)
                 );

        protected static void TextPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var target = (SimpleTextEditor)obj;

            if (target.Document != null)
            {
                var caretOffset = target.CaretOffset;
                var newValue = args.NewValue;

                if (newValue == null)
                {
                    newValue = "";
                }

                target.Document.Text = (string)newValue;
                target.CaretOffset = Math.Min(caretOffset, newValue.ToString().Length);
            }
        }
        public new string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                RaisePropertyChanged("Text");
            }
        }



        /// <summary>
        /// Override of OnTextChanged event.
        /// </summary>
        protected override void OnTextChanged(EventArgs e)
        {
            if (Document != null)
            {
                Text = Document.Text;
            }

            base.OnTextChanged(e);
        }


        #endregion // Text.



        #region Properties.

        #endregion // Properties.

        #region Raise Property Changed.
        /// <summary>
        /// Implement the INotifyPropertyChanged event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }
        #endregion // Raise Property Changed.
    }
}
