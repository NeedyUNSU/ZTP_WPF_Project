using System;
using QuestPDF;
using QuestPDF.Fluent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Model;
using System.Transactions;
using System.Data.Common;
using Microsoft.Win32;
using System.Windows;
using ZTP_WPF_Project.MVVM.ViewModel;

namespace ZTP_WPF_Project.MVVM.Strategy
{
    public class ReportingMonth : IReportingStrategy
    {
        public void GenerateReport(ReportModel report)
        {

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Report cannot be null.");
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = $"{report.Title}",
                Title = "Save Report as",
                Filter = "PDF file (*.pdf)|*.pdf;",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.Content()
                            .Column(col =>
                            {
                                col.Item().Text("Raport finansowy miesięczny")
                                    .Bold()
                                    .FontSize(24)
                                    .AlignCenter();
                                DateTime today = DateTime.Today;
                                report.StartDate = today.AddMonths(-1);
                                col.Item().Text($"Okres: {report.StartDate:yyyy.MM.dd} - {report.EndDate:yyyy.MM.dd}")
                                    .FontSize(14)
                                    .AlignCenter();

                                col.Item().Height(20);
                                var incomeSum = report.Transactions.Where(t => t._Type == TransactionType.Income).Sum(t => t.Amount);
                                var expenseSum = report.Transactions.Where(t => t._Type == TransactionType.Expense).Sum(t => t.Amount);
                                var balance = incomeSum - expenseSum;
                                var maxIncome = report.Transactions.Where(t => t._Type == TransactionType.Income).DefaultIfEmpty(new TransactionModel()).Max(t => t.Amount);
                                var maxExpense = report.Transactions.Where(t => t._Type == TransactionType.Expense).DefaultIfEmpty(new TransactionModel()).Max(t => t.Amount);
                                var incomeCount = report.Transactions.Count(t => t._Type == TransactionType.Income);
                                var expenseCount = report.Transactions.Count(t => t._Type == TransactionType.Expense);
                                
                                col.Item().Text($"Całkowity przychód: {incomeSum:C}");
                                col.Item().Text($"Całkowity wydatek: {expenseSum:C}");
                                col.Item().Text($"Bilans: {balance:C}");
                                col.Item().Text($"Maksymalny przychód: {maxIncome:C}");
                                col.Item().Text($"Maksymalny wydatek: {maxExpense:C}");
                                col.Item().Text($"Liczba przychodów: {incomeCount}");
                                col.Item().Text($"Liczba wydatków: {expenseCount}");

                                foreach (var transaction in report.Transactions)
                                {
                                    string transactionLine = transaction._Type == TransactionType.Income
                                        ? $"- {transaction.Title}: +{transaction.Amount}"
                                        : $"- {transaction.Title}: -{transaction.Amount}";

                                    col.Item().Text(transactionLine);
                                }
                            });
                    });
                }).GeneratePdf($"{filePath}");
                MessageBox.Show("Raport PDF został wygenerowany pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Saving pdf file failed.");
            }
        }
    }
}
