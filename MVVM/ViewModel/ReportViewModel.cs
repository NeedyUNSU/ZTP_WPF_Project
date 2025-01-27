using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ZTP_WPF_Project.MVVM.Model;
using ZTP_WPF_Project.MVVM.Builder;
using ZTP_WPF_Project.MVVM.Strategy;
using ZTP_WPF_Project.MVVM.Core;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class ReportViewModel : ReportModel, INotifyPropertyChanged
    {
        public ICommand CreatePDF { get; }
        private bool _isYearlyReport;
        private bool _isMonthlyReport;
        private string _reportDateRange;
        private readonly TransactionViewModel _transactionViewModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsYearlyReport
        {
            get => _isYearlyReport;
            set
            {
                if (_isYearlyReport != value)
                {
                    _isYearlyReport = value;
                    OnPropertyChanged();
                    UpdateReportDateRange();
                }
            }
        }

        public bool IsMonthlyReport
        {
            get => _isMonthlyReport;
            set
            {
                if (_isMonthlyReport != value)
                {
                    _isMonthlyReport = value;
                    OnPropertyChanged();
                    UpdateReportDateRange();
                }
            }
        }

        public string ReportDateRange
        {
            get => _reportDateRange;
            private set
            {
                if (_reportDateRange != value)
                {
                    _reportDateRange = value;
                    OnPropertyChanged();
                }
            }
        }

        public ReportViewModel(TransactionViewModel transactionViewModel)
        {
            _transactionViewModel = transactionViewModel;// ?? throw new ArgumentNullException(nameof(transactionViewModel));
            IsMonthlyReport = true;
            UpdateReportDateRange();
            CreatePDF = new RelayCommand(
                execute: _ => GenerateReport(),
                canExecute: _ => _transactionViewModel?.GetAll()?.Any() == true
            );
        }

        private void UpdateReportDateRange()
        {
            DateTime today = DateTime.Today;
            DateTime startDate = IsYearlyReport ? today.AddYears(-1) : today.AddMonths(-1);
            ReportDateRange = $"Od {startDate:dd.MM.yyyy} do {today:dd.MM.yyyy}";
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void GenerateReport()
        {
            try
            {
                if (_transactionViewModel == null)
                {
                    MessageBox.Show("Brak danych transakcji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Pobieranie listy transakcji
                var transactions = _transactionViewModel.GetAll()
                    .Where(t => t.AddedDate >= StartDate && t.AddedDate <= EndDate)
                    .ToList();

                // Obliczenia
                var incomeSum = transactions.Where(t => t._Type == TransactionType.Income).Sum(t => t.Amount);
                var expenseSum = transactions.Where(t => t._Type == TransactionType.Expense).Sum(t => t.Amount);
                var balance = incomeSum - expenseSum;
                var maxIncome = transactions.Where(t => t._Type == TransactionType.Income).DefaultIfEmpty(new TransactionModel()).Max(t => t.Amount);
                var maxExpense = transactions.Where(t => t._Type == TransactionType.Expense).DefaultIfEmpty(new TransactionModel()).Max(t => t.Amount);

                // Tworzenie raportu niezależnie od tego, czy są transakcje, czy nie
                var report = new ReportModel
                {
                    Title = IsMonthlyReport ? "Raport miesięczny" : "Raport roczny",
                    Description = transactions.Any() ? "Raport z transakcji." : "Brak transakcji w wybranym okresie.",
                    CreatedDate = DateTime.Now,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Transactions = transactions // Pusta lista transakcji też zostanie uwzględniona
                };

                // Generowanie raportu
                IReportingStrategy reportingStrategy = IsMonthlyReport ? new ReportingMonth() : new ReportingYear();
                var directory = new ReportingDirectory(reportingStrategy);
                directory.ExecuteReportGeneration(report);

                var builder = new ReportingBuilder();
                builder.BuildPDF(report);

                // Komunikat o sukcesie
                MessageBox.Show("Raport PDF został wygenerowany pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                // Zamknięcie aplikacji (można to usunąć, jeśli nie chcesz zamykać aplikacji po wygenerowaniu raportu)
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas generowania raportu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}