namespace IDE.Core.Interfaces
{
    public enum XMessageBoxImage
    {
        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contain no symbols. 
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        None = 0,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains a
        ///       hand symbol. 
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        Hand = 0x00000010,

        /// <devdoc>
        ///    <para>
        ///       Specifies
        ///       that the message
        ///       box contains a question
        ///       mark symbol. 
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        Question = 0x00000020,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an
        ///       exclamation symbol. 
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        Exclamation = 0x00000030,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an
        ///       asterisk symbol. 
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        Asterisk = 0x00000040,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box contains a hand icon. This field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        Stop = Hand,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains a
        ///       hand icon. 
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        Error = Hand,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the message box contains an exclamation icon. 
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        Warning = Exclamation,

        /// <devdoc>
        ///    <para>
        ///       Specifies that the
        ///       message box contains an
        ///       asterisk icon. 
        ///    </para>
        /// </devdoc>
        /// <ExternalAPI/> 
        Information = Asterisk,

        // NOTE: if you add or remove any values in this enum, be sure to update MessageBox.IsValidMessageBoxIcon()    
    }
}
