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
using System.ComponentModel;
using System.Transactions;
using System.Data.Common;

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
            set
            {
                selectedTransaction = value;
                OnPropertyChanged(nameof(SelectedTransactions));
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
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
        public bool IsButtonEnabledCSV => _values.Count() > 0;
        public bool IsButtonEnabledActionUndo => _commandManager.UndoStack;
        public bool IsButtonEnabledActionRedo => _commandManager.RedoStack;

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ExportToCSVCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand ReportsPageCommand { get; }
        public ICommand BudgetPageCommand { get; }
        public ICommand ForecastPageCommand { get; }

        private TransactionProxy validationObject;

        public TransactionProxy ValidationObject
        {
            get 
            {
                return validationObject; 
            }
            set 
            { 
                validationObject = value;
                OnPropertyChanged(nameof(TransactionObjectId));
                OnPropertyChanged(nameof(TransactionObjectTitle));
                OnPropertyChanged(nameof(TransactionObjectDescription));
                OnPropertyChanged(nameof(TransactionObjectAmount));
                OnPropertyChanged(nameof(TransactionObjectType));
                OnPropertyChanged(nameof(TransactionObjectAddedDate));
                OnPropertyChanged(nameof(TransactionObjectCategory));
                OnPropertyChanged(nameof(TypeIsntIncome));
            }
        }

        public string TransactionObjectId
        {
            get
            {
                return ValidationObject.Id;
            }
            set
            {
                ValidationObject.Id = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationObject));
            }
        }

        public string TransactionObjectTitle
        {
            get
            {
                return ValidationObject.Title;
            }
            set
            {
                ValidationObject.Title = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationObject));
            }
        }

        public string TransactionObjectDescription
        {
            get
            {
                return ValidationObject.Description;
            }
            set
            {
                ValidationObject.Description = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationObject));
            }
        }

        public float TransactionObjectAmount
        {
            get
            {
                return ValidationObject.Amount;
            }
            set
            {
                ValidationObject.Amount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationObject));
            }
        }

        public TransactionType TransactionObjectType
        {
            get
            {
                return ValidationObject.Type;
            }
            set
            {
                ValidationObject.Type = value;
                if (value == TransactionType.Income) TransactionObjectCategory = null;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationObject));
                OnPropertyChanged(nameof(TypeIsntIncome));
            }
        }

        public DateTime TransactionObjectAddedDate
        {
            get
            {
                return ValidationObject.AddedDate;
            }
            set
            {
                ValidationObject.AddedDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationObject));
            }
        }

        public TransactionCategoryModel TransactionObjectCategory
        {
            get
            {
                return ValidationObject.Category;
            }
            set
            {
                ValidationObject.Category = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidationObject));
            }
        }

        private ObservableCollection<TransactionCategoryModel> transactionCategories;
        public ObservableCollection<TransactionCategoryModel> TransactionCategories
        {
            get { return transactionCategories; }
            set { transactionCategories = value; OnPropertyChanged(); }
        }

        private ObservableCollection<TransactionType> transactionTypes;
        public ObservableCollection<TransactionType> TransactionTypes
        {
            get { return transactionTypes; }
            set { transactionTypes = value; OnPropertyChanged(); }
        }

        public bool TypeIsntIncome => TransactionObjectType != TransactionType.Income;

        private bool isEditorOpen = false;

        public bool IsEditorOpen
        {
            get { return isEditorOpen; }
            set { isEditorOpen = value; OnPropertyChanged(); }
        }

        private bool isAddingOpen = false;

        public bool IsAddingOpen
        {
            get { return isAddingOpen; }
            set { isAddingOpen = value; OnPropertyChanged(); }
        }


        private string editorTitle;

        public string EditorTitle
        {
            get { return editorTitle; }
            set { editorTitle = value; OnPropertyChanged(); }
        }



        public TransactionViewModel(TransactionCategoryViewModel categoryVM)
        {
            this.categoryVM = categoryVM;
            Load();

            Notification = new Notification();
            Notification.Attach(new BudgetOverNinety(GetBudget()));
            Notification.Attach(new Overrun(GetBudget()));
            Notification.Attach(new Congratulation(GetBudget()));

            TransactionTypes = new ObservableCollection<TransactionType>(Enum.GetValues(typeof(TransactionType)) as TransactionType[]);
            TransactionTypes.Remove(TransactionType.None);
            TransactionCategories = new ObservableCollection<TransactionCategoryModel>();
            TransactionCategories = new(categoryVM.GetAll());

            Add(new TransactionModel("TESTyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy", "testowa tranzakcja", 1000.00f, TransactionType.Income, null));

            Add(new TransactionModel("TESTyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy2", "testowa tranzakcja", 10.00f, TransactionType.Expense, categoryVM.GetAll().FirstOrDefault()));

            validationObject = new TransactionProxy(new TransactionModel("","",0,TransactionType.Income,null));

            ReloadTables();

            SaveCommand = new RelayCommand(param => 
            {
                if (ValidationObject == null) return;

                if (IsAddingOpen)
                {
                    var transaction = ValidationObject;
                    if (transaction != null)
                    {
                        var item = transaction.GetTransactionModel;
                        var command = new AddTransactionCommand(new(_values), _values, item);
                        _commandManager.ExecuteCommand(command);
                        OnPropertyChanged(nameof(IsButtonEnabledActionUndo));
                        OnPropertyChanged(nameof(IsButtonEnabledActionRedo));
                        CancelCommand.Execute(this);
                    }
                }
                else if (IsEditorOpen)
                {
                    var updatedTransaction = ValidationObject;
                    if (updatedTransaction != null)
                    {
                        var item = updatedTransaction.GetTransactionModel;

                        var originalTransaction = GetById(item.Id);
                        if (originalTransaction != null)
                        {
                            var command = new EditTransactionCommand(new(_values), _values, originalTransaction, new(item));
                            _commandManager.ExecuteCommand(command);
                            OnPropertyChanged(nameof(IsButtonEnabledActionUndo));
                            OnPropertyChanged(nameof(IsButtonEnabledActionRedo));
                            CancelCommand.Execute(this);
                        }
                    }
                }

                ReloadTables();
            });

            AddCommand = new RelayCommand(param =>
            {
                var item = new TransactionModel("","",0,TransactionType.Income,null);
                

                ValidationObject = new TransactionProxy(item);
                OnPropertyChanged(nameof(TransactionObjectId));
                OnPropertyChanged(nameof(TransactionObjectTitle));
                OnPropertyChanged(nameof(TransactionObjectDescription));
                OnPropertyChanged(nameof(TransactionObjectAmount));
                OnPropertyChanged(nameof(TransactionObjectType));
                OnPropertyChanged(nameof(TransactionObjectAddedDate));
                OnPropertyChanged(nameof(TransactionObjectCategory));
                OnPropertyChanged(nameof(TypeIsntIncome));
                EditorTitle = "Adder";
                IsEditorOpen = true;
                IsAddingOpen = true;

                //var transaction = param as TransactionModel;
                //if (transaction != null)
                //{
                //    var command = new AddTransactionCommand(new(_values), _values, transaction);
                //    _commandManager.ExecuteCommand(command);
                //    OnPropertyChanged(nameof(IsButtonEnabledActionUndo));
                //    OnPropertyChanged(nameof(IsButtonEnabledActionRedo));
                //}
            });

            CancelCommand = new RelayCommand(param =>
            {
                ValidationObject = new TransactionProxy(new TransactionModel("", "", 0, TransactionType.Income, null));
                OnPropertyChanged(nameof(TransactionObjectId));
                OnPropertyChanged(nameof(TransactionObjectTitle));
                OnPropertyChanged(nameof(TransactionObjectDescription));
                OnPropertyChanged(nameof(TransactionObjectAmount));
                OnPropertyChanged(nameof(TransactionObjectType));
                OnPropertyChanged(nameof(TransactionObjectAddedDate));
                OnPropertyChanged(nameof(TransactionObjectCategory));
                OnPropertyChanged(nameof(TypeIsntIncome));

                ReloadTables();
                IsEditorOpen = false;
                IsAddingOpen = false;
            });

            RemoveCommand = new RelayCommand(param =>
            {
                var transaction = param as TransactionModel;
                if (transaction != null)
                {
                    var command = new RemoveTransactionCommand(new(_values), _values, transaction);
                    _commandManager.ExecuteCommand(command);
                    OnPropertyChanged(nameof(IsButtonEnabledActionUndo));
                    OnPropertyChanged(nameof(IsButtonEnabledActionRedo));

                    ReloadTables();

                }
            });

            EditCommand = new RelayCommand(param =>
            {
                ValidationObject = new TransactionProxy(new(SelectedTransactions));
                OnPropertyChanged(nameof(TransactionObjectId));
                OnPropertyChanged(nameof(TransactionObjectTitle));
                OnPropertyChanged(nameof(TransactionObjectDescription));
                OnPropertyChanged(nameof(TransactionObjectAmount));
                OnPropertyChanged(nameof(TransactionObjectType));
                OnPropertyChanged(nameof(TransactionObjectAddedDate));
                OnPropertyChanged(nameof(TransactionObjectCategory));
                OnPropertyChanged(nameof(TypeIsntIncome));
                EditorTitle = "Editor";
                IsEditorOpen = true;

                //var updatedTransaction = param as TransactionModel;
                //if (updatedTransaction != null)
                //{
                //    var originalTransaction = GetById(updatedTransaction.Id);
                //    if (originalTransaction != null)
                //    {
                //        var command = new EditTransactionCommand(new(_values), _values, originalTransaction, updatedTransaction);
                //        _commandManager.ExecuteCommand(command);
                //        OnPropertyChanged(nameof(IsButtonEnabledActionUndo));
                //        OnPropertyChanged(nameof(IsButtonEnabledActionRedo));
                //    }
                //}
            });

            UndoCommand = new RelayCommand(_ => 
            { 
                _commandManager.Undo();
                OnPropertyChanged(nameof(IsButtonEnabledActionUndo));
                OnPropertyChanged(nameof(IsButtonEnabledActionRedo));
                ReloadTables();
            });
            RedoCommand = new RelayCommand(_ => 
            { 
                _commandManager.Redo();
                OnPropertyChanged(nameof(IsButtonEnabledActionUndo));
                OnPropertyChanged(nameof(IsButtonEnabledActionRedo));
                ReloadTables();
            });


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

        private void ReloadTables()
        {
            TransactionsExpenseCache = new(GetAll().Where(_ => _._Type == TransactionType.Expense).ToList());
            TransactionsIncomeCache = new(GetAll().Where(_ => _._Type == TransactionType.Income).ToList());
            SelectedTransactions = null;
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

            Save();

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

            Save();
            return _values != null ? _values.Remove(val) : false;
        }

        public override bool Add(TransactionModel obj)
        {
            if (obj == null) return false;

            if (_values?.Where(x => x?.Title?.ToLower() == obj?.Title?.ToLower()).Where(x => x?.Description?.ToLower() == obj?.Description?.ToLower()).Count() == 0)
            {
                _values.Add(obj);

                if (obj._Type == TransactionType.Expense)
                {
                    NotifyObservers(obj.Amount);
                }

                Save();
                return true;
            }
            return false;
        }

        private double GetBudget()
        {
            if (_values == null) return 0;

            return _values.Where(x => x._Type == TransactionType.Income).Sum(o => o.Amount);
        }

        private void NotifyObservers(double Expense)
        {
            Notification.CurrentBudget = GetBudget();
            Notification.Expense = Expense;
        }


    }
}
