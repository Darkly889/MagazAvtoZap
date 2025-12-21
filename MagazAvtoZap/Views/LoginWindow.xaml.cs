using System.Windows;
using System.Windows.Controls;
using MagazAvtoZap.Models;

namespace MagazAvtoZap.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ShowPasswordTextBox.Text = PasswordBox.Password;
            ShowPasswordTextBox.Visibility = Visibility.Visible;
            PasswordBox.Visibility = Visibility.Collapsed;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = ShowPasswordTextBox.Text;
            ShowPasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
        }

        private void ShowPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBox.Password = ShowPasswordTextBox.Text;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Логин и пароль обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User user = AuthenticateUser(login, password);

            if (user != null)
            {
                MainWindow mainWindow = new MainWindow(user.IsAdmin);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private User AuthenticateUser(string login, string password)
        {
            
            if (login == "AdminLeo" && password == "Admin12345")
            {
                return new User { Login = login, Password = password, IsAdmin = true };
            }
            
            else if (login == "Shaiko@gmail.com" && password == "12345678")
            {
                return new User { Login = login, Password = password, IsAdmin = false };
            }
            else
            {
                return null;
            }
        }
    }
}
