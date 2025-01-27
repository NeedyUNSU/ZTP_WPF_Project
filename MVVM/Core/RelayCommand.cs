using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Model;
using ZTP_WPF_Project.MVVM.ViewModel;

namespace ZTP_WPF_Project.MVVM.Core
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }

    public class CommandManager
    {
        private readonly Stack<TransactionCommand> _undoStack = new();
        private readonly Stack<TransactionCommand> _redoStack = new();

        public static EventHandler? RequerySuggested { get; internal set; }

        public void ExecuteCommand(TransactionCommand command)
        {
            command.Execute(null);
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Any())
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Any())
            {
                var command = _redoStack.Pop();
                command.Execute(this);
                _undoStack.Push(command);
            }
        }

        public bool UndoStack => _undoStack.Any();
        public bool RedoStack => _redoStack.Any();
    }


    public abstract class TransactionCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public abstract bool CanExecute(object? parameter);
        public abstract void Execute(object? parameter);

        public abstract void Undo();
    }

    public class AddTransactionCommand : TransactionCommand
    {
        private List<TransactionModel> _transactions;
        private List<TransactionModel> _transactionsSaveState;
        private TransactionModel _transaction;

        public AddTransactionCommand(List<TransactionModel> transactionsSave, List<TransactionModel> transactions, TransactionModel transaction)
        {
            _transactionsSaveState = transactionsSave;
            _transactions = transactions;
            _transaction = transaction;
        }

        public override bool CanExecute(object? parameter) => _transaction != null;

        public override void Execute(object? parameter)
        {
            _transactions.Add(_transaction);
        }

        public override void Undo()
        {
            _transactions.Remove(_transaction);
            //_transactions = new(_transactionsSaveState);
        }
    }

    public class RemoveTransactionCommand : TransactionCommand
    {
        private List<TransactionModel> _transactions;
        private List<TransactionModel> _transactionsSaveState;
        private TransactionModel _transaction;

        public RemoveTransactionCommand(List<TransactionModel> transactionsSave, List<TransactionModel> transactions, TransactionModel transaction)
        {
            _transactionsSaveState = transactionsSave;
            _transactions = transactions;
            _transaction = transaction;
        }

        public override bool CanExecute(object? parameter) => _transactions.Contains(_transaction);

        public override void Execute(object? parameter)
        {
            _transactions.Remove(_transaction);
        }

        public override void Undo()
        {
            _transactions.Add(_transaction);
            //_transactions = new(_transactionsSaveState);
        }
    }

    public class EditTransactionCommand : TransactionCommand
    {
        private readonly TransactionModel _originalTransaction;
        private readonly TransactionModel _updatedTransaction;
        private List<TransactionModel> _transactions;
        private List<TransactionModel> _transactionsSaveState;

        public EditTransactionCommand(List<TransactionModel> transactionsSave, List<TransactionModel> transactions, TransactionModel original, TransactionModel updated)
        {
            _transactionsSaveState = transactionsSave;
            _transactions = transactions;
            _originalTransaction = original;
            _updatedTransaction = updated;
        }

        public override bool CanExecute(object? parameter) => _transactions.Contains(_originalTransaction);

        public override void Execute(object? parameter)
        {
            int index = _transactions.IndexOf(_transactions.Where(o => o.Id == _originalTransaction.Id).FirstOrDefault());
            if (index >= 0)
            {
                _transactions[index] = _updatedTransaction;
            }
        }

        public override void Undo()
        {
            int index = _transactions.IndexOf(_transactions.Where(o => o.Id == _updatedTransaction.Id).FirstOrDefault());
            if (index >= 0)
            {
                _transactions[index] = _originalTransaction;
            }

            //_transactions = new(_transactionsSaveState);
        }
    }

}
