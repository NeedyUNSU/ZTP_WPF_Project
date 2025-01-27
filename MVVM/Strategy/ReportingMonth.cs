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

namespace ZTP_WPF_Project.MVVM.Strategy
{
    public class ReportingMonth : IReportingStrategy
    {
        public void GenerateReport(ReportModel report)
        {

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            // Sprawdzenie, czy obiekt `report` nie jest pusty
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Report cannot be null.");
            }

            // Generowanie PDF
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Content()
                        .Column(col =>
                        {
                            // Tytuł raportu
                            col.Item().Text("Raport finansowy miesięczny")
                                .Bold()
                                .FontSize(24)
                                .AlignCenter();
                            DateTime today = DateTime.Today;
                            report.StartDate = today.AddMonths(-1);
                            // Okres raportu
                            col.Item().Text($"Okres: {report.StartDate:yyyy.MM.dd} - {report.EndDate:yyyy.MM.dd}")
                                .FontSize(14)
                                .AlignCenter();

                            // Przerwa między sekcjami
                            col.Item().Height(20);
                            var incomeSum = report.Transactions.Where(t => t._Type == TransactionType.Income).Sum(t => t.Amount);
                            var expenseSum = report.Transactions.Where(t => t._Type == TransactionType.Expense).Sum(t => t.Amount);
                            var balance = incomeSum - expenseSum;
                            var maxIncome = report.Transactions.Where(t => t._Type == TransactionType.Income).DefaultIfEmpty(new TransactionModel()).Max(t => t.Amount);
                            var maxExpense = report.Transactions.Where(t => t._Type == TransactionType.Expense).DefaultIfEmpty(new TransactionModel()).Max(t => t.Amount);
                            var incomeCount = report.Transactions.Count(t => t._Type == TransactionType.Income);
                            var expenseCount = report.Transactions.Count(t => t._Type == TransactionType.Expense);
                            // Dane finansowe
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
            }).GeneratePdf($"{report.Title}.pdf");

            // Logowanie do konsoli
            Console.WriteLine($"Generowanie raportu miesięcznego zakończone: {report.Title}");
        }



        // Logika generowania raportu miesięcznego
       // Console.WriteLine($"Generowanie raportu miesięcznego: {report.Title}");
            // Implementacja generowania PDF, dodanie danych itp.
        
    }
}
