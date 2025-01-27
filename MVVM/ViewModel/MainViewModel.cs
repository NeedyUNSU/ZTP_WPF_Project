using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZTP_WPF_Project.MVVM.Core;
using RelayCommand = ZTP_WPF_Project.MVVM.Core.RelayCommand;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

        public TransactionCategoryViewModel transactionCategoryVM { get; set; }
        public TransactionViewModel transactionVM { get; set; }
        public BudgetViewModel budgetVM { get; set; }
        public ReportViewModel reportVM { get; set; }


        private object _currentView;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowTransactionPage { get; }
        public ICommand ShowBudgetPage { get; }
        public ICommand ShowReportPage { get; }

        public MainViewModel()
        {
            transactionCategoryVM = new TransactionCategoryViewModel();
            transactionVM = new TransactionViewModel(transactionCategoryVM);
            budgetVM = new BudgetViewModel(transactionVM, transactionCategoryVM);
            reportVM = new ReportViewModel();

            ShowTransactionPage = new RelayCommand(_ => OpenTransactionPage());
            ShowBudgetPage = new RelayCommand(_ => OpenBudgetPage());
            ShowReportPage = new RelayCommand(_ => OpenReportPage());

            CurrentView = transactionVM;
        }

        private void OpenTransactionPage()
        {
            CurrentView = transactionVM;
        }

        private void OpenReportPage()
        {
            CurrentView = reportVM;
        }

        private void OpenBudgetPage()
        {
            CurrentView = budgetVM;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
