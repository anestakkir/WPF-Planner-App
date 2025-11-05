using new_planner.Views;
using System;
using System.Windows;

namespace new_planner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Теперь App делает только ОДНУ вещь:
            // создает и показывает окно входа.
            // Вся дальнейшая логика находится внутри самого LoginView.
            var loginView = new LoginView();
            loginView.Show();
        }
    }

}
