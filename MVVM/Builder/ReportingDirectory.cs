using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Model;
using ZTP_WPF_Project.MVVM.Strategy;

namespace ZTP_WPF_Project.MVVM.Builder
{
    public class ReportingDirectory
    {
        private readonly IReportingStrategy _reportingStrategy;

        public ReportingDirectory(IReportingStrategy reportingStrategy)
        {
            _reportingStrategy = reportingStrategy;
        }

        public void ExecuteReportGeneration(ReportModel report)
        {
            _reportingStrategy.GenerateReport(report);
        }
    }
}
