using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Model;
using ZTP_WPF_Project.MVVM.Core;
using RelayCommand = ZTP_WPF_Project.MVVM.Core.RelayCommand;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class BudgetViewModel : BaseViewModel
    {
        protected readonly TransactionCategoryViewModel _categoryVM;
        protected readonly TransactionViewModel _transactionVM;

        public BudgetViewModel(TransactionViewModel transactionVM, TransactionCategoryViewModel categoryVM)
        {
            this._categoryVM = categoryVM;
            this._transactionVM = transactionVM;
        }

        public List<TransactionModel> GetAllBudgetTransactions()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (budgetTransactions.Count > 0 ? budgetTransactions : new());
        }

        public float GetValues()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (budgetTransactions.Count > 0 ? budgetTransactions.Sum(o=>o.Amount) : 0.0f);
        }

        public uint GetCount()
        {
            var budgetTransactions = _transactionVM.GetAll().Where(o => o._Type == TransactionType.Income).ToList();

            return (uint)budgetTransactions.Count;
        }
    }
}
