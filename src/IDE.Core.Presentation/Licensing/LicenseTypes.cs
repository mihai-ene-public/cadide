using System.ComponentModel;

namespace IDE.Core.Presentation.Licensing
{
    public enum LicenseType
    {
        /// <summary>
        /// No type specified
        /// </summary>
        None,

        /// <summary>
        /// For trial use
        /// </summary>
        Trial,

        /// <summary>
        /// Standard license
        /// </summary>
        Standard,

        /// <summary>
        /// For personal use
        /// </summary>
        Personal,

        /// <summary>
        /// Floating /Volume license
        /// </summary>
        Floating,

        /// <summary>
        /// Subscription based license
        /// </summary>
        Subscription,

        /// <summary>
        /// Free license for the light edition
        /// </summary>
        Free
    }
}
