using System.Windows;
using GestionOrdenesMVVM.ViewModels;

namespace GestionOrdenesMVVM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new OrdenViewModel();
        }
    }
}