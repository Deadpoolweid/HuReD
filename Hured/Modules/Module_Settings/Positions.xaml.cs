using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Hured.DataBase;
using Hured.Tables_templates;

namespace Hured
{
    /// <summary>
    /// Логика взаимодействия для Positions.xaml
    /// </summary>
    public partial class Positions
    {
        public Positions()
        {
            Closing += Positions_OnClosing;

            InitializeComponent();
            Functions.AddUnitsFromDB(ref LbUnits);
            LbUnits.Items.Insert(0,new ListViewItem()
            {
                Content = new Подразделение()
                {
                    Название = "Все подразделения",
                    ПодразделениеId = -1
                },
                Tag = -1
            });
            LbUnits.SelectedIndex = 0;

            SyncPositions();

            Functions.AddSortingToListView(LvPositions);
        }

        private readonly TransactionResult _tResult = new TransactionResult();

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;

                var w = new Position();
                w.ShowDialog();


                _tResult.RecordsAdded++;

                SyncPositions();
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось добавить должность. Информация: " + ex);
            }
            finally
            {
                IsHitTestVisible = true;
            }

        }

        private void bChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;


                var listViewItem = LvPositions.SelectedItem as ListViewItem;
                if (listViewItem != null)
                {
                    var position = listViewItem.Content as Должность;

                    var w = new Position(position);
                    w.ShowDialog();
                    _tResult.RecordsChanged++;
                }

                SyncPositions();
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось изменить должность. Информация: " + ex);
            }
            finally
            {
                IsHitTestVisible = true;
            }
        }

        private void bRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsHitTestVisible = false;

                Controller.OpenConnection();


                var tag = (LvPositions.SelectedItem as ListViewItem)?.Tag;
                if (tag != null)
                {
                    int positionId = (int)tag;

                    Controller.Remove<Должность>(
                        q => q.ДолжностьId == positionId);
                    Controller.CloseConnection();
                }
                _tResult.RecordsDeleted++;

                SyncPositions();
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(sender as Button, "Не удалось удалить должность. Информация: " + ex);
            }
            finally
            {
                Controller.CloseConnection(true);
                IsHitTestVisible = true;
            }
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }

        private void Positions_OnClosing(object sender, CancelEventArgs e)
        {
            Controller.OpenConnection();
            _tResult.RecordsCount = Controller.RecordsCount<Должность>();
            Controller.CloseConnection();


            Tag = _tResult;
        }

        private void SyncPositions()
        {
            try
            {
                LvPositions.Items.Clear();

                string[] filter = null;

                if (tbSearch.Text != String.Empty && !tbSearch.IsHavePlaceholder())
                {
                    filter = tbSearch.Text.Split(' ');
                }

                if (LbUnits.SelectedIndex == 0)
                {
                    Functions.AddPositionsFromDB(ref LvPositions, filter: filter);
                }
                else
                {
                    var tag = (LbUnits.SelectedItem as ListBoxItem)?.Tag;
                    if (tag != null)
                        Functions.AddPositionsFromDB(ref LvPositions,
                            (int) tag, filter);
                }
            }
            catch (Exception ex)
            {
                Functions.ShowPopup(this, "Не удалось обновить список должностей. Окно будет закрыто. Информация: " + ex);
                Close();
            }
            finally
            {
                Controller.CloseConnection(true);
            }
        }

        private void LbUnits_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SyncPositions();
        }

        private void TbSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SyncPositions();
        }
    }
}