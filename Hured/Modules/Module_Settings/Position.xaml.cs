﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Catel.Collections;
using Hured.DataBase;
using Hured.Tables_templates;
using MahApps.Metro.Controls;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Position.xaml
    /// </summary>
    public partial class Position
    {
        public Position(Должность position = null)
        {
            Closing += Position_OnClosing;

            InitializeComponent();
            Functions.AddUnitsFromDB(ref CbUnit);
            CbUnit.SelectedIndex = 0;

            var cbselecteditem = new ComboBoxItem();
            var units = new List<ComboBoxItem>();
            foreach (var item in CbUnit.Items)
            {
                units.Add(item as ComboBoxItem);
            }


            if (position != null)
            {
                _isEditMode = true;
                TbName.Text = _oldName = position.Название;
                TbРасписание.Text = position.Расписание;
                CbUnit.SelectedItem = units.Find(q => (q.Content as Подразделение).ПодразделениеId ==
                                                      position.Подразделение.ПодразделениеId);
            }
        }

        private readonly bool _isEditMode;
        private readonly string _oldName;

        private void bOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Functions.ValidateAllTextboxes(this))
                {
                    return;
                }


                Controller.OpenConnection();

                var tag = (CbUnit.SelectedItem as ComboBoxItem)?.Tag;
                if (tag != null)
                {
                    var unitId = (int)tag;

                    var unit = Controller.Select<Подразделение>(
                        q => q.ПодразделениеId == unitId).FirstOrDefault();

                    var position = new Должность
                    {
                        Название = TbName.Text,
                        Расписание = TbРасписание.Text,
                        Подразделение = unit
                    };


                    if (_isEditMode)
                    {
                        Controller.Edit(q => q.Название == _oldName, position);
                    }
                    else
                    {
                        Controller.Insert(position);

                    }
                }
                Controller.CloseConnection();


                DialogResult = true;
            }
            finally
            {
                Controller.CloseConnection(true);
            }

            Close();
        }

        private void Position_OnClosing(object sender, CancelEventArgs e)
        {
            
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
