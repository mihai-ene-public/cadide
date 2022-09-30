namespace IDE.Controls
{
    using IDE.Core;
    using IDE.Core.Controls;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    public class EditableLabel : ContentControl
    {
        #region dependency properties

        public static readonly DependencyProperty TextWrappingProperty =
                        DependencyProperty.Register("TextWrapping",
                                                    typeof(TextWrapping),
                                                    typeof(EditableLabel),
                                                    new FrameworkPropertyMetadata(TextWrapping.NoWrap)
                                                    );

        public static readonly DependencyProperty TextAlignmentProperty =
             DependencyProperty.Register("TextAlignment",
                                                    typeof(TextAlignment),
                                                    typeof(EditableLabel),
                                                    new FrameworkPropertyMetadata(TextAlignment.Center)
                                                    );

        public static readonly DependencyProperty TextDecorationsProperty =
             DependencyProperty.Register("TextDecorations",
                                                    typeof(TextDecorationCollection),
                                                    typeof(EditableLabel),
                                                    new FrameworkPropertyMetadata(null)
                                                    );


        public static readonly DependencyProperty ShrinkFontSizeWhenEditingByProperty =
           DependencyProperty.Register("ShrinkFontSizeWhenEditingBy",
                                                  typeof(double),
                                                  typeof(EditableLabel),
                                                  new FrameworkPropertyMetadata(0d)
                                                  );


        /// <summary>
        /// TextProperty DependencyProperty should be used to indicate
        /// the string that should be edit in the <seealso cref="EditableLabel"/> control.
        /// </summary>
        private static readonly DependencyProperty TextProperty =
                DependencyProperty.Register(
                        "Text",
                        typeof(string),
                        typeof(EditableLabel),
                        new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTextChangedCallback))
                        );


        /// <summary>
        /// Backing storage of DisplayText dependency property should be used to indicate
        /// the string that should displayed when <seealso cref="EditableLabel"/>
        /// control is not in edit mode.
        /// </summary>
        private static readonly DependencyProperty DisplayTextProperty =
                DependencyProperty.Register("DisplayText",
                                            typeof(string),
                                            typeof(EditableLabel),
                                            new PropertyMetadata(string.Empty));

        /// <summary>
        /// IsEditingProperty DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty =
                DependencyProperty.Register(
                        "IsEditing",
                        typeof(bool),
                        typeof(EditableLabel),
                        new FrameworkPropertyMetadata(false,
                        new PropertyChangedCallback(OnIsEditingChanged)));

        /// <summary>
        /// Implement dependency property to determine whether editing data is allowed or not
        /// (control never enters editing mode if IsReadOnly is set to true [default is false])
        /// </summary>
        private static readonly DependencyProperty mIsReadOnlyProperty =
                DependencyProperty.Register(
                        "IsReadOnly",
                        typeof(bool),
                        typeof(EditableLabel),
                        new FrameworkPropertyMetadata(false));


        #region InvalidCharacters dependency properties
        /// <summary>
        /// Backing store of dependency property
        /// </summary>
        public static readonly DependencyProperty InvalidInputCharactersProperty =
            DependencyProperty.Register("InvalidInputCharacters",
                                        typeof(string), typeof(EditableLabel), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of dependency property
        /// </summary>
        public static readonly DependencyProperty InvalidInputCharactersErrorMessageProperty =
            DependencyProperty.Register("InvalidInputCharactersErrorMessage",
                               typeof(string), typeof(EditableLabel), new PropertyMetadata(null));

        /// <summary>
        /// Backing store of dependency property
        /// </summary>
        public static readonly DependencyProperty InvalidInputCharactersErrorMessageTitleProperty =
            DependencyProperty.Register("InvalidInputCharactersErrorMessageTitle",
                               typeof(string), typeof(EditableLabel), new PropertyMetadata(null));
        #endregion InvalidCharacters dependency properties


        #endregion dependency properties

        public EditableLabel()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
            //HorizontalContentAlignment = HorizontalAlignment.Center;
            //Foreground = Brushes.Black;
            InitComponents();

            //DataContextChanged += OnDataContextChanged;

            //Unloaded += Oncontrol_Unloaded;

            Content = displaytextBlock;
        }


        void InitComponents()
        {
            displaytextBlock = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 10,
                Foreground = Foreground
            };

            editTextBox = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0),
                Margin = new Thickness(-6, -6, -6, -6),//todo: binding
                Background = new SolidColorBrush(Color.FromArgb(127, 0, 0, 0)),
                BorderThickness = new Thickness(0)
            };


            var displayBinding = new Binding("DisplayText")
            {
                Source = this,
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            displaytextBlock.SetBinding(TextBlock.TextProperty, displayBinding);

            var foregroundBinding = new Binding(nameof(Foreground))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            displaytextBlock.SetBinding(TextBlock.ForegroundProperty, foregroundBinding);

            var textBinding = new Binding(nameof(TextWrapping))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            displaytextBlock.SetBinding(TextBlock.TextWrappingProperty, textBinding);
            editTextBox.SetBinding(TextBox.TextWrappingProperty, textBinding);

            textBinding = new Binding(nameof(TextAlignment))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            displaytextBlock.SetBinding(TextBlock.TextAlignmentProperty, textBinding);
            editTextBox.SetBinding(TextBox.TextAlignmentProperty, textBinding);

            textBinding = new Binding(nameof(TextDecorations))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            displaytextBlock.SetBinding(TextBlock.TextDecorationsProperty, textBinding);
            editTextBox.SetBinding(TextBox.TextDecorationsProperty, textBinding);


            var editBinding = new Binding("Text")
            {
                Source = this,
                Mode = BindingMode.TwoWay,
            };
            editTextBox.SetBinding(TextBox.TextProperty, editBinding);

            editTextBox.PreviewTextInput += OnPreviewTextInput;
            editTextBox.KeyDown += OnTextBoxKeyDown;
            editTextBox.LostKeyboardFocus += OnTextBoxLostKeyboardFocus;
            editTextBox.PreviewMouseLeftButtonDown += EditTextBox_PreviewMouseLeftButtonDown;
            editTextBox.GotKeyboardFocus += EditTextBox_GotKeyboardFocus;
            editTextBox.LayoutUpdated += EditTextBox_LayoutUpdated;

            editTextBox.LostFocus += OnLostFocus;
        }

        private void EditTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (IsEditing && !editTextBox.IsFocused)
            {
                editTextBox.Focus();
            }
        }

        private void EditTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            editTextBox.SelectAll();
        }

        private void EditTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!editTextBox.IsKeyboardFocusWithin && ( e.OriginalSource == editTextBox || e.Source == editTextBox ))
            {
                e.Handled = true;
                editTextBox.Focus();
            }
        }

        private readonly object lockObject = new object();

        private TextBox editTextBox;

        TextBlock displaytextBlock;

        private ItemsControl mParentItemsControl;

        #region properties

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
        }

        public double ShrinkFontSizeWhenEditingBy
        {
            get { return (double)GetValue(ShrinkFontSizeWhenEditingByProperty); }
            set { SetValue(ShrinkFontSizeWhenEditingByProperty, value); }
        }

        /// <summary>
        /// Gets the text value for editing in the
        /// text portion of the EditableLabel.
        /// </summary>
        public string Text
        {
            private get
            {
                return (string)GetValue(EditableLabel.TextProperty);
            }
            set
            {
                SetValue(EditableLabel.TextProperty, value);
            }
        }

        /// <summary>
        /// Gets the text to display.
        /// 
        /// The DisplayText dependency property should be used to indicate
        /// the string that should displayed when <seealso cref="EditableLabel"/>
        /// control is not in edit mode.
        /// </summary>
        public string DisplayText
        {
            private get { return (string)GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        /// <summary>
        /// Implement dependency property to determine whether editing data is allowed or not
        /// (control never enters efiting mode if IsReadOnly is set to true [default is false])
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(mIsReadOnlyProperty); }
            set { SetValue(mIsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Gets the scrollviewer in which this control is embeded.
        /// </summary>
        internal ScrollViewer ParentScrollViewer { get; private set; }

        /// <summary>
        /// Gets Editing mode which is true if the EditableLabel control
        /// is in editing mode, otherwise false.
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return (bool)GetValue(EditableLabel.IsEditingProperty);
            }
            set
            {
                SetValue(EditableLabel.IsEditingProperty, value);
            }
        }

        public static readonly DependencyProperty IsEditableOnDoubleClickProperty =
           DependencyProperty.Register("IsEditableOnDoubleClick", typeof(bool), typeof(EditableLabel), new PropertyMetadata(true));
        public bool IsEditableOnDoubleClick
        {
            get { return (bool)GetValue(IsEditableOnDoubleClickProperty); }
            set { SetValue(IsEditableOnDoubleClickProperty, value); }
        }

        #region InvalidCharacters dependency properties


        /// <summary>
        /// Gets/sets the string dependency property that contains the characters
        /// that are considered to be invalid in the textbox input overlay element.
        /// </summary>
        public string InvalidInputCharacters
        {
            get { return (string)GetValue(InvalidInputCharactersProperty); }
            set { SetValue(InvalidInputCharactersProperty, value); }
        }

        /// <summary>
        /// Gets/sets the string dependency property that contains the error message
        /// that is shown when the user enters an invalid key.
        /// </summary>
        public string InvalidInputCharactersErrorMessage
        {
            get { return (string)GetValue(InvalidInputCharactersErrorMessageProperty); }
            set { SetValue(InvalidInputCharactersErrorMessageProperty, value); }
        }

        /// <summary>
        /// Gets/sets the string dependency property that contains
        /// the title of the error message that is shown when the user
        /// enters an invalid key. This title is similar to a window
        /// caption but it is not a window caption since the error message
        /// is shown in a custom pop-up element.
        /// </summary>
        public string InvalidInputCharactersErrorMessageTitle
        {
            get { return (string)GetValue(InvalidInputCharactersErrorMessageTitleProperty); }
            set { SetValue(InvalidInputCharactersErrorMessageTitleProperty, value); }
        }
        #endregion InvalidCharacters dependency properties


        #endregion properties

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (IsEditing)
                return;

            if (IsEditableOnDoubleClick)
                IsEditing = true;
        }


        #region textbox events
        /// <summary>
        /// Previews input from TextBox and cancels those characters (with pop-up error message)
        /// that do not appear to be valid (based on given array of invalid characters and error message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Nothing to process if this dependency property is not set
            if (string.IsNullOrEmpty(InvalidInputCharacters))
                return;

            if (e == null)
                return;

            lock (lockObject)
            {
                if (IsEditing)
                {
                    foreach (char item in InvalidInputCharacters.ToCharArray())
                    {
                        if (string.Compare(e.Text, item.ToString(), false) == 0)
                        {
                            e.Handled = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This handler method is called when the dependency property <seealso cref="EditableLabel.TextProperty"/>
        /// is changed in the data source (the ViewModel). The event is raised to tell the view to update its display.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnTextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (EditableLabel)d;

            if (vm.IsEditing)
            {
                vm.Text = (string)e.NewValue;
            }
        }

        /// <summary>
        /// When an EditableLabel is in editing mode, pressing the ENTER or F2
        /// keys switches the EditableLabel to normal mode.
        /// </summary>
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            lock (lockObject)
            {
                if (e.Key == Key.Escape)
                {
                    CancelEdit();
                    e.Handled = true;

                    return;
                }

                // Cancel editing mode (editing string is OK'ed)
                if (IsEditing)
                {
                    if (( e.Key == Key.Enter || e.Key == Key.F2 ) && e.KeyboardDevice.Modifiers == ModifierKeys.None)
                    {
                        AcceptEdit();
                        e.Handled = true;

                        return;
                    }
                    else if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                    {
                        //editTextBox.Text += Environment.NewLine;
                        var oldCaretIndex = editTextBox.CaretIndex;
                        editTextBox.Text = editTextBox.Text.Insert(editTextBox.CaretIndex, Environment.NewLine);
                        editTextBox.CaretIndex = oldCaretIndex + 1;
                    }
                }
            }
        }

        /// <summary>
        /// If an EditableLabel loses focus while it is in editing mode, 
        /// the EditableLabel mode switches to normal mode.
        /// </summary>
        private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CancelEdit();
        }

        /// <summary>
        /// Ends the editing mode if textbox loses the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            CancelEdit();
        }
        #endregion textbox events

        bool bCancelEdit = true;

        private void OnSwitchToNormalMode()
        {
            lock (lockObject)
            {
                string sNewName = string.Empty;

                if (editTextBox != null)
                    sNewName = editTextBox.Text;

                if (bCancelEdit == false)
                {
                    Text = sNewName;
                }
                else
                {
                    if (editTextBox != null)
                        editTextBox.Text = Text;
                }

                Content = displaytextBlock;

            }
        }

        void CancelEdit()
        {
            bCancelEdit = true;
            IsEditing = false;
            OnIsEditingChanged();
        }

        void AcceptEdit()
        {
            bCancelEdit = false;
            IsEditing = false;
            OnIsEditingChanged();
        }

        private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editLabel = (EditableLabel)d;
            editLabel.OnIsEditingChanged();
        }

        void OnIsEditingChanged()
        {
            if (IsReadOnly)
            {
                if (IsEditing)
                    IsEditing = false;

                return;
            }
            if (IsEditing)
            {
                OnSwitchToEditingMode();
            }
            else
            {
                OnSwitchToNormalMode();
            }
        }

        /// <summary>
        /// Displays the adorner textbox to let the user edit the text string.
        /// </summary>
        private void OnSwitchToEditingMode()
        {
            lock (lockObject)
            {
                HookItemsControlEvents();

                editTextBox.Width = Width + 8;
                //editTextBox.Width = displaytextBlock.ActualWidth;
                //editTextBox.Height = displaytextBlock.ActualHeight;
                editTextBox.FontSize = displaytextBlock.FontSize - ShrinkFontSizeWhenEditingBy;
                SetValue(Canvas.ZIndexProperty, 5000);

                Content = editTextBox;
            }
        }

        /// <summary>
        /// Walk the visual tree to find the ItemsControl and 
        /// hook into some of its events. This is done to make
        /// sure that editing is cancelled whenever
        /// 
        ///   1> The parent control is scrolling its content
        /// 1.1> The MouseWheel is used
        ///   2> A user clicks outside the adorner control
        ///   3> The parent control changes its size
        /// 
        /// </summary>
        private void HookItemsControlEvents()
        {

            mParentItemsControl = this.FindParent<ItemsControl>();
            Debug.Assert(mParentItemsControl != null, "DEBUG ISSUE: No FolderTreeView found.");

            if (mParentItemsControl != null)
            {
                // Handle events on parent control and determine whether to switch to Normal mode or stay in editing mode
                //mParentItemsControl.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(OnScrollViewerChanged));
                mParentItemsControl.AddHandler(ScrollViewer.MouseWheelEvent, new RoutedEventHandler((s, e) => CancelEdit()), true);

                mParentItemsControl.MouseDown += (s, e) => CancelEdit();
                //mParentItemsControl.SizeChanged += (s, e) => CancelEdit();

                // Restrict text box to visible area of scrollviewer
                ParentScrollViewer = mParentItemsControl.FindParent<ScrollViewer>();

                if (ParentScrollViewer == null)
                    ParentScrollViewer = mParentItemsControl.FindChild<ScrollViewer>();

                Debug.Assert(ParentScrollViewer != null, "DEBUG ISSUE: No ScrollViewer found.");

                if (ParentScrollViewer != null)
                    editTextBox.MaxWidth = ParentScrollViewer.ViewportWidth;
            }
        }

    }
}
