using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Model;

namespace ZTP_WPF_Project.MVVM.Strategy
{
    public class ReportingYear : IReportingStrategy
    {
        public void GenerateReport(ReportModel report)
        {
            // Logika generowania raportu rocznego
            Console.WriteLine($"Generowanie raportu rocznego: {report.Title}");
            // Implementacja generowania PDF, dodanie danych itp.
        }
    }
}
