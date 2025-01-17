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
    /// Logika interakcji dla klasy ReportView.xaml
    /// </summary>
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();

            //this.DataContext = new ReportViewModel();
        }
    }
}
