using Study_Kamalov_wpf_320P.Connection;
using Study_Kamalov_wpf_320P.DbModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Study_Kamalov_wpf_320P.Pages
{
    public partial class EditDisciplinePage : Page
    {
        private Discipline _currentDiscipline;

        public EditDisciplinePage(Discipline discipline)
        {
            InitializeComponent();
            _currentDiscipline = discipline;
            LoadKafedras();
            LoadDisciplineData();
        }

        private void LoadKafedras()
        {
            try
            {
                var kafedras = ConnectionHelper.db.Kafedra
                    .OrderBy(k => k.Name_kaf)
                    .ToList();
                KafedraComboBox.ItemsSource = kafedras;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке кафедр: {ex.Message}");
            }
        }

        private void LoadDisciplineData()
        {
            KodBox.Text = _currentDiscipline.Kod.ToString();
            NameBox.Text = _currentDiscipline.Name_discip;
            ObemBox.Text = _currentDiscipline.Obem?.ToString();

            if (!string.IsNullOrEmpty(_currentDiscipline.Shifr))
            {
                var kafedra = ConnectionHelper.db.Kafedra.FirstOrDefault(k => k.Shifr == _currentDiscipline.Shifr);
                KafedraComboBox.SelectedItem = kafedra;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                    string.IsNullOrWhiteSpace(ObemBox.Text) ||
                    KafedraComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля");
                    return;
                }

                // Проверка корректности объема
                if (!int.TryParse(ObemBox.Text, out int obem))
                {
                    MessageBox.Show("Объем должен быть числом");
                    return;
                }

                // Обновление данных дисциплины
                _currentDiscipline.Name_discip = NameBox.Text.Trim();
                _currentDiscipline.Obem = obem;
                _currentDiscipline.Shifr = (KafedraComboBox.SelectedItem as Kafedra)?.Shifr;

                // Сохранение изменений в базе данных
                ConnectionHelper.db.SaveChanges();

                MessageBox.Show("Дисциплина успешно обновлена");
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