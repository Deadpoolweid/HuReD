using System;
using System.Collections.Generic;
using System.Drawing;
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

            Controller.OpenConnection();

            foreach (var подразделение in Controller.Select(new Подразделение(), e => e != null))
            {
                cbUnit.Items.Add(подразделение.Название);
            }

            foreach (var должность in Controller.Select(new Должность(), e => e != null))
            {
                cbPosition.Items.Add(должность.Название);
            }

            Controller.CloseConnection();
        }

        private Сотрудник EmployeeToAdd = new Сотрудник();

        private void bChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();// создаем диалоговое окно
            if (ofd.ShowDialog() == true)
            {
                iAvatar.Source = new BitmapImage(new Uri(ofd.FileName));

            }// открываем окно

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
            ImageConverter _imageConverter = new ImageConverter();
            //byte[] xByte = (byte[])_imageConverter.ConvertTo(iAvatar, typeof(byte[]));
            byte[] xByte = null;
            // TODO Исправить конвертацию изображения


            Controller.OpenConnection();
            EmployeeToAdd = new Сотрудник()
            {
                ОсновнаяИнформация = new ОсновнаяИнформация()
                {
                    Фамилия = tbФИО.Text.Split(' ')[0],
                    Имя = tbФИО.Text.Split(' ')[1],
                    Отчество = tbФИО.Text.Split(' ')[2],
                    Должность = Controller.Select(new Должность(), q => q.Название == cbPosition.SelectedValue.ToString()).FirstOrDefault(),
                    ДатаПриема = dpCurrentDate.DisplayDate,
                    ДомашнийТелефон = tbДомашний.Text,
                    ИНН = tbИНН.Text,
                    МобильныйТелефон = tbМобильный.Text,
                    Пол = rbM.IsChecked == true ? "Мужской" : "Женский",
                    Дополнительно = tbДополнительно.Text,
                    Статус = cbStatus.SelectedValue.ToString(),
                    ТабельныйНомер = tbТабельныйНомер.Text,
                    Фото = xByte
                },
                УдостоверениеЛичности = new УдостоверениеЛичности()
                {
                    Вид = cbDocumentType.SelectedValue.ToString(),
                    ДатаРождения = dpДатаРождения.DisplayDate,
                    КемВыдан = tbКем.Text,
                    КогдаВыдан = dpКогдаВыдан.DisplayDate,
                    МестоРождения = tbМестоРождения.Text,
                    Серия = tbСерия.Text,
                    Номер = tbномер.Text,
                    Прописка = new Адрес()
                    {
                        НаселённыйПункт = tbПНаселённыйПункт.Text,
                        Индекс = tbПИндекс.Text,
                        Улица = tbПУлица.Text,
                        Дом = tbПДом.Text,
                        Корпус = tbПКорпус.Text,
                        Квартира = tbПКвартира.Text
                    },
                    ФактическоеМестоЖительства = new Адрес()
                    {
                        НаселённыйПункт = tbФНаселённыйПункт.Text,
                        Индекс = tbФИндекс.Text,
                        Улица = tbФУлица.Text,
                        Дом = tbФДом.Text,
                        Корпус = tbФКорпус.Text,
                        Квартира = tbФКвартира.Text
                    }
                },
                ВоинскийУчёт = new ВоинскийУчёт()
                {
                    Звание = tbЗвание.Text,
                    КатегорияГодности = tbКатегорияГодности.Text,
                    КатегорияЗапаса = tbКатегорияЗапаса.Text,
                    КодВУС = tbКодВУС.Text,
                    НаименованиеВоенкомата = tbНаименованиеВоенкомата.Text,
                    Профиль = tbПрофиль.Text,
                    СостоитНаУчете = tbСостоитНаУчёте.Text
                },

                // TODO Реализовать добавление образования
            };

            Controller.Insert(EmployeeToAdd);
            Controller.CloseConnection();

        }

        private void chbSameAsRegistration_Changed(object sender, RoutedEventArgs e)
        {
            if (chbSameAsRegistration.IsChecked == true)
            {
                tbФДом.Text = tbПДом.Text;
                tbФИндекс.Text = tbПИндекс.Text;
                tbФКвартира.Text = tbПКвартира.Text;
                tbФКорпус.Text = tbПКорпус.Text;
                tbФНаселённыйПункт.Text = tbПНаселённыйПункт.Text;
                tbФУлица.Text = tbПУлица.Text;
            }
            else
            {
                tbФДом.Text = "";
                tbФИндекс.Text = "";
                tbФКвартира.Text = "";
                tbФКорпус.Text = "";
                tbФНаселённыйПункт.Text = "";
                tbФУлица.Text = "";
            }
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
