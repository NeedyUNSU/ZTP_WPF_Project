using System;
using System.IO;
using System.Text;
using ZTP_WPF_Project.MVVM.Model;

namespace ZTP_WPF_Project.MVVM.Builder
{
    public class ReportingBuilder
    {

        //public void BuildPDF(ReportModel report)
        //{
        //    // Generowanie PDF z danymi raportu
        //    var sb = new StringBuilder();
        //    sb.AppendLine($"Raport: {report.Title}");
        //    sb.AppendLine($"Opis: {report.Description}");
        //    sb.AppendLine($"Data utworzenia: {report.CreatedDate}");
        //    sb.AppendLine($"Zakres dat: {report.StartDate.ToShortDateString()} - {report.EndDate.ToShortDateString()}");
        //    sb.AppendLine("Transakcje:");
            //foreach (var transaction in report.Transactions)
            //{
            //    if (transaction._Type == TransactionType.Income)
            //        sb.AppendLine($"- {transaction.Title}: +{transaction.Amount}");
            //    if (transaction._Type == TransactionType.Expense)
            //        sb.AppendLine($"- {transaction.Title}: -{transaction.Amount}");
            //}

    //    // Zapisz do pliku PDF (lub symulacja zapisu w pliku tekstowym)
    //    string filePath = Path.Combine(Environment.CurrentDirectory, $"{report.Title}.pdf");
    //    File.WriteAllText(filePath, sb.ToString());
    //    Console.WriteLine($"Raport zapisany do pliku: {filePath}");
    //}
}
}
