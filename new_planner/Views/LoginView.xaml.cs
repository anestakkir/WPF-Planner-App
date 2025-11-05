using new_planner.ViewModels;
using System.Windows;

namespace new_planner.Views
{
    
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            var viewModel = new LoginViewModel();
            DataContext = viewModel;

            // =======================================================
            // НАША НОВАЯ ЛОГИКА
            // =======================================================
            // Когда ViewModel сообщает об успешном входе...
            viewModel.OnLoginSuccess += () =>
            {
                // 1. Создаем новое главное окно
                var mainWindow = new MainWindow();

                // 2. Показываем его
                mainWindow.Show();

                // 3. И только ПОСЛЕ этого закрываем текущее окно входа
                this.Close();
            };
            // =======================================================
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationView = new RegistrationView();
            // Оставляем ShowDialog для модального окна регистрации
            registrationView.ShowDialog();
        }
    }
}
