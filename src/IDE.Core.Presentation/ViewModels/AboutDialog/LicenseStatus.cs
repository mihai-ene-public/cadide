namespace IDE.Dialogs.About
{
    using IDE.Core.Presentation.Licensing;
    using System;
    using System.Diagnostics;
    using System.Text;

    public class LicenseStatus
    {

        public string GetLicenseStatusString()
        {
            try
            {


                var sb = new StringBuilder();

#if !DEBUG

#if Standard

                var licManager = new LicenseManager();

                var licenseInfo = licManager.GetLicenseInfo();

                if (licenseInfo != null)
                {
                    sb.Append(licenseInfo.ToString());
                }
                else
                {
                    sb.Append("Unknown");
                }

#elif Light

                sb.Append("Free");
#endif

#else
            sb.Append("YES - Debug");

#endif
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
