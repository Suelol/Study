using Study_Kamalov_wpf_320P.Connection;
using Study_Kamalov_wpf_320P.DbModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Study_Kamalov_wpf_320P.Pages
{
    public partial class KafedrasPage : Page
    {
        public KafedrasPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var kafedras = ConnectionHelper.db.Kafedra
                .Include("Faculty") // Изменено здесь
                .OrderBy(k => k.Name_kaf)
                .ToList();
            KafedrasGrid.ItemsSource = kafedras;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            var filtered = ConnectionHelper.db.Kafedra
                .Include("Faculty") // Изменено здесь
                .Where(k => k.Name_kaf.ToLower().Contains(searchText) ||
                            k.Shifr.ToLower().Contains(searchText) ||
                            (k.Faculty != null && k.Faculty.Name_faculty.ToLower().Contains(searchText)))
                .OrderBy(k => k.Name_kaf)
                .ToList();

            KafedrasGrid.ItemsSource = filtered;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddKafedraPage());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedKafedra = KafedrasGrid.SelectedItem as Kafedra;
            if (selectedKafedra != null)
            {
                NavigationService.Navigate(new EditKafedraPage(selectedKafedra));
            }
            else
            {
                MessageBox.Show("Выберите кафедру для редактирования");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedKafedra = KafedrasGrid.SelectedItem as Kafedra;
            if (selectedKafedra != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить кафедру {selectedKafedra.Name_kaf}?",
                                             "Подтверждение удаления",
                                             MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Проверка на связанные записи
                        if (selectedKafedra.Discipline.Any() || selectedKafedra.Employee.Any() || selectedKafedra.Speciality.Any())
                        {
                            MessageBox.Show("Невозможно удалить кафедру, так как есть связанные записи в других таблицах.");
                            return;
                        }

                        ConnectionHelper.db.Kafedra.Remove(selectedKafedra);
                        ConnectionHelper.db.SaveChanges();

                        LoadData();
                        MessageBox.Show("Кафедра успешно удалена");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите кафедру для удаления");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}