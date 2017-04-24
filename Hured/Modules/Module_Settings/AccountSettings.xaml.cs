using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using Hured.DataBase;
using Hured.Modules.Module_Auth;
using Hured.Tables_templates;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Hured.Modules.Module_Settings
{
    /// <summary>
    /// Логика взаимодействия для AccountSettings.xaml
    /// </summary>
    public partial class AccountSettings : MetroWindow
    {
        public AccountSettings()
        {
            InitializeComponent();

            SyncAccounts();
        }

        public void SyncAccounts()
        {
            try
            {
                LvAccounts.Items.Clear();

                Controller.OpenConnection();
                List<УчётнаяЗапись> accounts = Controller.SelectAll<УчётнаяЗапись>(); 

                foreach (var item in accounts.Select(
                    аккаунт => new ListViewItem
                    {
                        Content = аккаунт,
                        Tag = аккаунт.УчётнаяЗаписьId
                    }))
                {
                    if (((УчётнаяЗапись) item.Content).Login == "admin")
                    {
                        item.Background = Brushes.LightSeaGreen;
                    }
                    LvAccounts.Items.Add(item);
                }

                Controller.CloseConnection();
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(this, "Не удалось обновить список аккаунтов. Окно будет закрыто. Информация: " + ex);
                Close();
            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private void BAdd_OnClick(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;
            var w = new Account();
            w.ShowDialog();
            if (w.DialogResult == true)
            {
                // Добавить в базу нового пользователя

                try
                {
                    AuthController.AddUser(w.Tag as УчётнаяЗапись);
                }
                catch (Exception exception)
                {
                    Functions.ShowPopup(bAdd,exception.Message);
                }
                
            }

            SyncAccounts();

            IsHitTestVisible = true;
        }

        private void BClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void BRemove_OnClick(object sender, RoutedEventArgs e)
        {
            var login = ((LvAccounts.SelectedItem as ListViewItem).Content as УчётнаяЗапись).Login;

            if (login == "admin")
            {
                Functions.ShowPopup(bRemove,"Невозможно удалить учётную запись администратора.");
                return;
            }

            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Да",
                NegativeButtonText = "Нет",
                AnimateShow = true,
                AnimateHide = false
            };
            var result = await this.ShowMessageAsync("Предупреждение", $"Удалить пользователя {login}?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                AuthController.RemoveUser(account => account.Login == login);

                SyncAccounts();
            }

        }

        private void BChangePassword_OnClick(object sender, RoutedEventArgs e)
        {
            var item = LvAccounts.SelectedItem as ListViewItem;
            var selectedAccount = item.Content as УчётнаяЗапись;
            var w = new Account(selectedAccount);
            w.ShowDialog();
            if (w.DialogResult == true)
            {
                var newAccount = w.Tag as УчётнаяЗапись;
                AuthController.ChangePassword(account => account.Login == selectedAccount.Login,newAccount.Password);
            }
        }
    }
}
