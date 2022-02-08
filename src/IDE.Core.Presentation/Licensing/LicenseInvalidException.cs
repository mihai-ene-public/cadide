using System;

namespace IDE.Core.Presentation.Licensing
{
    public class LicenseInvalidException : Exception
    {
        public LicenseInvalidException()
            : base("License is invalid")
        {

        }

        public LicenseInvalidException(string message) 
            : base(message)
        {

        }
    }
}
