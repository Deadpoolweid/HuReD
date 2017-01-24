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
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Vacation.xaml
    /// </summary>
    public partial class Vacation : MetroWindow
    {
        public Vacation(ПриказОтпуск order = null)
        {
            InitializeComponent();

            rbEveryYear.IsChecked = true;

            if (order != null)
            {
                dpBegin.Text = order.НачалоОтпуска.ToString();
                dpEnd.Text = order.КонецОтпуска.ToString();
                if (order.Вид == "Ежегодный")
                {
                    rbEveryYear.IsChecked = true;
                }
                else if (order.Вид == "Единоразовый")
                {
                    rbOnce.IsChecked = true;
                }
                else
                {
                    rbOther.IsChecked = true;
                    tbДругое.Text = order.Вид;
                }
                filePath = order.Файл;
            }
        }

        private string filePath;

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            string vacationType = "";
            if (rbEveryYear.IsChecked == true)
            {
                vacationType = rbEveryYear.Content.ToString();
            }
            else if (rbOnce.IsChecked == true)
            {
                vacationType = rbOnce.Content.ToString();
            }
            else if (rbOther.IsChecked == true)
            {
                vacationType = tbДругое.Text;
            }

            var order = new ПриказОтпуск()
            {
                НачалоОтпуска = DateTime.Parse(dpBegin.Text),
                КонецОтпуска = DateTime.Parse(dpEnd.Text),
                Вид = vacationType,
                Файл = filePath
            };
            Tag = order;

            DialogResult = true;
            Close();
        }
    }
}
