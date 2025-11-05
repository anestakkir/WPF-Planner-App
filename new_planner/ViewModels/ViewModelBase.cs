using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace new_planner.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    

        protected void RaiseCanExecuteChangedForAllCommands()
        {
            // Эта строка заставляет все команды в WPF перепроверить свое состояние CanExecute
            CommandManager.InvalidateRequerySuggested();
        }

    }
}