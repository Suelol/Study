using Study_Kamalov_wpf_320P.Connection;
using Study_Kamalov_wpf_320P.DbModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System;
using Study_Kamalov_wpf_320P.DbModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Study_Kamalov_wpf_320P.Pages
{
    public partial class EditKafedraPage : Page
    {
        private Kafedra _currentKafedra;

        public EditKafedraPage(Kafedra kafedra)
        {
            InitializeComponent();
            _currentKafedra = kafedra;
            LoadFaculties();
            LoadKafedraData();
        }

        private void LoadFaculties()
        {
            var faculties = ConnectionHelper.db.Faculty
                .OrderBy(f => f.Name_faculty)
                .ToList();
            FacultyComboBox.ItemsSource = faculties;
        }

        private void LoadKafedraData()
        {
            ShifrBox.Text = _currentKafedra.Shifr;
            NameBox.Text = _currentKafedra.Name_kaf;

            if (!string.IsNullOrEmpty(_currentKafedra.Facultet))
            {
                var faculty = ConnectionHelper.db.Faculty.FirstOrDefault(f => f.Abbriveatura_faculty == _currentKafedra.Facultet);
                FacultyComboBox.SelectedItem = faculty;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(ShifrBox.Text) ||
                    string.IsNullOrWhiteSpace(NameBox.Text) ||
                    FacultyComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля");
                    return;
                }

                // Обновление данных кафедры
                _currentKafedra.Shifr = ShifrBox.Text.Trim();
                _currentKafedra.Name_kaf = NameBox.Text.Trim();
                _currentKafedra.Facultet = (FacultyComboBox.SelectedItem as Faculty)?.Abbriveatura_faculty;

                // Сохранение изменений в базе данных
                ConnectionHelper.db.SaveChanges();

                MessageBox.Show("Кафедра успешно обновлена");
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