// Файл: Views/MainWindow.xaml.cs
using new_planner.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace new_planner.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            this.Loaded += MainWindow_Loaded;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            NotificationService.CheckEvents(SessionContext.CurrentUser!.UserId);
            _ = _viewModel.LoadEventsAsync();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadDataAsync();
            // Передаем ID текущего пользователя
            NotificationService.CheckEvents(SessionContext.CurrentUser!.UserId);
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.EditEventCommand.CanExecute(null))
            {
                _viewModel.EditEventCommand.Execute(null);
            }
        }
    }
}