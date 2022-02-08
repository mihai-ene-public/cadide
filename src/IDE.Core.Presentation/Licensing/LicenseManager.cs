using IDE.Core.ViewModels;
using System.IO;
using System.Reflection;
using System.Security;
using System;
using System.Collections.Generic;
using IDE.Core.Common.API;
using System.Threading.Tasks;
using IDE.Core.Utilities;
using IDE.Documents.Views;

namespace IDE.Core.Presentation.Licensing
{
    public class LicenseManager
    {
        string GetPublicKeyXmlString()
        {
            return
@"<RSAParameters>
  <Exponent>AQAB</Exponent>
  <Modulus>rwMzi/wVT58AxUE8EAfXQ94J0fd65/Ah3FWZTdszYZLlxs8TcHKx52jqAuv9CqSRz74MxHaW5UQG2PfhdsBc8MOjOd04XY2krf3JJEwLW4nba9Qm+G9wd8av3bNQ/VaNwKOBnmfFwWFI9SP0Ll30Z4Yn4QflLUI4OxvWVAv+GMuERJMlrGJeq4PGtMFI9Aic7g6ZoiH2mj/XsTCZbS0/vgtJGs2ylzH07Rz1Ls7VOHzqw0hz9Ioq5sfh4pXUezBGlaS5YkmsH6P6BYy1ZpGpi9mJUOKobinhf0YMDPtlz+OTJ6ns+p8NYDBvjL0fXQGLip9rPWiaY77CJkmvc2qroQ==</Modulus>
</RSAParameters>";
        }



        //called on first load of the app if no licenses are present
        public void GenerateTrialLicense()
        {
            GenerateLicenseFromServer(LicenseType.Trial);
        }

        //called when user wants to activate a license
        public void GenerateStandardLicense(string email, string activationCode)
        {
            GenerateLicenseFromServer(LicenseType.Standard, email, activationCode);
        }

        public async Task DeactivateStandardLicense(string email, string activationCode)
        {
            var appName = $"{AppHelpers.ApplicationFullTitle}-v{AppHelpers.ApplicationVersionMajor}";
            var hardwareId = HardwareInfo.GenerateUID(appName);

            var url = "api/license/deactivate";

            var headers = new Dictionary<string, string>
            {
                [nameof(appName)] = appName,
                [nameof(hardwareId)] = hardwareId
            };
            if (email != null)
                headers[nameof(email)] = email;
            if (activationCode != null)
                headers[nameof(activationCode)] = activationCode;

            var apiHelper = new APIHelper();
            var success = await apiHelper.GetResponse<bool>(url, headers);

            if (success)
            {
                //delete license file
                var filePath = AppHelpers.LicenseFilePath;
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        void GenerateLicenseFromServer(LicenseType licenseType, string email = null, string activationCode = null)
        {
            var appName = $"{AppHelpers.ApplicationFullTitle}-v{AppHelpers.ApplicationVersionMajor}";
            var hardwareId = HardwareInfo.GenerateUID(appName);

            var url = licenseType switch
            {
                LicenseType.Trial => "api/license/trial",
                LicenseType.Standard => "api/license/activate",
                _ => "api/license/trial"
            };

            var headers = new Dictionary<string, string>
            {
                [nameof(appName)] = appName,
                [nameof(hardwareId)] = hardwareId
            };
            if (email != null)
                headers[nameof(email)] = email;
            if (activationCode != null)
                headers[nameof(activationCode)] = activationCode;

            var apiHelper = new APIHelper();
            var licenseString = Task.Run(async () => await apiHelper.GetResponse<string>(url, headers)).GetAwaiter().GetResult();

            ValidateLicense(licenseString);

            //save license file
            var filePath = AppHelpers.LicenseFilePath;
            SaveLicenseFile(filePath, licenseString);
        }

        /// <summary>
        /// Validates the license content. Throws exception when invalid
        /// </summary>
        /// <param name="licenseString"></param>
        void ValidateLicense(string licenseString)
        {
            var licenseStatus = LicenseStatus.UNDEFINED;
            var validationMessage = string.Empty;

            var pubXmlString = GetPublicKeyXmlString();
            var license = LicenseHandler.ParseLicenseFromBASE64String(typeof(LicenseEntity), licenseString, pubXmlString, out licenseStatus, out validationMessage);

            if (licenseStatus != LicenseStatus.VALID)
                throw new LicenseInvalidException(validationMessage);

        }

        void SaveLicenseFile(string licenseFilePath, string licenseContent)
        {
            File.WriteAllText(licenseFilePath, licenseContent);
        }

        /// <summary>
        /// called o startup
        /// </summary>
        public void VerifyLicensing()
        {
#if Standard


            var licensePath = AppHelpers.LicenseFilePath;

            if (File.Exists(licensePath))
            {
                var licenseString = File.ReadAllText(licensePath);

                try
                {
                    ValidateLicense(licenseString);
                }
                catch (LicenseInvalidException ex)
                {
                    if (MessageDialog.Show(ex.Message + Environment.NewLine +
                                            "Do you want to activate the license?",
                                          "License invalid",
                                          Interfaces.XMessageBoxButton.YesNo) == Interfaces.XMessageBoxResult.Yes)
                    {
                        var vm = new LicenseActivationDialogViewModel();

                        vm.ShowDialog();
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                GenerateTrialLicense();
            }

#endif
        }

        public LicenseInfo GetLicenseInfo()
        {

            LicenseInfo licenseInfo = null;

#if Standard

            var licensePath = AppHelpers.LicenseFilePath;

            if (File.Exists(licensePath))
            {
                var licenseString = File.ReadAllText(licensePath);

                var licenseStatus = LicenseStatus.UNDEFINED;
                var validationMessage = string.Empty;

                var pubXmlString = GetPublicKeyXmlString();
                var license = LicenseHandler.ParseLicenseFromBASE64String(typeof(LicenseEntity), licenseString, pubXmlString, out licenseStatus, out validationMessage);

                return new LicenseInfo
                {
                    LicenseStatus = licenseStatus,
                    LicenseType = license.LicenseType,
                    License = license
                };
            }

            licenseInfo = new LicenseInfo
            {
                LicenseStatus = LicenseStatus.INVALID,
                LicenseType = LicenseType.None
            };

#endif

            return licenseInfo;
        }
    }
}
