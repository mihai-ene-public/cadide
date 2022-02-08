namespace IDE.Core.Interfaces
{
    public enum XMessageBoxButton
    {
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an OK button. This field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        OK = 0x00000000,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains OK and Cancel button. This field
        ///       is
        ///       constant.
        ///    </para>
        /// </devdoc>
        OKCancel = 0x00000001,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains Yes, No, and Cancel button. This
        ///       field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        YesNoCancel = 0x00000003,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains Yes and No button. This field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        YesNo = 0x00000004,

        // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxButton()
    }
}
