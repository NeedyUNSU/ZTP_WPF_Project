using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ZTP_WPF_Project.MVVM.Model;

namespace ZTP_WPF_Project.MVVM.Core
{
    public class TransactionProxy : INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected TransactionModel transaction;

        public bool HasErrors => transaction.Validate();

        public TransactionProxy(TransactionModel transaction)
        {
            this.transaction = transaction;
        }



        public IEnumerable GetErrors(string? propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
