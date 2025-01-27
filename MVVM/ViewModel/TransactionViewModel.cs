using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows; // application.Current.mainwindow etc.
using ZTP_WPF_Project.MVVM.Core;
using ZTP_WPF_Project.MVVM.Model;
using CommandManager = ZTP_WPF_Project.MVVM.Core.CommandManager;
using RelayCommand = ZTP_WPF_Project.MVVM.Core.RelayCommand;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;

namespace ZTP_WPF_Project.MVVM.ViewModel
{
    public class TransactionViewModel : BaseViewModel<TransactionModel>
    {
        protected readonly TransactionCategoryViewModel categoryVM;
        protected Notification Notification;

        private readonly CommandManager _commandManager = new();

        private TransactionModel selectedTransaction;

        public TransactionModel SelectedTransactions
        {
            get { return selectedTransaction; }
            set { selectedTransaction = value; OnPropertyChanged(); }
        }

        private MainViewModel MainContext
        {
            get
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null && mainWindow.DataContext is MainViewModel)
                {
                    return mainWindow.DataContext as MainViewModel;
                }
                else
                {
                    throw new InvalidOperationException("Main data context must be MainWindowViewModel");
                }
            }
        }

        private ObservableCollection<TransactionModel> transactionsExpenseCache = new();

        public ObservableCollection<TransactionModel> TransactionsExpenseCache
        {
            get { return transactionsExpenseCache; }
            set { transactionsExpenseCache = value; OnPropertyChanged(); }
        }

        private ObservableCollection<TransactionModel> transactionsIncomeCache = new();

        public ObservableCollection<TransactionModel> TransactionsIncomeCache
        {
            get { return transactionsIncomeCache; }
            set { transactionsIncomeCache = value; OnPropertyChanged(); }
        }

        public bool IsButtonEnabled => SelectedTransactions != null;

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ExportToCSVCommand { get; }

        public ICommand ReportsPageCommand { get; }
        public ICommand BudgetPageCommand { get; }
        public ICommand ForecastPageCommand { get; }

        public TransactionViewModel(TransactionCategoryViewModel categoryVM)
        {
            this.categoryVM = categoryVM;
            _values = new();

            Notification = new Notification();
            Notification.Attach(new BudgetOverNinety(GetBudget()));
            Notification.Attach(new Overrun(GetBudget()));
            Notification.Attach(new Congratulation(GetBudget()));
                        
            Add(new TransactionModel("TESTyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "testowa tranzakcja", 1000.00f, TransactionType.Income, null));

            Add(new TransactionModel("TESTyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy2", "testowa tranzakcja", 10.00f, TransactionType.Expense, new TransactionCategoryModel("food", "jedzenie potrzebne do życia")));



            TransactionsExpenseCache = new(GetAll().Where(_ => _._Type == TransactionType.Expense).ToList());
            TransactionsIncomeCache = new(GetAll().Where(_ => _._Type == TransactionType.Income).ToList());


            AddCommand = new RelayCommand(param =>
            {
                var transaction = param as TransactionModel;
                if (transaction != null)
                {
                    var command = new AddTransactionCommand(new(_values), _values, transaction);
                    _commandManager.ExecuteCommand(command);
                }
            });

            RemoveCommand = new RelayCommand(param =>
            {
                var transaction = param as TransactionModel;
                if (transaction != null)
                {
                    var command = new RemoveTransactionCommand(new(_values), _values,  transaction);
                    _commandManager.ExecuteCommand(command);
                }
            });

            EditCommand = new RelayCommand(param =>
            {
                var updatedTransaction = param as TransactionModel;
                if (updatedTransaction != null)
                {
                    var originalTransaction = GetById(updatedTransaction.Id);
                    if (originalTransaction != null)
                    {
                        var command = new EditTransactionCommand(new(_values), _values, originalTransaction, updatedTransaction);
                        _commandManager.ExecuteCommand(command);
                    }
                }
            });

            UndoCommand = new RelayCommand(_ => _commandManager.Undo());
            RedoCommand = new RelayCommand(_ => _commandManager.Redo());

            ReportsPageCommand = new RelayCommand(_ => OpenReportsPage());
            BudgetPageCommand = new RelayCommand(_ => OpenBudgetPage());
            ForecastPageCommand = new RelayCommand(_ => OpenForecastPage());

            ExportToCSVCommand = new RelayCommand(_ => ExportToCSVFile());
        }

        private void OpenReportsPage()       
        {                                    
            MainContext.ShowReportPage.Execute(this);  
        }                                    
                                             
        private void OpenBudgetPage()        
        {                                    
            MainContext.ShowBudgetPage.Execute(this);
        }                                    
                                             
        private void OpenForecastPage()      
        {
            MainContext.ShowForecastPage.Execute(this); 
        }                
        
        private void ExportToCSVFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save Transactions as",
                Filter = "CSV (*.csv)|*.csv|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true && _values.Count > 0)
            {
                string filePath = saveFileDialog.FileName;
                MessageBox.Show($"Saved file path: {filePath}");


                string fileContent = "Id;Title;Description;Amount;Type;AddedDate;CategoryId;CategoryName;CategoryDesc;\n" + string.Join('\n', _values);
                File.WriteAllText(filePath, fileContent, Encoding.UTF8);
            }
            else
            {
                MessageBox.Show("Saving file failed.");
            }
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
                    catch (Exception ex)
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

            if (obj._category != null && categoryVM?.GetAll()?.Where(o => o.Id == obj.CategoryId).FirstOrDefault() == null)
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

            if (val?._Type == TransactionType.Expense)
            {
                NotifyObservers(-1 * val.Amount);
            }

            if (obj._Type == TransactionType.Expense)
            {
                NotifyObservers(obj.Amount);
            }

            return true;
        }

        public override bool DeleteById(string id)
        {
            var val = _values?.Where(x => x.Id == id).FirstOrDefault();
            if (val == null) return false;

            if (val._Type == TransactionType.Expense)
            {
                NotifyObservers(-1 * val.Amount);
            }

            return _values != null ? _values.Remove(val) : false;
        }

        public override bool Add(TransactionModel obj)
        {
            if (obj == null) return false;

            if (_values?.Where(x => x.Id == obj.Id).Where(x => x?.Title?.ToLower() == obj?.Title?.ToLower()).Where(x => x?.Description?.ToLower() == obj?.Description?.ToLower()).Count() == 0)
            {
                _values.Add(obj);

                if (obj._Type == TransactionType.Expense)
                {
                    NotifyObservers(obj.Amount);
                }

                return true;
            }
            return false;
        }

        private double GetBudget()
        {
            if (_values == null) return 0;

            return _values.Where(x=>x._Type == TransactionType.Income).Sum(o => o.Amount);
        }

        private void NotifyObservers(double Expense)
        {
            Notification.CurrentBudget = GetBudget();
            Notification.Expense = Expense;
        }


    }
}
