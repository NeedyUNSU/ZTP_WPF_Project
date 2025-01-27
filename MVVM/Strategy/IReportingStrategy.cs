using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Model;

namespace ZTP_WPF_Project.MVVM.Strategy
{
    public interface IReportingStrategy
    {
        void GenerateReport(ReportModel report);
    }
}
