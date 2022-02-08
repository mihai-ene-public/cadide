using System;
using System.Text;

namespace IDE.Core.Presentation.Licensing
{
    /// <summary>
    /// Provides info about licensing
    /// </summary>
    public class LicenseInfo
    {
        public LicenseType LicenseType { get; set; }

        public LicenseStatus LicenseStatus { get; set; }

        public LicenseEntity License { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (License != null)
            {
                var trialStatus = string.Empty;

                if (LicenseType == LicenseType.Trial)
                {
                    var daysCurrent = (DateTime.UtcNow - License.CreateDateTime).Days;
                    var daysRemaining = LicenseValidator.TrialDays - daysCurrent;

                    trialStatus = $" ({daysRemaining} days remaining)";
                }
                else
                {
                    if (LicenseStatus == LicenseStatus.VALID)
                    {
                        trialStatus = " (Licensed)";
                    }
                }
                sb.AppendLine($"{LicenseType}{trialStatus}");
            }
            return sb.ToString();
        }
    }
}
