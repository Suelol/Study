using Study_Kamalov_wpf_320P.Connection;
using Study_Kamalov_wpf_320P.DbModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Study_Kamalov_wpf_320P.Pages
{
    public partial class AddKafedraPage : Page
    {
        public AddKafedraPage()
        {
            InitializeComponent();
            LoadFaculties();
        }

        private void LoadFaculties()
        {
            var faculties = ConnectionHelper.db.Faculty
                .OrderBy(f => f.Name_faculty)
                .ToList();
            FacultyComboBox.ItemsSource = faculties;
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

                // Создание новой кафедры
                var newKafedra = new Kafedra
                {
                    Shifr = ShifrBox.Text.Trim(),
                    Name_kaf = NameBox.Text.Trim(),
                    Facultet = (FacultyComboBox.SelectedItem as Faculty)?.Abbriveatura_faculty
                };

                ConnectionHelper.db.Kafedra.Add(newKafedra);
                ConnectionHelper.db.SaveChanges();

                MessageBox.Show("Кафедра успешно добавлена");
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