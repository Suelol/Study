using Study_Kamalov_wpf_320P.Connection;
using Study_Kamalov_wpf_320P.DbModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Study_Kamalov_wpf_320P.Pages
{
    public partial class DisciplinesPage : Page
    {
        public DisciplinesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var disciplines = ConnectionHelper.db.Discipline
                .OrderBy(d => d.Name_discip)
                .ToList();
            DisciplinesGrid.ItemsSource = disciplines;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            var filtered = ConnectionHelper.db.Discipline
                .Where(d => d.Name_discip.ToLower().Contains(searchText) ||
                           d.Shifr.ToLower().Contains(searchText))
                .OrderBy(d => d.Name_discip)
                .ToList();

            DisciplinesGrid.ItemsSource = filtered;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddDisciplinePage());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDiscipline = DisciplinesGrid.SelectedItem as Discipline;
            if (selectedDiscipline != null)
            {
                NavigationService.Navigate(new EditDisciplinePage(selectedDiscipline));
            }
            else
            {
                MessageBox.Show("Выберите дисциплину для редактирования");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedDiscipline = DisciplinesGrid.SelectedItem as Discipline;
            if (selectedDiscipline != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить дисциплину {selectedDiscipline.Name_discip}?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаление связанных записей
                        var exams = ConnectionHelper.db.Exsamen
                            .Where(ex => ex.Kod == selectedDiscipline.Kod)
                            .ToList();
                        foreach (var exam in exams)
                        {
                            ConnectionHelper.db.Exsamen.Remove(exam);
                        }

                        var zayavki = ConnectionHelper.db.Zayavka
                            .Where(z => z.Kod == selectedDiscipline.Kod)
                            .ToList();
                        foreach (var zayavka in zayavki)
                        {
                            ConnectionHelper.db.Zayavka.Remove(zayavka);
                        }

                        // Удаление самой дисциплины
                        ConnectionHelper.db.Discipline.Remove(selectedDiscipline);
                        ConnectionHelper.db.SaveChanges();

                        LoadData();
                        MessageBox.Show("Дисциплина успешно удалена");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите дисциплину для удаления");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}