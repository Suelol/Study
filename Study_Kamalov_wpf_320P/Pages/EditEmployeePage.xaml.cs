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
    /// Логика взаимодействия для EditEmployeePage.xaml
    /// </summary>
    public partial class EditEmployeePage : Page
    {
        private Employee _currentEmployee;

        public EditEmployeePage(Employee employee)
        {
            InitializeComponent();
            _currentEmployee = employee;
            LoadKafedras();
            LoadEmployeeData();
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

        private void LoadEmployeeData()
        {
            TabNumberBox.Text = _currentEmployee.Tab_number.ToString();
            SurnameBox.Text = _currentEmployee.Surname;
            DoljnostBox.Text = _currentEmployee.Doljnost;
            ZarplataBox.Text = _currentEmployee.Zarplata?.ToString();
            ShefBox.Text = _currentEmployee.Shef?.ToString();

            // Выбираем кафедру в комбобоксе
            if (!string.IsNullOrEmpty(_currentEmployee.Shifr))
            {
                var kafedra = ConnectionHelper.db.Kafedra.FirstOrDefault(k => k.Shifr == _currentEmployee.Shifr);
                KafedraComboBox.SelectedItem = kafedra;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(SurnameBox.Text) ||
                    string.IsNullOrWhiteSpace(DoljnostBox.Text) ||
                    KafedraComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля");
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

                // Обновление данных сотрудника
                _currentEmployee.Surname = SurnameBox.Text.Trim();
                _currentEmployee.Doljnost = DoljnostBox.Text.Trim();
                _currentEmployee.Zarplata = zarplata;
                _currentEmployee.Shifr = (KafedraComboBox.SelectedItem as Kafedra)?.Shifr;
                _currentEmployee.Shef = shef;

                // Сохранение изменений в базе данных
                ConnectionHelper.db.SaveChanges();

                MessageBox.Show("Данные сотрудника успешно обновлены");
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
