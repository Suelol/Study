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
        private readonly int? _headId;
        private readonly string _kafedraShifr;
        private bool _isHeadOfDepartment;

        public EmployeesPage(int? headId = null, string kafedraShifr = null)
        {
            InitializeComponent();
            _headId = headId;
            _kafedraShifr = kafedraShifr;
            _isHeadOfDepartment = headId.HasValue;

            SetButtonsVisibility();
            LoadData();
        }

        private void SetButtonsVisibility()
        {
            // Если это не заведующий кафедрой, скрываем кнопки редактирования
            if (!_isHeadOfDepartment)
            {
                AddButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadData()
        {
            try
            {
                IQueryable<Employee> query = ConnectionHelper.db.Employee;

                // Если это заведующий кафедрой, показываем только сотрудников его кафедры
                if (_isHeadOfDepartment && !string.IsNullOrEmpty(_kafedraShifr))
                {
                    query = query.Where(e => e.Shifr == _kafedraShifr);
                }

                var employees = query.OrderBy(e => e.Surname).ToList();
                EmployeesGrid.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs args) // Изменил e на args
        {
            string searchText = SearchBox.Text.ToLower();

            IQueryable<Employee> query = ConnectionHelper.db.Employee;

            // Применяем фильтр по кафедре, если это заведующий
            if (_isHeadOfDepartment && !string.IsNullOrEmpty(_kafedraShifr))
            {
                query = query.Where(e => e.Shifr == _kafedraShifr);
            }

            var filtered = query
                .Where(emp => emp.Surname.ToLower().Contains(searchText)) // Изменил e на emp
                .OrderBy(emp => emp.Surname) // Изменил e на emp для консистентности
                .ToList();

            EmployeesGrid.ItemsSource = filtered;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeesGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления");
                return;
            }

            // Проверяем, принадлежит ли сотрудник к кафедре заведующего
            if (_isHeadOfDepartment && selectedEmployee.Shifr != _kafedraShifr)
            {
                MessageBox.Show("Вы можете удалять только сотрудников своей кафедры");
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {selectedEmployee.Surname}?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаление связанных записей...
                    var teacher = ConnectionHelper.db.Teacher.FirstOrDefault(t => t.Tab_number == selectedEmployee.Tab_number);
                    if (teacher != null)
                        ConnectionHelper.db.Teacher.Remove(teacher);

                    var engineer = ConnectionHelper.db.Enginer.FirstOrDefault(eng => eng.Tab_number == selectedEmployee.Tab_number);
                    if (engineer != null)
                        ConnectionHelper.db.Enginer.Remove(engineer);

                    var zavKaf = ConnectionHelper.db.Zav_kaf.FirstOrDefault(z => z.Tab_number == selectedEmployee.Tab_number);
                    if (zavKaf != null)
                        ConnectionHelper.db.Zav_kaf.Remove(zavKaf);

                    var exams = ConnectionHelper.db.Exsamen.Where(ex => ex.Tab_number == selectedEmployee.Tab_number).ToList();
                    foreach (var exam in exams)
                    {
                        ConnectionHelper.db.Exsamen.Remove(exam);
                    }

                    ConnectionHelper.db.Employee.Remove(selectedEmployee);
                    ConnectionHelper.db.SaveChanges();

                    LoadData();
                    MessageBox.Show("Сотрудник успешно удален");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = EmployeesGrid.SelectedItem as Employee;
            if (selectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования");
                return;
            }

            // Проверяем, принадлежит ли сотрудник к кафедре заведующего
            if (_isHeadOfDepartment && selectedEmployee.Shifr != _kafedraShifr)
            {
                MessageBox.Show("Вы можете редактировать только сотрудников своей кафедры");
                return;
            }

            // Здесь код для редактирования сотрудника
            MessageBox.Show($"Редактирование сотрудника: {selectedEmployee.Surname}");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isHeadOfDepartment)
            {
                // Здесь код для добавления нового сотрудника
                // При добавлении нужно автоматически установить Shifr равным _ kafedraShifr
                MessageBox.Show("Добавление нового сотрудника на кафедру: " + _kafedraShifr);
            }
            else
            {
                MessageBox.Show("Вы можете добавлять сотрудников только будучи заведующим кафедрой");
            }
        }




        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
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