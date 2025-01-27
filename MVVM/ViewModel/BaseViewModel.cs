using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public abstract class BaseViewModel<T> : INotifyPropertyChanged
    {
        protected List<T>? _values;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Load();
        public abstract void Save();
        public List<T>? GetAll() { return _values; }
        public abstract T? GetById(string id);
        public abstract bool ModifyById(string id, T obj);
        public abstract bool DeleteById(string id);
        public bool DeleteAll()
        {
            _values?.Clear();
            if (_values?.Count == 0) return true;
            else return false;
        }
        public abstract bool Add(T obj);
    }

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
