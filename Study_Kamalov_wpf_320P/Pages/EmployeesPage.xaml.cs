using Study_Kamalov_wpf_320P.Connection;
using Study_Kamalov_wpf_320P.DbModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Study_Kamalov_wpf_320P.Pages
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var employees = ConnectionHelper.db.Employee
                .OrderBy(e => e.Surname)
                .ToList();

            EmployeesGrid.ItemsSource = employees;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            var filtered = ConnectionHelper.db.Employee
                .Where(emp => emp.Surname.ToLower().Contains(searchText))
                .OrderBy(emp => emp.Surname)
                .ToList();

            EmployeesGrid.ItemsSource = filtered;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEmployeePage());
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeesGrid.SelectedItem as Employee;
            if (selectedEmployee != null)
            {
                NavigationService.Navigate(new EditEmployeePage(selectedEmployee));
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для редактирования");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeesGrid.SelectedItem as Employee;
            if (selectedEmployee != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {selectedEmployee.Surname}?",
                                             "Подтверждение удаления",
                                             MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем связанные записи
                        var teacher = ConnectionHelper.db.Teacher.FirstOrDefault(t => t.Tab_number == selectedEmployee.Tab_number);
                        if (teacher != null)
                            ConnectionHelper.db.Teacher.Remove(teacher);

                        var engineer = ConnectionHelper.db.Enginer.FirstOrDefault(eng => eng.Tab_number == selectedEmployee.Tab_number);
                        if (engineer != null)
                            ConnectionHelper.db.Enginer.Remove(engineer);

                        var zavKaf = ConnectionHelper.db.Zav_kaf.FirstOrDefault(z => z.Tab_number == selectedEmployee.Tab_number);
                        if (zavKaf != null)
                            ConnectionHelper.db.Zav_kaf.Remove(zavKaf);

                        // Удаляем экзамены, связанные с преподавателем
                        var exams = ConnectionHelper.db.Exsamen.Where(ex => ex.Tab_number == selectedEmployee.Tab_number).ToList();
                        foreach (var exam in exams)
                        {
                            ConnectionHelper.db.Exsamen.Remove(exam);
                        }

                        // Теперь можно удалить самого сотрудника
                        ConnectionHelper.db.Employee.Remove(selectedEmployee);
                        ConnectionHelper.db.SaveChanges();

                        LoadData();
                        MessageBox.Show("Сотрудник успешно удален");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}\n\nВнутреннее исключение: {ex.InnerException?.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления");
            }
        }

        private void EmployeesGrid_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            // Этот метод можно использовать, если нужно выполнить какие-то действия при выборе сотрудника
        }

        private void Discipline_Go(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DisciplinesPage());
        }

        private void Kafedra_Go(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new KafedrasPage());
        }
    }
}