using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Model;
using ZTP_WPF_Project.MVVM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows; // application.Current.mainwindow etc.
using RelayCommand = ZTP_WPF_Project.MVVM.Core.RelayCommand;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class BudgetViewModel : BaseViewModel
    {
        protected readonly TransactionCategoryViewModel _categoryVM;
        protected readonly TransactionViewModel _transactionVM;

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
                    throw new InvalidOperationException("Main data context must be MainWindowViewModel");
                }
            }
        }

        private ObservableCollection<TransactionProxy> transactionsCache = new();

        public ObservableCollection<TransactionProxy> TransactionsCache
        {
            get { return transactionsCache; }
            set { transactionsCache = value; OnPropertyChanged(); }
        }

        private string sumOfExpenses;

        public string SumOfExpenses
        {
            get { return sumOfExpenses; }
            set { sumOfExpenses = value; OnPropertyChanged(); }
        }

        private string sumOfIncome;

        public string SumOfIncome
        {
            get { return sumOfIncome; }
            set { sumOfIncome = value; OnPropertyChanged(); }
        }

        private string percentOfExpensesAndIncome;

        public string PercentOfExpensesAndIncome
        {
            get { return percentOfExpensesAndIncome; }
            set { percentOfExpensesAndIncome = value;  OnPropertyChanged(); }
        }

        private string leftToSpend;

        public string LeftToSpend
        {
            get { return leftToSpend; }
            set { leftToSpend = value;  OnPropertyChanged(); }
        }

        private string overALimit;

        public string OverALimit
        {
            get { return overALimit; }
            set { overALimit = value;  OnPropertyChanged(); }
        }





        public ICommand GoToMenuCommand { get; }

        public BudgetViewModel(TransactionViewModel transactionVM, TransactionCategoryViewModel categoryVM)
        {
            this._categoryVM = categoryVM;
            this._transactionVM = transactionVM;

            var list = _transactionVM.GetAll();

            if (list != null)
            foreach (var item in list)
            {
                if(item != null)
                TransactionsCache.Add(new TransactionProxy(item));
            }


            SumOfExpenses = "$" + list.Where(_ => _._Type == TransactionType.Expense).Sum(o => o.Amount).ToString();
            SumOfIncome = "$" + list.Where(_ => _._Type == TransactionType.Income).Sum(o => o.Amount).ToString();
            PercentOfExpensesAndIncome = (list.Where(_ => _._Type == TransactionType.Expense).Sum(o => o.Amount) / list.Where(_ => _._Type == TransactionType.Income).Sum(o => o.Amount) * 100).ToString() + "%";
            LeftToSpend = "$" + (list.Where(_ => _._Type == TransactionType.Income).Sum(o => o.Amount) - list.Where(_ => _._Type == TransactionType.Expense).Sum(o => o.Amount)).ToString();
            OverALimit = (list.Where(_ => _._Type == TransactionType.Income).Sum(o => o.Amount) - list.Where(_ => _._Type == TransactionType.Expense).Sum(o => o.Amount) < 0 ? $"${list.Where(_ => _._Type == TransactionType.Income).Sum(o => o.Amount) - list.Where(_ => _._Type == TransactionType.Expense).Sum(o => o.Amount)}" : "None").ToString();



            GoToMenuCommand = new RelayCommand(_ => { MainContext.ShowTransactionPage.Execute(this); });
        }

        public List<TransactionModel> GetAllBudgetTransactions()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (budgetTransactions.Count > 0 ? budgetTransactions : new());
        }

        public float GetValues()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (budgetTransactions.Count > 0 ? budgetTransactions.Sum(o=>o.Amount) : 0.0f);
        }

        public uint GetCount()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (uint)budgetTransactions.Count;
        }
    }
}
