using System;

namespace IDE.Core.Presentation.Licensing
{
    class LicenseValidator
    {
        public const int TrialDays = 30;

        public LicenseStatus Validate(LicenseEntity license, out string validationMsg)
        {
            validationMsg = string.Empty;

            switch(license.LicenseType)
            {
                case LicenseType.Standard:
                    if (license.HardwareId == HardwareInfo.GenerateUID(license.AppName))
                    {
                        return LicenseStatus.VALID;
                    }
                    else
                    {
                        validationMsg = "The license is NOT for this copy!";
                    }
                    break;

                case LicenseType.Trial:
                    {

                        var daysCurrent = (DateTime.UtcNow - license.CreateDateTime).Days;
                        var isValid = daysCurrent <= TrialDays;

                        if(isValid)
                        {
                            return LicenseStatus.VALID;
                        }
                        else
                        {
                            validationMsg = "Trial license has expired";
                        }
                        break;
                    }

                case LicenseType.Floating:
                    return LicenseStatus.VALID;

                default:
                    validationMsg = "Invalid license";
                    break;
            }
           

            return LicenseStatus.INVALID;

        }

        //returns true if clock was manipulated
        bool IsClockManipulated(DateTime thresholdTime)
        {
            DateTime adjustedThresholdTime = new DateTime(thresholdTime.Year, thresholdTime.Month, thresholdTime.Day, 23, 59, 59);

            //EventLog eventLog = new System.Diagnostics.EventLog("system");

            //foreach (EventLogEntry entry in eventLog.Entries)
            //{
            //    if (entry.TimeWritten > adjustedThresholdTime)
            //        return true;
            //}

            return false;
        }
    }
}
