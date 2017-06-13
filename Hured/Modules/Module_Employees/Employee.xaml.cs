using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Hured.DataBase;
using Hured.Tables_templates;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee
    {
        public Employee(Сотрудник employee = null)
        {
            InitializeComponent();

            Functions.AddUnitsFromDB(ref CbUnit);

            CbUnit.SelectedIndex = 0;

            if (employee != null)
            {
                this.employee = employee;
                Tag = employee.СотрудникId;

                _isEditMode = true;
                TbФио.Text = employee.ОсновнаяИнформация.Фамилия + " " +
                    employee.ОсновнаяИнформация.Имя + " " +
                    employee.ОсновнаяИнформация.Отчество;

                var units = new List<ComboBoxItem>();
                foreach (var item in CbUnit.Items)
                {
                    units.Add(item as ComboBoxItem);
                }
                CbUnit.SelectedItem = units.Find(q => (q.Content as Подразделение).ПодразделениеId ==
                                                      employee.ОсновнаяИнформация.Должность.Подразделение.ПодразделениеId);
                units.Clear();
                foreach (var item in CbPosition.Items)
                {
                    units.Add(item as ComboBoxItem);
                }
                var selectedPosition = units.Find(q => Convert.ToInt32(q.Tag) ==
                                                       employee.ОсновнаяИнформация.Должность.ДолжностьId);
                CbPosition.SelectedItem = selectedPosition ;
                DpCurrentDate.Text = employee.ОсновнаяИнформация.ДатаПриема.ToShortDateString();
                TbДомашний.Text = employee.ОсновнаяИнформация.ДомашнийТелефон;
                TbИнн.Text = employee.ОсновнаяИнформация.Инн;
                TbМобильный.Text = employee.ОсновнаяИнформация.МобильныйТелефон;
                if (employee.ОсновнаяИнформация.Пол == "Мужской")
                {
                    RbM.IsChecked = true;
                }
                else if (employee.ОсновнаяИнформация.Пол == "Женский")
                {
                    RbW.IsChecked = true;
                }
                TbДополнительно.Text = employee.ОсновнаяИнформация.Дополнительно;
                TbТабельныйНомер.Text = employee.ОсновнаяИнформация.ТабельныйНомер;

                var img = Functions.ByteArrayToImage(employee.ОсновнаяИнформация.Фото);
                IAvatar.Source = Functions.GetImageStream(img);

                DpДатаРождения.Text = employee.УдостоверениеЛичности.ДатаРождения.ToShortDateString();
                TbКем.Text = employee.УдостоверениеЛичности.КемВыдан;
                DpКогдаВыдан.Text = employee.УдостоверениеЛичности.КогдаВыдан.ToShortDateString();
                TbМестоРождения.Text = employee.УдостоверениеЛичности.МестоРождения;
                TbСерия.Text = employee.УдостоверениеЛичности.Серия;
                Tbномер.Text = employee.УдостоверениеЛичности.Номер;
                TbПНаселённыйПункт.Text = employee.УдостоверениеЛичности.Прописка.НаселённыйПункт;
                TbПИндекс.Text = employee.УдостоверениеЛичности.Прописка.Индекс;
                TbПУлица.Text = employee.УдостоверениеЛичности.Прописка.Улица;
                TbПДом.Text = employee.УдостоверениеЛичности.Прописка.Дом;
                TbПКвартира.Text = employee.УдостоверениеЛичности.Прописка.Квартира;
                TbФНаселённыйПункт.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.НаселённыйПункт;
                TbФИндекс.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.Индекс;
                TbФУлица.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.Улица;
                TbФДом.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.Дом;
                TbФКвартира.Text = employee.УдостоверениеЛичности.ФактическоеМестоЖительства.Квартира;

                TbЗвание.Text = employee.ВоинскийУчёт.Звание;
                TbКатегорияГодности.Text = employee.ВоинскийУчёт.КатегорияГодности;
                TbКатегорияЗапаса.Text = employee.ВоинскийУчёт.КатегорияЗапаса;
                TbКодВус.Text = employee.ВоинскийУчёт.КодВус;
                TbНаименованиеВоенкомата.Text = employee.ВоинскийУчёт.НаименованиеВоенкомата;
                TbПрофиль.Text = employee.ВоинскийУчёт.Профиль;
                TbСостоитНаУчёте.Text = employee.ВоинскийУчёт.СостоитНаУчете;

                _educations = employee.Образование.ToList();
                SyncEducationList();

                TbEMail.Text = employee.ДополнительнаяИнформация.EMail;
                TbSkype.Text = employee.ДополнительнаяИнформация.Skype;
            }
        }

        private readonly bool _isEditMode;

        private Сотрудник employee;

        private void SyncEducationList()
        {
            LvEducations.Items.Clear();

            foreach (var education in _educations)
            {
                LvEducations.Items.Add(education);
            }
        }

        private void bChooseImage_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                IAvatar.Source = new BitmapImage(new Uri(ofd.FileName));

            }

        }

        private void bDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            IAvatar.Source = null;
        }

        private void bSetStandartImage_Click(object sender, RoutedEventArgs e)
        {
            IAvatar.Source = Imaging.CreateBitmapSourceFromHBitmap(
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
            if (!Functions.ValidateAllTextboxes(this))
            {
                return;
            }

            // TODO Заменить регуляркой
            if (TbФио.Text.Split(' ').Length < 3)
            {
                Functions.ShowPopup(TbФио, "Введите фамилию, имя и отчество через пробел.");
                return;
            }

            var numericTextboxex = new List<TextBox>
            {
                TbДомашний,
                TbИнн,
                TbМобильный,
                TbПИндекс,
                TbСерия,
                TbФИндекс,
                TbТабельныйНомер,
                Tbномер
            };

            if (numericTextboxex.Any(textBox => !Functions.IsNumber(textBox)))
            {
                return;
            }

            string серия = TbСерия.Text, номер = Tbномер.Text;


            if (!_isEditMode)
            {
                Controller.OpenConnection();
                if (Controller.Exists<Сотрудник>(q => q.УдостоверениеЛичности.Серия == серия) &&
                    Controller.Exists<Сотрудник>(q => q.УдостоверениеЛичности.Номер == номер))
                {
                    Functions.ShowPopup(this, "Сотрудник с указанными паспортными данными уже существует.");
                    Controller.CloseConnection();
                    return;
                }
                Controller.CloseConnection(); 
            }

            var xByte = Functions.ImageSourceToBytes(new PngBitmapEncoder(), IAvatar.Source);

            var selectedPosition = (CbPosition.SelectedItem as ComboBoxItem)?.Tag;
            if (selectedPosition != null)
            {
                var positionId = (int)selectedPosition;
                Controller.OpenConnection();
                var chosenPosition = Controller.Select<Должность>(
                    q => q.ДолжностьId == positionId).FirstOrDefault();

                var employeeToAdd = new Сотрудник
                {
                    ОсновнаяИнформация = new ОсновнаяИнформация
                    {
                        Фамилия = TbФио.Text.Split(' ')[0],
                        Имя = TbФио.Text.Split(' ')[1],
                        Отчество = TbФио.Text.Split(' ')[2],
                        Должность = chosenPosition,
                        ДатаПриема = DateTime.Parse(DpCurrentDate.Text),
                        ДомашнийТелефон = TbДомашний.Text,
                        Инн = TbИнн.Text,
                        МобильныйТелефон = TbМобильный.Text,
                        Пол = RbM.IsChecked == true ? "Мужской" : "Женский",
                        Дополнительно = TbДополнительно.Text,
                        ТабельныйНомер = TbТабельныйНомер.Text,
                        Фото = xByte
                    },
                    УдостоверениеЛичности = new УдостоверениеЛичности
                    {
                        ДатаРождения = DateTime.Parse(DpДатаРождения.Text),
                        КемВыдан = TbКем.Text,
                        КогдаВыдан = DateTime.Parse(DpКогдаВыдан.Text),
                        МестоРождения = TbМестоРождения.Text,
                        Серия = TbСерия.Text,
                        Номер = Tbномер.Text,
                        Прописка = new Адрес
                        {
                            НаселённыйПункт = TbПНаселённыйПункт.Text,
                            Индекс = TbПИндекс.Text,
                            Улица = TbПУлица.Text,
                            Дом = TbПДом.Text,
                            Корпус = TbПКорпус.Text,
                            Квартира = TbПКвартира.Text
                        },
                        ФактическоеМестоЖительства = new Адрес
                        {
                            НаселённыйПункт = TbФНаселённыйПункт.Text,
                            Индекс = TbФИндекс.Text,
                            Улица = TbФУлица.Text,
                            Дом = TbФДом.Text,
                            Корпус = TbФКорпус.Text,
                            Квартира = TbФКвартира.Text
                        }
                    },
                    ВоинскийУчёт = new ВоинскийУчёт
                    {
                        Звание = TbЗвание.Text,
                        КатегорияГодности = TbКатегорияГодности.Text,
                        КатегорияЗапаса = TbКатегорияЗапаса.Text,
                        КодВус = TbКодВус.Text,
                        НаименованиеВоенкомата = TbНаименованиеВоенкомата.Text,
                        Профиль = TbПрофиль.Text,
                        СостоитНаУчете = TbСостоитНаУчёте.Text
                    },
                    ДополнительнаяИнформация = new ДополнительнаяИнформация
                    {
                        Skype = TbSkype.Text,
                        EMail = TbEMail.Text
                    },
                    Образование = _educations
                };

                if (_isEditMode)
                {
                    var result = Controller.Edit<ОсновнаяИнформация>(
                        q => q.ОсновнаяИнформацияId == employee.ОсновнаяИнформация.ОсновнаяИнформацияId,
                        employeeToAdd.ОсновнаяИнформация);
                    result = Controller.Edit<УдостоверениеЛичности>(
                        q => q.УдостоверениеЛичностиId == employee.УдостоверениеЛичности.УдостоверениеЛичностиId,
                        employeeToAdd.УдостоверениеЛичности);
                    result = Controller.Edit<ВоинскийУчёт>(
                        q => q.ВоинскийУчётId == employee.ВоинскийУчёт.ВоинскийУчётId,
                        employeeToAdd.ВоинскийУчёт);
                    result = Controller.Edit<ДополнительнаяИнформация>(
                        q => q.ДополнительнаяИнформацияId == employee.ДополнительнаяИнформация.ДополнительнаяИнформацияId,
                        employeeToAdd.ДополнительнаяИнформация);

                    //Controller.Edit(q => q.СотрудникId == (int)Tag, employeeToAdd);
                    // TODO Изменять все поля, а не только запись о сотруднике, что позволит избежать дублирования данных
                }
                else
                {
                    Controller.Insert(employeeToAdd);

                }
                Controller.CloseConnection();

                Tag = employeeToAdd.СотрудникId;
            }
            DialogResult = true;
            Close();
        }

        private void chbSameAsRegistration_Changed(object sender, RoutedEventArgs e)
        {
            if (ChbSameAsRegistration.IsChecked == true)
            {
                TbФДом.Text = TbПДом.Text;
                TbФИндекс.Text = TbПИндекс.Text;
                TbФКвартира.Text = TbПКвартира.Text;
                TbФКорпус.Text = TbПКорпус.Text;
                TbФНаселённыйПункт.Text = TbПНаселённыйПункт.Text;
                TbФУлица.Text = TbПУлица.Text;
            }
            else
            {
                TbФДом.Text = "";
                TbФИндекс.Text = "";
                TbФКвартира.Text = "";
                TbФКорпус.Text = "";
                TbФНаселённыйПункт.Text = "";
                TbФУлица.Text = "";
            }
        }

        readonly List<Образование> _educations = new List<Образование>();

        private void bAddEducation_Click(object sender, RoutedEventArgs e)
        {
            IsHitTestVisible = false;

            var w = new Education();
            w.ShowDialog();

            if (w.DialogResult == true)
            {
                _educations.Add(w.Tag as Образование);
            }

            IsHitTestVisible = true;
            SyncEducationList();
        }

        private void bRemoveEducation_Click(object sender, RoutedEventArgs e)
        {
            _educations.Remove(LvEducations.SelectedItem as Образование);
            SyncEducationList();
        }

        private void CbUnit_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CbPosition.Items.Clear();
            var comboBoxItem = CbUnit.SelectedItem as ComboBoxItem;
            if (comboBoxItem != null)
                Functions.AddPositionsFromDB(ref CbPosition, (int)comboBoxItem.Tag);
            CbPosition.SelectedIndex = 0;
        }
    }
}
