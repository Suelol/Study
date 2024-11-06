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
    /// Логика взаимодействия для AddDisciplinePage.xaml
    /// </summary>
    public partial class AddDisciplinePage : Page
    {
        public AddDisciplinePage()
        {
            InitializeComponent();
            LoadKafedras();
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

                // Создание новой дисциплины
                var newDiscipline = new Discipline
                {
                    Name_discip = NameBox.Text.Trim(),
                    Obem = obem,
                    Shifr = (KafedraComboBox.SelectedItem as Kafedra)?.Shifr
                };

                // Сохранение в базе данных
                ConnectionHelper.db.Discipline.Add(newDiscipline);
                ConnectionHelper.db.SaveChanges();

                MessageBox.Show("Дисциплина успешно добавлена");
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
