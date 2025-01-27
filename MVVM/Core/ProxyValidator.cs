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
        private readonly TransactionModel _transactionModel;
        private readonly Dictionary<string, List<string>> _errors = new();

        public TransactionProxy(TransactionModel transactionModel)
        {
            _transactionModel = transactionModel;
        }

        public string? Id
        {
            get => _transactionModel.Id;
            set
            {
                _transactionModel.Id = value;
            }
        }

        public string? Title
        {
            get => _transactionModel.Title;
            set
            {
                _transactionModel.Title = value;
                ValidateProperty(nameof(Title));
            }
        }

        public string? Description
        {
            get => _transactionModel.Description;
            set
            {
                _transactionModel.Description = value;
                ValidateProperty(nameof(Description));
            }
        }

        public float Amount
        {
            get => _transactionModel.Amount;
            set
            {
                _transactionModel.Amount = value;
                ValidateProperty(nameof(Amount));
            }
        }

        public TransactionType Type
        {
            get => _transactionModel._Type;
            set
            {
                _transactionModel._Type = value;
                if (value == TransactionType.Income)
                {
                    Category = null;
                }
                ValidateProperty(nameof(Type));
            }
        }

        public DateTime AddedDate
        {
            get => _transactionModel.AddedDate;
            set => _transactionModel.AddedDate = value;
        }

        public TransactionCategoryModel? Category
        {
            get => _transactionModel._category;
            set
            {
                _transactionModel._category = value;
                ValidateProperty(nameof(Category));
            }
        }

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        public Dictionary<string, List<string>> GetErrors()
        {
            return _errors;
        }

        private void ValidateProperty(string propertyName)
        {
            ClearErrors(propertyName);

            switch (propertyName)
            {
                case nameof(Title):
                    if (string.IsNullOrWhiteSpace(Title))
                        AddError(propertyName, "Title cannot be empty.");
                    break;

                case nameof(Description):
                    if (string.IsNullOrWhiteSpace(Description))
                        AddError(propertyName, "Description cannot be empty.");
                    break;

                case nameof(Amount):
                    if (Amount <= 0.01f)
                        AddError(propertyName, "Amount must be greater than zero.");
                    break;

                case nameof(Type):
                    if (Type == TransactionType.None)
                        AddError(propertyName, "Transaction type must be selected.");
                    break;

                case nameof(Category):
                    if (Type == TransactionType.Expense && Category == null)
                        AddError(propertyName, "Category is required for expenses.");
                    break;
            }
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
