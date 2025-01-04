using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Core;
using ZTP_WPF_Project.MVVM.Model;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class TransactionViewModel : BaseViewModel<TransactionModel>
    {
        protected readonly TransactionCategoryViewModel categoryVM;

        public TransactionViewModel(TransactionCategoryViewModel categoryVM)
        {
            this.categoryVM = categoryVM;
        }

        public override void Load()
        {
            _values = DataManager.LoadTransactions();

            foreach (var item in (_values != null ? _values : new()))
            {
                if (item != null && item.CategoryIdXaml != null && item._Type == TransactionType.Expense)
                {
                    var category = categoryVM.GetById(item.CategoryIdXaml);
                    try
                    {
                        item._category = category;
                    }
                    catch(Exception ex)
                    {
                        throw new CustomException(ex.Message, ex);
                    }
                }
            }
        }

        public override void Save()
        {
            DataManager.SaveTransactions(_values != null ? _values : new());
        }

        public override TransactionModel? GetById(string id)
        {
            if (_values == null || _values.Count == 0 || string.IsNullOrWhiteSpace(id)) return null;

            var value = _values.Where(x => x.Id == id).FirstOrDefault();

            RefreshCategory(value != null ? value : new());

            return value;
        }

        private void RefreshCategory(TransactionModel obj)
        {
            if (obj == null) return;

            if (obj._category != null && categoryVM?.GetAll()?.Where(o=>o.Id == obj.CategoryId).FirstOrDefault() == null)
            {
                obj._category = null;
                obj.CategoryIdXaml = null;
            }
        }

        public override bool ModifyById(string id, TransactionModel obj)
        {
            if (obj == null) return false;
            var val = _values?.Where(x => x.Id == id).FirstOrDefault();
            if (val == null) return false;

            if (val.Title != obj.Title) val.Title = obj.Title;
            if (val.Description != obj.Description) val.Description = obj.Description;
            if (val.Amount != obj.Amount) val.Amount = obj.Amount;
            if (val._Type != obj._Type) val._Type = obj._Type;
            if (val.AddedDate != obj.AddedDate) val.AddedDate = obj.AddedDate;
            if (val._category != obj._category) val._category = obj._category;

            RefreshCategory(val != null ? val : new());

            return true;
        }

        public override bool DeleteById(string id)
        {
            var val = _values?.Where(x => x.Id == id).FirstOrDefault();
            if (val == null) return false;

            return _values != null ? _values.Remove(val) : false;
        }

        public override bool Add(TransactionModel obj)
        {
            if (obj == null) return false;

            if (_values?.Where(x => x.Id == obj.Id).Where(x => x?.Title?.ToLower() == obj?.Title?.ToLower()).Where(x => x?.Description?.ToLower() == obj?.Description?.ToLower()).Where(x => x.Amount == obj.Amount).Where(x => x._Type.ToString().ToLower() == obj._Type.ToString().ToLower()).Count() == 0)
            {
                _values.Add(obj);
                return true;
            }
            return false;
        }


    }
}
