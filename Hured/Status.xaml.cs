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
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Status.xaml
    /// </summary>
    public partial class Status : MetroWindow
    {
        public Status(Статус status = null)
        {
            InitializeComponent();

            if (status != null)
            {
                IsEditMode = true;
                tbName.Text = status.Название;
                byte r = Convert.ToByte(status.Цвет.Split(' ')[0]);
                byte g = Convert.ToByte(status.Цвет.Split(' ')[1]);
                byte b = Convert.ToByte(status.Цвет.Split(' ')[2]);


                cpColor.SelectedColor = Color.FromRgb(r, g, b);
                Tag = status.СтатусId;
            }
        }

        private bool IsEditMode = false;

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            Controller.OpenConnection();

            var status = new Статус()
            {
                Название = tbName.Text,
                Цвет = cpColor.SelectedColor.Value.R.ToString() + " " +
                    cpColor.SelectedColor.Value.G.ToString() + " " +
                    cpColor.SelectedColor.Value.B.ToString()
            };

            if (IsEditMode)
            {
                int id = (int) Tag;
                Controller.Edit(q => q.СтатусId == id, status);
            }
            else
            {
                Controller.Insert(status);
            }


            Controller.CloseConnection();

            // TODO Добавить логику добавления статуса сотрудника
            DialogResult = true;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
