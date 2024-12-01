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
        private readonly string _filePath = "_TransactionDB.xml";

        public override void Load()
        {
            _values = DataManager.LoadFromXmlFile<TransactionModel>(_filePath);
        }

        public override void Save()
        {
            DataManager.SaveToXMLFile(_values, _filePath);
        }

        public override TransactionModel GetById(string id)
        {
            return _values.Where(x => x.Id == id).FirstOrDefault();
        }

        public override bool ModifyById(string id, TransactionModel obj)
        {
            if (obj == null) return false;
            var val = _values.Where(x => x.Id == id).FirstOrDefault();
            if (val == null) return false;
            
            if (val.Title != obj.Title) val.Title = obj.Title;
            if (val.Description != obj.Description) val.Description = obj.Description;
            if (val.Amount != obj.Amount) val.Amount = obj.Amount;
            if (val._Type != obj._Type) val._Type = obj._Type;
            if (val.AddedDate != obj.AddedDate) val.AddedDate = obj.AddedDate;
            if (val._categoryModel != obj._categoryModel) val._categoryModel = obj._categoryModel;

            return true;
        }
    }
}
