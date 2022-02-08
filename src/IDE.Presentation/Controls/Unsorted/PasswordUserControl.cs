using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace IDE.Core.Controls
{
    public class PasswordUserControl : UserControl
    {

        public static readonly DependencyProperty SecurePasswordProperty =
           DependencyProperty.Register("SecurePassword", typeof(SecureString), typeof(PasswordUserControl),
               new PropertyMetadata(default(SecureString)));

        public static readonly DependencyProperty PasswordProperty =
         DependencyProperty.Register("Password", typeof(string), typeof(PasswordUserControl),
             new PropertyMetadata(default(string), OnPasswordChanged));


        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passCtrl = d as PasswordUserControl;

            passCtrl.Setpassword(passCtrl.Password);
        }

        PasswordBox passwordBox;
        public PasswordUserControl()
        {
            passwordBox = new PasswordBox();
            var grid = new Grid();
            grid.Children.Add(passwordBox);
            AddChild(grid);

            // Update DependencyProperty whenever the password changes
            passwordBox.PasswordChanged += (s, e) =>
            {
                if (isSettingPassword)
                    return;

                SecurePassword = ((PasswordBox)s).SecurePassword;
                Password = ((PasswordBox)s).Password;
            };
        }

        public SecureString SecurePassword
        {
            get { return (SecureString)GetValue(SecurePasswordProperty); }
            set { SetValue(SecurePasswordProperty, value); }
        }


        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        bool isSettingPassword;
        void Setpassword(string password)
        {
            isSettingPassword = true;
            passwordBox.Password = password;
            SetCaretToEnd();
            isSettingPassword = false;
        }

        private void SetCaretToEnd()
        {
            var passLen = passwordBox.Password.Length;
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { passLen, 0 });
        }
    }
}
