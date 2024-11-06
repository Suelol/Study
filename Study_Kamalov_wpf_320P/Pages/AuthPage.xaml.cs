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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void ToQrCodePage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new QR_Kod());
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка авторизации");
                return;
            }

            try
            {
                var user = ConnectionHelper.db.Employee.FirstOrDefault(u => u.Tab_number.ToString() == password);

                if (user != null)
                {
                    MessageBox.Show($"Добро пожаловать, {user.Surname}!", "Успешная авторизация");

                    switch (user.Doljnost)
                    {
                        case "инженер":
                            NavigationService.Navigate(new EmployeesPage());
                            break;
                        case "преподаватель":
                            NavigationService.Navigate(new StudentSchedule(false));
                            break;
                        case "зав. кафедрой":
                            NavigationService.Navigate(new EmployeesPage(user.Tab_number, user.Shifr));
                            break;
                        default:
                            MessageBox.Show("Неизвестная должность", "Ошибка");
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль", "Ошибка авторизации");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
