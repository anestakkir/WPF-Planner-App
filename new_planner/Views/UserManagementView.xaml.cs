using new_planner.ViewModels;
using System.Windows;

namespace new_planner.Views
{
    public partial class UserManagementView : Window
    {
        public UserManagementView()
        {
            InitializeComponent();
            DataContext = new UserManagementViewModel();
        }
    }
}