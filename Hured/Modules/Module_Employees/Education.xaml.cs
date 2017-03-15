using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Education.xaml
    /// </summary>
    public partial class Education
    {
        public Education()
        {
            InitializeComponent();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            if (!Functions.ValidateAllTextboxes(this))
            {
                return;
            }

            var education = new Образование
            {
                Документ = TbДокумент.Text,
                Номер = TbНомер.Text,
                Серия = TbСерия.Text,
                Дополнительно = TbДополнительно.Text,
                Тип = TbТип.Text,
                Квалификация = TbКвалификация.Text,
                НачалоОбучения = DateTime.Parse(DpBegin.Text),
                КонецОбучения = DateTime.Parse(DpBegin.Text),
                Специальность = TbСпециальность.Text,
                Учреждение = TbУчреждение.Text
            };
            Tag = education;
            DialogResult = true;
            Close();
        }
    }
}
