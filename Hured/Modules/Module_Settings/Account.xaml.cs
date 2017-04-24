using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hured.Modules.Module_Auth;
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured.Modules.Module_Settings
{
    /// <summary>
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public partial class Account : MetroWindow
    {
        public Account(УчётнаяЗапись account = null)
        {
            InitializeComponent();

            if (account == null) return;
            tbLogin.Text = account.Login;
            if (account.IsAdmin)
            {
                rbIsAdmin.IsChecked = true;
            }
            else
            {
                rbIsNotAdmin.IsChecked = true;
            }

            lOldPassword.Visibility = Visibility.Visible;
            pboldPassword.Visibility = Visibility.Visible;
            lPassword.Content = "Новый пароль";
            IsEditMode = true;
        }

        private bool IsEditMode = false;

        private void BCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsEditMode)
            {
                if (!AuthController.IsCanSignIn(tbLogin.Text, pboldPassword.Password))
                {
                    Functions.ShowPopup(bOK,"Старый пароль введён неверно.");
                    return;
                }
            }

            if (pbPassword.Password != pbPasswordConfirmation.Password)
            {
                Functions.ShowPopup(bOK, "Пароли не совпадают");
                return;
            }

            var account = new УчётнаяЗапись()
            {
                Login = tbLogin.Text,
                Password = pbPassword.Password,
                IsAdmin = rbIsAdmin.IsChecked.Value
            };
            Tag = account;
            DialogResult = true;
            Close();
        }
    }
}
