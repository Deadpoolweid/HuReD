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
        public Employee(Сотрудник employee = null)
        {
            InitializeComponent();

            Functions.AddUnitsFromDB(ref cbUnit);

            cbUnit.SelectedIndex = 0;
            cbPosition.SelectedIndex = 0;

            if (employee != null)
            {
                Tag = employee.СотрудникId;

                IsEditMode = true;
                tbФИО.Text = employee.ОсновнаяИнформация.Фамилия + " " +
                    employee.ОсновнаяИнформация.Имя + " " + 
                    employee.ОсновнаяИнформация.Отчество;
                cbUnit.SelectedItem = employee.ОсновнаяИнформация.Должность.Подразделение.Название;
                cbPosition.SelectedItem = employee.ОсновнаяИнформация.Должность.Название;
                dpCurrentDate.DisplayDate = employee.ОсновнаяИнформация.ДатаПриема;
                tbДомашний.Text = employee.ОсновнаяИнформация.ДомашнийТелефон;
                tbИНН.Text = employee.ОсновнаяИнформация.ИНН;
                tbМобильный.Text = employee.ОсновнаяИнформация.МобильныйТелефон;
                if (employee.ОсновнаяИнформация.Пол == "Мужской")
                {
                    rbM.IsChecked = true;
                }
                else if (employee.ОсновнаяИнформация.Пол == "Женский")
                {
                    rbW.IsChecked = true;
                }
                tbДополнительно.Text = employee.ОсновнаяИнформация.Дополнительно;
                cbStatus.SelectedItem = employee.ОсновнаяИнформация.Статус;
                tbТабельныйНомер.Text = employee.ОсновнаяИнформация.ТабельныйНомер;
                // TODO Загрузка фотографии из БД

                cbDocumentType.SelectedItem = employee.УдостоверениеЛичности.Вид;
                dpДатаРождения.DisplayDate = employee.УдостоверениеЛичности.ДатаРождения;
                tbКем.Text = employee.УдостоверениеЛичности.КемВыдан;
                dpКогдаВыдан.DisplayDate = employee.УдостоверениеЛичности.КогдаВыдан;
                tbМестоРождения.Text = employee.УдостоверениеЛичности.МестоРождения;
                tbСерия.Text = employee.УдостоверениеЛичности.Серия;
                tbномер.Text = employee.УдостоверениеЛичности.Номер;
                tbПНаселённыйПункт.Text = employee.УдостоверениеЛичности.Прописка.НаселённыйПункт;
                tbПИндекс.Text = employee.УдостоверениеЛичности.Прописка.Индекс;
                tbПУлица.Text = employee.УдостоверениеЛичности.Прописка.Улица;
                tbПДом.Text = employee.УдостоверениеЛичности.Прописка.Дом;
                tbПКвартира.Text = employee.УдостоверениеЛичности.Прописка.Квартира;
                tbФНаселённыйПункт.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.НаселённыйПункт;
                tbФИндекс.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.Индекс;
                tbФУлица.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.Улица;
                tbФДом.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.Дом;
                tbФКвартира.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.Квартира;

                tbЗвание.Text = employee.ВоинскийУчёт.Звание;
                tbКатегорияГодности.Text = employee.ВоинскийУчёт.КатегорияГодности;
                tbКатегорияЗапаса.Text = employee.ВоинскийУчёт.КатегорияЗапаса;
                tbКодВУС.Text = employee.ВоинскийУчёт.КодВУС;
                tbНаименованиеВоенкомата.Text = employee.ВоинскийУчёт.НаименованиеВоенкомата;
                tbПрофиль.Text = employee.ВоинскийУчёт.Профиль;
                tbСостоитНаУчёте.Text = employee.ВоинскийУчёт.СостоитНаУчете;

                Educations = employee.Образование.ToList();
                SyncEducationList();
            }
    }

        private bool IsEditMode = false;

        private void SyncEducationList()
        {
            lvEducations.Items.Clear();

            foreach (var education in Educations)
            {
                lvEducations.Items.Add(education);
            }
        }

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
            // TODO Не отображается дата в режиме редактирования
            ImageConverter _imageConverter = new ImageConverter();
            //byte[] xByte = (byte[])_imageConverter.ConvertTo(iAvatar, typeof(byte[]));
            byte[] xByte = null;
            // TODO Исправить конвертацию изображения


            Controller.OpenConnection();
            var chosenPosition = Controller.Select(new Должность(), 
                q => q.Название == cbPosition.SelectedValue.ToString()).FirstOrDefault();

            Сотрудник EmployeeToAdd = new Сотрудник()
            {
                ОсновнаяИнформация = new ОсновнаяИнформация()
                {
                    Фамилия = tbФИО.Text.Split(' ')[0],
                    Имя = tbФИО.Text.Split(' ')[1],
                    Отчество = tbФИО.Text.Split(' ')[2],
                    Должность = chosenPosition,
                    ДатаПриема = dpCurrentDate.DisplayDate,
                    ДомашнийТелефон = tbДомашний.Text,
                    ИНН = tbИНН.Text,
                    МобильныйТелефон = tbМобильный.Text,
                    Пол = rbM.IsChecked == true ? "Мужской" : "Женский",
                    Дополнительно = tbДополнительно.Text,
                    Статус = cbStatus.SelectedValue?.ToString(),
                    ТабельныйНомер = tbТабельныйНомер.Text,
                    Фото = xByte
                },
                УдостоверениеЛичности = new УдостоверениеЛичности()
                {
                    Вид = cbDocumentType.SelectedValue?.ToString(),
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
            };
            EmployeeToAdd.Образование = Educations;

            if (IsEditMode)
            {
                Controller.Edit(q => q.СотрудникId == (int) Tag, EmployeeToAdd);
            }
            else
            {
            Controller.Insert(EmployeeToAdd);

            }
            Controller.CloseConnection();

            Tag = EmployeeToAdd.СотрудникId;
            DialogResult = true;
            Close();
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

        List<Образование> Educations = new List<Образование>();

        private void bAddEducation_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            Education w = new Education();
            w.ShowDialog();

            if (w.DialogResult == true)
            {
                Educations.Add(w.Tag as Образование);
            }


            this.IsEnabled = true;
            SyncEducationList();
        }

        private void bRemoveEducation_Click(object sender, RoutedEventArgs e)
        {
            Educations.Remove(lvEducations.SelectedItem as Образование);
            SyncEducationList();
        }

        private void CbUnit_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbPosition.Items.Clear();
                Functions.AddPositionsFromDB(ref cbPosition, cbUnit.SelectedValue.ToString());
            cbPosition.SelectedIndex = 0;
        }
    }
}
