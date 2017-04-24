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

namespace Hured.Modules
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : MetroWindow
    {
        public Auth()
        {
            InitializeComponent();
            tbLogin.Focus();

            tbLogin.KeyDown += FieldsOnKeyDown;
            pbPassword.KeyDown += FieldsOnKeyDown;
        }

        private void FieldsOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key == Key.Enter)
            {
                BSignin_OnClick(sender,null);
            }
        }

        private void BSignin_OnClick(object sender, RoutedEventArgs e)
        {
            if (AuthController.IsCanSignIn(tbLogin.Text, pbPassword.Password))
            {
                DialogResult = true;
                AuthController.CurrentUser = tbLogin.Text;
            }
            else
            {
                Functions.ShowPopup(bSignin,"Неправильное имя пользователя или пароль.");
            }
        }
    }
}
