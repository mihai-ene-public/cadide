namespace IDE.Controls
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    public partial class ComboBoxWaterMarkTextInput : UserControl
    {
        #region fields

        #region ComboBox

        private static readonly DependencyProperty ItemsSourceProperty =
          ItemsSourceProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

        private static readonly DependencyProperty LabelContentProperty =
          DependencyProperty.Register("LabelContent", typeof(string), typeof(ComboBoxWaterMarkTextInput));

        private static readonly DependencyProperty DisplayMemberPathProperty =
          DisplayMemberPathProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

        private static readonly DependencyProperty SelectedValuePathProperty =
          SelectedValuePathProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

        private static readonly DependencyProperty SelectedItemProperty =
          SelectedItemProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

        private static readonly DependencyProperty SelectedValueProperty =
          SelectedValueProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

        private static readonly DependencyProperty SelectedIndexProperty =
          SelectedIndexProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

        #endregion ComboBox

        #region TextBox

        private static readonly DependencyProperty LabelTextBoxProperty =
          DependencyProperty.Register("LabelTextBox", typeof(string), typeof(ComboBoxWaterMarkTextInput));

        private static readonly DependencyProperty TextProperty =
          TextBox.TextProperty.AddOwner(typeof(ComboBoxWaterMarkTextInput));

        private static readonly DependencyProperty WatermarkProperty =
          DependencyProperty.Register("Watermark", typeof(string), typeof(ComboBoxWaterMarkTextInput));

        #endregion TextBox
        #endregion fields

        #region constructor
        public ComboBoxWaterMarkTextInput()
        {
            InitializeComponent();
        }
        #endregion constructor

        #region properties
        #region ComboBox
        /// <summary>
        /// Declare ItemsSource and Register as an Owner of ComboBox.ItemsSource
        /// the ComboBoxWaterMarkTextInput.xaml will bind the ComboBox.ItemsSource to this property
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Declare a ComboBox label dependency property
        /// </summary>
        public string LabelContent
        {
            // These proeprties can be bound to. The XAML for this control binds the Label's content to this.
            get { return (string)GetValue(LabelContentProperty); }
            set { SetValue(LabelContentProperty, value); }
        }

        /// <summary>
        /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        /// <summary>
        /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
        /// </summary>
        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        /// <summary>
        /// MSDN Reference: http://msdn.microsoft.com/en-us/library/system.windows.controls.combobox.aspx
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        #endregion ComboBox

        #region TextBox
        /// <summary>
        /// Declare a TextBox label dependency property
        /// </summary>
        public string LabelTextBox
        {
            // These proeprties can be bound to. The XAML for this control binds the Label's content to this.
            get { return (string)GetValue(LabelTextBoxProperty); }
            set { SetValue(LabelTextBoxProperty, value); }
        }

        /// <summary>
        /// Declare a TextBox Text dependency property
        /// </summary>
        public string Text
        {
            // These proeprties can be bound to. The XAML for this control binds the Label's content to this.
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Declare a TextBox Watermark label dependency property
        /// </summary>
        public string Watermark
        {
            // These proeprties can be bound to. The XAML for this control binds the Watermark's content to this.
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
        #endregion TextBox
        #endregion properties
    }
}
