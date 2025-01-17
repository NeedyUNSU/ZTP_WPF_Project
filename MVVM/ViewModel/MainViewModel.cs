using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ZTP_WPF_Project.MVVM.Core;
using RelayCommand = ZTP_WPF_Project.MVVM.Core.RelayCommand;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

        public TransactionCategoryViewModel transactionCategoryVM { get; set; }
        public TransactionViewModel transactionVM { get; set; }

        private object _currentView;
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

        public MainViewModel()
        {
            transactionCategoryVM = new TransactionCategoryViewModel();
            transactionVM = new TransactionViewModel(transactionCategoryVM);

            ShowTransactionPage = new RelayCommand(o => OpenTransactionPage());
        }

        private void OpenTransactionPage()
        {
            CurrentView = transactionVM;
        }
    }
}
