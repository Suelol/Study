using Study_Kamalov_wpf_320P.Connection;
using Study_Kamalov_wpf_320P.DbModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Study_Kamalov_wpf_320P.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEmployeePage.xaml
    /// </summary>
    public partial class AddEmployeePage : Page
    {
        public AddEmployeePage()
        {
            InitializeComponent();
            LoadKafedras();
        }

        private void LoadKafedras()
        {
            try
            {
                var kafedras = ConnectionHelper.db.Kafedra.OrderBy(k => k.Name_kaf).ToList();
                KafedraComboBox.ItemsSource = kafedras;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке кафедр: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(TabNumberBox.Text) ||
                    string.IsNullOrWhiteSpace(SurnameBox.Text) ||
                    string.IsNullOrWhiteSpace(DoljnostBox.Text) ||
                    KafedraComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля");
                    return;
                }

                // Проверка корректности табельного номера
                if (!int.TryParse(TabNumberBox.Text, out int tabNumber))
                {
                    MessageBox.Show("Табельный номер должен быть числом");
                    return;
                }

                // Проверка существования табельного номера
                if (ConnectionHelper.db.Employee.Any(emp => emp.Tab_number == tabNumber))
                {
                    MessageBox.Show("Сотрудник с таким табельным номером уже существует");
                    return;
                }

                // Проверка и преобразование зарплаты
                decimal? zarplata = null;
                if (!string.IsNullOrWhiteSpace(ZarplataBox.Text))
                {
                    if (!decimal.TryParse(ZarplataBox.Text, out decimal zp))
                    {
                        MessageBox.Show("Некорректное значение зарплаты");
                        return;
                    }
                    zarplata = zp;
                }

                // Проверка и преобразование номера начальника
                int? shef = null;
                if (!string.IsNullOrWhiteSpace(ShefBox.Text))
                {
                    if (!int.TryParse(ShefBox.Text, out int sh))
                    {
                        MessageBox.Show("Некорректный табельный номер начальника");
                        return;
                    }
                    shef = sh;
                }

                // Создание нового сотрудника
                var newEmployee = new Employee
                {
                    Tab_number = tabNumber,
                    Surname = SurnameBox.Text.Trim(),
                    Doljnost = DoljnostBox.Text.Trim(),
                    Zarplata = zarplata,
                    Shifr = (KafedraComboBox.SelectedItem as Kafedra)?.Shifr,
                    Shef = shef
                };

                // Добавление в базу данных
                ConnectionHelper.db.Employee.Add(newEmployee);
                ConnectionHelper.db.SaveChanges();

                MessageBox.Show("Сотрудник успешно добавлен");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
