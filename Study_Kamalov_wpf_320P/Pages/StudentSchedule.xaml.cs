using Study_Kamalov_wpf_320P.Connection;
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
    /// Логика взаимодействия для StudentSchedule.xaml
    /// </summary>
    public partial class StudentSchedule : Page
    {
        public StudentSchedule()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            // Загружаем данные из БД и сортируем по названию
            var disciplines = ConnectionHelper.db.Discipline
                .OrderBy(d => d.Name_discip)
                .ToList();

            DisciplinesGrid.ItemsSource = disciplines;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
