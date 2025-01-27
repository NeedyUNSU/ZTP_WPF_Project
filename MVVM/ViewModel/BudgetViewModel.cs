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
using System.Windows.Documents;

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
            set { percentOfExpensesAndIncome = value; OnPropertyChanged(); }
        }

        private string leftToSpend;

        public string LeftToSpend
        {
            get { return leftToSpend; }
            set { leftToSpend = value; OnPropertyChanged(); }
        }

        private string overALimit;

        public string OverALimit
        {
            get { return overALimit; }
            set { overALimit = value; OnPropertyChanged(); }
        }

        private string countOfTransactions;

        public string CountOfTransactions
        {
            get { return countOfTransactions; }
            set { countOfTransactions = value; OnPropertyChanged(); }
        }

        private string avgExpenses;

        public string AVGExpenses
        {
            get { return avgExpenses; }
            set { avgExpenses = value; OnPropertyChanged(); }
        }

        private string avgIncome;

        public string AVGIncome
        {
            get { return avgIncome; }
            set { avgIncome = value; OnPropertyChanged(); }
        }


        private float sumIncome;

        public float SumIncome
        {
            get
            {
                return sumIncome;
            }
            set
            {
                sumIncome = value;
                OnPropertyChanged(nameof(SumOfExpenses));
                OnPropertyChanged(nameof(SumOfIncome));
                OnPropertyChanged(nameof(PercentOfExpensesAndIncome));
                OnPropertyChanged(nameof(LeftToSpend));
                OnPropertyChanged(nameof(OverALimit));
                OnPropertyChanged(nameof(CountOfTransactions));
                OnPropertyChanged(nameof(AVGExpenses));
                OnPropertyChanged(nameof(AVGIncome));
            }
        }

        private float sumExpenses;

        public float SumExpenses
        {
            get
            {
                return sumExpenses;
            }
            set
            {
                sumExpenses = value;
                OnPropertyChanged(nameof(SumOfExpenses));
                OnPropertyChanged(nameof(SumOfIncome));
                OnPropertyChanged(nameof(PercentOfExpensesAndIncome));
                OnPropertyChanged(nameof(LeftToSpend));
                OnPropertyChanged(nameof(OverALimit));
                OnPropertyChanged(nameof(CountOfTransactions));
                OnPropertyChanged(nameof(AVGExpenses));
                OnPropertyChanged(nameof(AVGIncome));
            }
        }

        private int incomeCount;

        public int IncomeCount
        {
            get { return incomeCount; }
            set 
            {
                incomeCount = value; 
                OnPropertyChanged(); 
            }
        }

        private int expensesCount;

        public int ExpensesCount
        {
            get { return expensesCount; }
            set 
            { 
                expensesCount = value; 
                OnPropertyChanged(); 
            }
        }



        public ICommand GoToMenuCommand { get; }

        public BudgetViewModel(TransactionViewModel transactionVM, TransactionCategoryViewModel categoryVM)
        {
            this._categoryVM = categoryVM;
            this._transactionVM = transactionVM;

            RefreshGui();

            GoToMenuCommand = new RelayCommand(_ => { MainContext.ShowTransactionPage.Execute(this); });
        }

        private void RefreshGui()
        {
            var list = _transactionVM.GetAll();


            if (list != null)
            {
                TransactionsCache = new();
                foreach (var item in list)
                {
                    if (item != null)
                        TransactionsCache.Add(new TransactionProxy(item));
                }
            }

            SumExpenses = list.Where(_ => _._Type == TransactionType.Expense).Sum(o => o.Amount);
            SumIncome = list.Where(_ => _._Type == TransactionType.Income).Sum(o => o.Amount);
            ExpensesCount = list.Where(_ => _._Type == TransactionType.Expense).Count();
            IncomeCount = list.Where(_ => _._Type == TransactionType.Income).Count();

            SumOfExpenses = $"${SumExpenses}";
            SumOfIncome = $"${SumIncome}";
            PercentOfExpensesAndIncome = $"{(SumExpenses / SumIncome * 100)}%";
            LeftToSpend = $"${SumIncome - SumExpenses}";
            OverALimit = (SumIncome - SumExpenses < 0 ? $"${SumIncome - SumExpenses}" : "None").ToString();
            CountOfTransactions = $"{ExpensesCount + IncomeCount}";
            AVGExpenses = $"${SumExpenses / ExpensesCount}";
            AVGIncome = $"${SumIncome/IncomeCount}";
        }

        public List<TransactionModel> GetAllBudgetTransactions()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (budgetTransactions.Count > 0 ? budgetTransactions : new());
        }

        public float GetValues()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (budgetTransactions.Count > 0 ? budgetTransactions.Sum(o => o.Amount) : 0.0f);
        }

        public uint GetCount()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (uint)budgetTransactions.Count;
        }
    }
}
