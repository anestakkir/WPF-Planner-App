using new_planner.Models;
using new_planner.ViewModels;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Controls;

namespace new_planner.Views
{
    public partial class EventWindow : Window
    {
        // Конструктор для НОВОГО дела
        public EventWindow()
        {
            InitializeComponent();
            var viewModel = new EventViewModel();
            DataContext = viewModel;
            viewModel.OnSaveSuccess += () => { this.Close(); };
        }

        // Конструктор для РЕДАКТИРОВАНИЯ
        public EventWindow(Event eventToEdit)
        {
            InitializeComponent();
            var viewModel = new EventViewModel(eventToEdit); // Передаем дело в ViewModel
            DataContext = viewModel;
            viewModel.OnSaveSuccess += () => { this.Close(); };
        }

        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Создаем регулярное выражение, которое ищет "не цифры"
            Regex regex = new Regex("[^0-9]+");

            // e.Text - это символ, который пользователь пытается ввести.
            // Если этот символ НЕ является цифрой, то...
            e.Handled = regex.IsMatch(e.Text);

            // e.Handled = true; означает "отменить этот ввод, не показывать его".
        }

        private void TimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Преобразуем "отправителя" в TextBox, чтобы получить доступ к его тексту
            if (sender is not TextBox textBox) return;

            // Если поле пустое, ничего не делаем
            if (string.IsNullOrWhiteSpace(textBox.Text)) return;

            // Пытаемся преобразовать текст в число
            if (int.TryParse(textBox.Text, out int value))
            {
                // Находим имя TextBox'а, чтобы понять, это часы или минуты
                string name = textBox.Name;
                // !!! ВАЖНО: Нам нужно дать имена нашим TextBox'ам в XAML

                // Проверяем часы
                if (name.Contains("Hours"))
                {
                    if (value < 0 || value > 23)
                    {
                        MessageBox.Show("Значение часов должно быть в диапазоне от 0 до 23.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                        textBox.Text = ""; // Очищаем некорректное значение
                    }
                }
                // Проверяем минуты
                else if (name.Contains("Minutes"))
                {
                    if (value < 0 || value > 59)
                    {
                        MessageBox.Show("Значение минут должно быть в диапазоне от 0 до 59.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                        textBox.Text = ""; // Очищаем некорректное значение
                    }
                }
            }
        }
    }
}