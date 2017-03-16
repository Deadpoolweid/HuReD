using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Hured.DataBase;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Status.xaml
    /// </summary>
    public partial class Status
    {
        public Status(Статус status = null)
        {
            InitializeComponent();

            if (status != null)
            {
                _isEditMode = true;
                TbName.Text = status.Название;
                var r = Convert.ToByte(status.Цвет.Split(' ')[0]);
                var g = Convert.ToByte(status.Цвет.Split(' ')[1]);
                var b = Convert.ToByte(status.Цвет.Split(' ')[2]);


                _selectedColor = Color.FromRgb(r, g, b);
                Tag = status.СтатусId;
            }

            BColor.Background = new SolidColorBrush(_selectedColor);
        }

        private readonly bool _isEditMode;

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            if (Functions.IsEmpty(TbName))
            {
                return;
            }

            Controller.OpenConnection();


            var status = new Статус
            {
                Название = TbName.Text,
                Цвет = _selectedColor.R + " " +
                    _selectedColor.G + " " +
                    _selectedColor.B
            };

            if (_isEditMode)
            {
                var id = (int)Tag;
                Controller.Edit(q => q.СтатусId == id, status);
            }
            else
            {
                Controller.Insert(status);
            }


            Controller.CloseConnection();

            DialogResult = true;
            Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private Color _selectedColor = Colors.White;

        private void BColor_OnClick(object sender, RoutedEventArgs e)
        {
            var colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            var color = colorDialog.Color;
            _selectedColor.A = color.A;
            _selectedColor.R = color.R;
            _selectedColor.G = color.G;
            _selectedColor.B = color.B;
            BColor.Background = new SolidColorBrush(_selectedColor);
        }

        private void Status_OnClosing(object sender, CancelEventArgs e)
        {
            DialogResult = false;
        }
    }
}
