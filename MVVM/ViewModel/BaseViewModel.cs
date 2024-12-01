using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public abstract class BaseViewModel<T>
    {
        protected List<T> _values;

        public abstract void Load();
        public abstract void Save();
        public List<T> GetAll() { return _values; }
        public abstract T GetById(string id);
        public abstract bool ModifyById(string id, T obj);
    }
}
