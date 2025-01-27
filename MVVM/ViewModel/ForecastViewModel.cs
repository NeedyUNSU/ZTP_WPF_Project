using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZTP_WPF_Project.MVVM.Model;
using RelayCommand = ZTP_WPF_Project.MVVM.Core.RelayCommand;
using System.Windows;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class ForecastViewModel : BaseViewModel
    {
        private readonly TransactionViewModel transactionVM;

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

        public ICommand GoToMenuCommand { get; }

        private float totalIncome;
        private float totalExpenses;
        private float balance;
        private float predictedExpensesNextMonth;
        private Dictionary<string, float> expensesByCategory;
        private Dictionary<string, float> averageExpensesByCategory;
        private Dictionary<string, float> predictedExpensesByCategoryNextYear;
        private Dictionary<string, float> monthlyIncomes;
        private Dictionary<string, float> monthlyExpenses;
        private string mostCommonCategory;

        public float TotalIncome
        {
            get { return totalIncome; }
            set { totalIncome = value; OnPropertyChanged(); }
        }

        public float TotalExpenses
        {
            get { return totalExpenses; }
            set { totalExpenses = value; OnPropertyChanged(); }
        }

        public float Balance
        {
            get { return balance; }
            set { balance = value; OnPropertyChanged(); }
        }

        public float PredictedExpensesNextMonth
        {
            get { return predictedExpensesNextMonth; }
            set { predictedExpensesNextMonth = value; OnPropertyChanged(); }
        }

        public Dictionary<string, float> ExpensesByCategory
        {
            get { return expensesByCategory; }
            set { expensesByCategory = value; OnPropertyChanged(); }
        }

        public Dictionary<string, float> AverageExpensesByCategory
        {
            get { return averageExpensesByCategory; }
            set { averageExpensesByCategory = value; OnPropertyChanged(); }
        }

        public Dictionary<string, float> PredictedExpensesByCategoryNextYear
        {
            get { return predictedExpensesByCategoryNextYear; }
            set { predictedExpensesByCategoryNextYear = value; OnPropertyChanged(); }
        }

        public Dictionary<string, float> MonthlyIncomes
        {
            get { return monthlyIncomes; }
            set { monthlyIncomes = value; OnPropertyChanged(); }
        }

        public Dictionary<string, float> MonthlyExpenses
        {
            get { return monthlyExpenses; }
            set { monthlyExpenses = value; OnPropertyChanged(); }
        }

        public string MostCommonCategory
        {
            get { return mostCommonCategory; }
            set { mostCommonCategory = value; OnPropertyChanged(); }
        }


        public ForecastViewModel(TransactionViewModel transactionVM)
        {
            this.transactionVM = transactionVM ?? throw new ArgumentNullException(nameof(transactionVM));

            Load(this.transactionVM.GetAll());
            GoToMenuCommand = new RelayCommand(_ => { MainContext.ShowTransactionPage.Execute(this); });
        }

        public void Load(List<TransactionModel> transactions)
        {
            MonthlyExpenses = new Dictionary<string, float>();
            MonthlyIncomes = new Dictionary<string, float>();
            ExpensesByCategory = new Dictionary<string, float>();
            AverageExpensesByCategory = new Dictionary<string, float>();
            PredictedExpensesByCategoryNextYear = new Dictionary<string, float>();

            CalculateStatistics(transactions);
        }

        private void CalculateStatistics(List<TransactionModel> transactions)
        {
            var groupedTransactions = transactions.GroupBy(t => new { t.AddedDate.Year, t.AddedDate.Month });

            foreach (var group in groupedTransactions)
            {
                string monthKey = $"{group.Key.Year}-{group.Key.Month.ToString("D2")}";

                float monthlyExpenses = group.Where(t => t._Type == TransactionType.Expense).Sum(t => t.Amount);
                float monthlyIncomes = group.Where(t => t._Type == TransactionType.Income).Sum(t => t.Amount);

                MonthlyExpenses[monthKey] = monthlyExpenses;
                MonthlyIncomes[monthKey] = monthlyIncomes;
            }

            TotalIncome = transactions.Where(t => t._Type == TransactionType.Income).Sum(t => t.Amount);
            TotalExpenses = transactions.Where(t => t._Type == TransactionType.Expense).Sum(t => t.Amount);

            Balance = TotalIncome - TotalExpenses;

            PredictedExpensesNextMonth = MonthlyExpenses.Values.Any()
                ? MonthlyExpenses.Values.Average()
                : 0;

            var expensesByCategory = transactions
                .Where(t => t._Type == TransactionType.Expense && t._category != null)
                .GroupBy(t => t.CategoryName);

            foreach (var categoryGroup in expensesByCategory)
            {
                string categoryName = categoryGroup.Key ?? "Brak kategorii";
                float totalCategoryExpenses = categoryGroup.Sum(t => t.Amount);
                float averageCategoryExpenses = categoryGroup.Average(t => t.Amount);

                ExpensesByCategory[categoryName] = totalCategoryExpenses;
                AverageExpensesByCategory[categoryName] = averageCategoryExpenses;

                PredictedExpensesByCategoryNextYear[categoryName] = averageCategoryExpenses * 12;
            }

            MostCommonCategory = expensesByCategory
                .OrderByDescending(g => g.Count())
                .FirstOrDefault()?.Key ?? "Brak danych";
        }
    }
}
