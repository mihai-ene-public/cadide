using IDE.Core;
using IDE.Core.Commands;
using IDE.Core.Settings;
using IDE.Core.Storage;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using IDE.Core.Interfaces;
using System;
using System.Text.RegularExpressions;
using IDE.Core.Presentation.Licensing;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class LicenseActivationDialogViewModel : DialogViewModel
    {

        public string WindowTitle
        {
            get
            {
                return "License activation";
            }
        }

        string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        string validationCode;
        public string ValidationCode
        {
            get
            {
                return validationCode;
            }
            set
            {
                validationCode = value;
                OnPropertyChanged(nameof(ValidationCode));
            }
        }

        string statusLabel;
        public string StatusLabel
        {
            get { return statusLabel; }
            set
            {
                statusLabel = value;
                OnPropertyChanged(nameof(StatusLabel));

            }
        }

        ICommand activateLicenseCommand;
        public ICommand ActivateLicenseCommand
        {
            get
            {
                if (activateLicenseCommand == null)
                {
                    activateLicenseCommand = CreateCommand(async p =>
                      {
                          try
                          {
                              StatusLabel = "Activating...";

                              await ActivateLicenseAsync();

                              WindowCloseResult = true;
                          }
                          catch (Exception ex)
                          {
                              StatusLabel = $"Could not activate the license: {ex.Message}";
                          }
                      });
                }

                return activateLicenseCommand;
            }
        }

        ICommand deactivateLicenseCommand;
        public ICommand DeactivateLicenseCommand
        {
            get
            {
                if (deactivateLicenseCommand == null)
                {
                    deactivateLicenseCommand = CreateCommand(async p =>
                    {
                        try
                        {
                            StatusLabel = "Deactivating...";

                            await DeactivateLicenseAsync();

                            StatusLabel = "License deactivated.";
                        }
                        catch (Exception ex)
                        {
                            StatusLabel = $"Could not deactivate the license: {ex.Message}";
                        }
                    });
                }

                return deactivateLicenseCommand;
            }
        }

        async Task DeactivateLicenseAsync()
        {
            ValidateEmail();

            ValidateValidationCode();

            var encodedMail = EncodeString(email);
            var encodedValidationCode = EncodeString(validationCode);

            var licenseManager = new LicenseManager();
            await  licenseManager.DeactivateStandardLicense(encodedMail, encodedValidationCode);
        }

        async Task ActivateLicenseAsync()
        {
            ValidateEmail();

            ValidateValidationCode();

            var encodedMail = EncodeString(email);
            var encodedValidationCode = EncodeString(validationCode);

            var licenseManager = new LicenseManager();
            await Task.Run(() => licenseManager.GenerateStandardLicense(encodedMail, encodedValidationCode));
        }

        string EncodeString(string value)
        {
            var radixEncoder = new RadixBase36Encoding();

            var encodedString = radixEncoder.Encode(value);
            encodedString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(encodedString));

            return encodedString;
        }

        void ValidateEmail()
        {
            if (string.IsNullOrEmpty(email))
                throw new Exception("Email was not specified");

            email = email.Trim();

            var emailRegex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$"
                                        , RegexOptions.IgnoreCase
                                        );

            if (!emailRegex.IsMatch(email))
                throw new Exception("Email provided is not valid");
        }

        void ValidateValidationCode()
        {
            if (string.IsNullOrEmpty(validationCode))
                throw new Exception("Validation Code was not specified");
        }

    }
}
