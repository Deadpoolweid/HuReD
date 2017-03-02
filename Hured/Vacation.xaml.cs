using System;
using System.ComponentModel;
using System.Windows;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Vacation.xaml
    /// </summary>
    public partial class Vacation
    {
        public Vacation(ПриказОтпуск order = null)
        {
            InitializeComponent();

            RbEveryYear.IsChecked = true;

            if (order == null) return;
            DpBegin.Text = order.НачалоОтпуска.ToShortDateString();
            DpEnd.Text = order.КонецОтпуска.ToShortDateString();
            DpПериодРаботыНачало.Text = order.ПериодРаботыНачало.ToShortDateString();
            DpПериодРаботыКонец.Text = order.ПериодРаботыКонец.ToShortDateString();

            switch (order.Вид)
            {
                case "Ежегодный":
                    RbEveryYear.IsChecked = true;
                    break;
                case "Единоразовый":
                    RbOnce.IsChecked = true;
                    break;
                default:
                    RbOther.IsChecked = true;
                    TbДругое.Text = order.Вид;
                    break;
            }
        }


        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            var vacationType = "";
            if (RbEveryYear.IsChecked == true)
            {
                vacationType = RbEveryYear.Content.ToString();
            }
            else if (RbOnce.IsChecked == true)
            {
                vacationType = RbOnce.Content.ToString();
            }
            else if (RbOther.IsChecked == true)
            {
                vacationType = TbДругое.Text;
            }

            var order = new ПриказОтпуск
            {
                НачалоОтпуска = DateTime.Parse(DpBegin.Text),
                КонецОтпуска = DateTime.Parse(DpEnd.Text),
                Вид = vacationType,
                ПериодРаботыНачало = DateTime.Parse(DpПериодРаботыНачало.Text),
                ПериодРаботыКонец = DateTime.Parse(DpПериодРаботыКонец.Text)
            };
            Tag = order;

            DialogResult = true;
            Close();
        }

        private void Vacation_OnClosing(object sender, CancelEventArgs e)
        {
            DialogResult = false;
        }
    }
}
