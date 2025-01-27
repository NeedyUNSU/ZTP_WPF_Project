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
using ZTP_WPF_Project.MVVM.ViewModel;

namespace ZTP_WPF_Project.MVVM.View
{
    /// <summary>
    /// Interaction logic for TransactionsView.xaml
    /// </summary>
    public partial class TransactionsView : UserControl
    {
        public TransactionsView()
        {
            InitializeComponent();
        }

        private MainViewModel MainContext
        {
            get
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null && mainWindow.DataContext is MainViewModel)
                {
                    return mainWindow.DataContext as MainViewModel;
                }
                else
                {
                    throw new InvalidOperationException("Main data context must be MainViewModel");
                }
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (!MainContext.UserVM.CurrentUserIsModerator()) return;
            //if (sender is DataGrid dataGrid && dataGrid.SelectedItem != null)
            //{
            //MainContext.CurrentView = null; //OtherVM.EditSelectedSubPageCommand.Execute(this);
            //}
        }
    }
}
