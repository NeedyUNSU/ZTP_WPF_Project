using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP_WPF_Project.MVVM.Model
{
    public class ReportModel : BaseModel
    {
        
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Data utworzenia raportu.
        public decimal TotalIncome { get; set; } // Całkowite przychody (tylko do odczytu).
        public decimal TotalExpenses { get; set; } // Całkowite wydatki (tylko do odczytu).
        public decimal MaxIncome { get; set; } // maksymalny przychód
        public decimal MaxExpenses { get; set; } //maksymalny wydatek
        public decimal Amount_income { get; set; } //ilość przychodów
        public decimal Amount_expenses { get; set; } //ilość wydatków

        public List<TransactionModel>? transactions;

        public override bool Validate()
        {
            if (transactions?.Count != 0) return true; else return false;
        }
    }
}
