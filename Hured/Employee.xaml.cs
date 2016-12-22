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
using Microsoft.Win32;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : Window
    {
        public Employee()
        {
            InitializeComponent();
            // TODO Инициализация Combobox-ов
        }

        private void bChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();// создаем диалоговое окно
            ofd.ShowDialog();// открываем окно
            iAvatar.Source = new BitmapImage(new Uri(ofd.FileName));
        }

        private void bDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            iAvatar.Source = null;
        }

        private void bSetStandartImage_Click(object sender, RoutedEventArgs e)
        {
            iAvatar.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                      Properties.Resources.avatar.GetHbitmap(),
                      IntPtr.Zero,
                      Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            // TODO Добавить логику добавления сотрудника
        }

        private void chbSameAsRegistration_Changed(object sender, RoutedEventArgs e)
        {
            tbФДом.Text = tbПДом.Text;
            tbФИндекс.Text = tbПИндекс.Text;
            tbФКвартира.Text = tbПКвартира.Text;
            tbФКорпус.Text = tbПКорпус.Text;
            tbФНаселённыйПункт.Text = tbПНаселённыйПункт.Text;
            tbФУлица.Text = tbПУлица.Text;
        }

        private void bAddEducation_Click(object sender, RoutedEventArgs e)
        {
            // TODO Добавление образования вывод окна
        }

        private void bRemoveEducation_Click(object sender, RoutedEventArgs e)
        {
            // TODO реализация удаления образования
        }
    }
}
