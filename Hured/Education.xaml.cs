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
using Hured.DBModel;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Education.xaml
    /// </summary>
    public partial class Education : MetroWindow
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
            Образование education = new Образование()
            {
                Документ = tbДокумент.Text,
                Номер = tbНомер.Text,
                Серия = tbСерия.Text,
                Дополнительно = tbДополнительно.Text,
                Тип = tbТип.Text,
                Квалификация = tbКвалификация.Text,
                НачалоОбучения = dpBegin.DisplayDate,
                КонецОбучения = dpBegin.DisplayDate,
                Специальность = tbСпециальность.Text,
                Учреждение = tbУчреждение.Text,
            };
            Tag = education;
            DialogResult = true;
            Close();
        }
    }
}
