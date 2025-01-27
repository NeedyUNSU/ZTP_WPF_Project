using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Core;
using ZTP_WPF_Project.MVVM.Model;
using RelayCommand = ZTP_WPF_Project.MVVM.Core.RelayCommand;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class TransactionCategoryViewModel : BaseViewModel<TransactionCategoryModel>
    {

        public TransactionCategoryViewModel()
        {
            Load();

            Add(new TransactionCategoryModel("test0", "test"));
            Add(new TransactionCategoryModel("test1", "test"));
            Add(new TransactionCategoryModel("test2", "test"));
            Add(new TransactionCategoryModel("test3", "test"));
            Add(new TransactionCategoryModel("test4", "test"));
        }

        public override bool Add(TransactionCategoryModel obj)
        {
            if (obj == null) return false;

            if (_values?.Where(x => x?.Name?.ToLower() == obj?.Name?.ToLower()).Where(x => x?.Description?.ToLower() == obj?.Description?.ToLower()).Count() == 0)
            {
                _values.Add(new(obj));

                Save();
                return true;
            }
            return false;
        }

        public override bool DeleteById(string id)
        {
            var val = _values?.Where(x => x.Id == id).FirstOrDefault();
            if (val == null) return false;

            Save();
            return _values != null ? _values.Remove(val) : false;
        }

        public override TransactionCategoryModel? GetById(string id)
        {
            if (_values == null || _values.Count == 0 || string.IsNullOrWhiteSpace(id)) return null;

            return _values.Where(x => x.Id == id).FirstOrDefault();
        }

        public override bool ModifyById(string id, TransactionCategoryModel obj)
        {
            if (obj == null) return false;
            var val = _values?.Where(x => x.Id == id).FirstOrDefault();
            if (val == null) return false;

            if (val.Name != obj.Name) val.Name = obj.Name;
            if (val.Description != obj.Description) val.Description = obj.Description;

            Save();
            return true;
        }

        public override void Load()
        {
            _values = DataManager.LoadTransactionCategories();
        }

        public override void Save()
        {
            DataManager.SaveTransactionCategories(_values != null ? _values : new());
        }
    }
}
