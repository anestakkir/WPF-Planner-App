using new_planner.ViewModels;
using System.Windows;

namespace new_planner.Views
{
   
    public partial class RegistrationView : Window
    {
        public RegistrationView()
        {
            InitializeComponent();

            var viewModel = new RegistrationViewModel();
            DataContext = viewModel;
            viewModel.OnRegistrationSuccess += () => { this.Close(); };
        }
    }
}
