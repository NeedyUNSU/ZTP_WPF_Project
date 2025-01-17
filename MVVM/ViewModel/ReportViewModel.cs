using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Model;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class ReportViewModel : BaseViewModel<ReportModel>
    {
        public override bool Add(ReportModel obj)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteById(string id)
        {
            throw new NotImplementedException();
        }

        public override ReportModel? GetById(string id)
        {
            throw new NotImplementedException();
        }

        public override void Load()
        {
            throw new NotImplementedException();
        }

        public override bool ModifyById(string id, ReportModel obj)
        {
            throw new NotImplementedException();
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
        private bool _isYearlyReport;
        private bool _isMonthlyReport;
        private string _reportDateRange;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsYearlyReport
        {
            get => _isYearlyReport;
            set
            {
                _isYearlyReport = value;
                OnPropertyChanged();
                UpdateReportDateRange();
            }
        }

        public bool IsMonthlyReport
        {
            get => _isMonthlyReport;
            set
            {
                _isMonthlyReport = value;
                OnPropertyChanged();
                UpdateReportDateRange();
            }
        }

        public string ReportDateRange
        {
            get => _reportDateRange;
            set
            {
                _reportDateRange = value;
                OnPropertyChanged();
            }
        }

        public ReportViewModel()
        {
            // Ustawienia domyślne
            IsYearlyReport = true; // Domyślnie roczny raport
            UpdateReportDateRange();
        }

        private void UpdateReportDateRange()
        {
            var today = DateTime.Now;
            if (IsYearlyReport)
            {
                var start = today.AddYears(-1).AddDays(1); // -1 rok, następny dzień
                ReportDateRange = $"Raport dotyczy okresu: {start:dd.MM.yyyy} - {today:dd.MM.yyyy}";
            }
            else if (IsMonthlyReport)
            {
                var start = today.AddMonths(-1).AddDays(1); // -1 miesiąc, następny dzień
                ReportDateRange = $"Raport dotyczy okresu: {start:dd.MM.yyyy} - {today:dd.MM.yyyy}";
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
