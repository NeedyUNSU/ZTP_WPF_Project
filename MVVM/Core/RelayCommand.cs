using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
                command.Execute(null);
                _undoStack.Push(command);
            }
        }
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
        private readonly ObservableCollection<TransactionModel> _transactions;
        private TransactionModel _transaction;

        public AddTransactionCommand(ObservableCollection<TransactionModel> transactions, TransactionModel transaction)
        {
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
        }
    }

    public class RemoveTransactionCommand : TransactionCommand
    {
        private readonly ObservableCollection<TransactionModel> _transactions;
        private TransactionModel _transaction;

        public RemoveTransactionCommand(ObservableCollection<TransactionModel> transactions, TransactionModel transaction)
        {
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
        }
    }

    public class EditTransactionCommand : TransactionCommand
    {
        private readonly TransactionModel _originalTransaction;
        private readonly TransactionModel _updatedTransaction;
        private readonly ObservableCollection<TransactionModel> _transactions;

        public EditTransactionCommand(ObservableCollection<TransactionModel> transactions, TransactionModel original, TransactionModel updated)
        {
            _transactions = transactions;
            _originalTransaction = original;
            _updatedTransaction = updated;
        }

        public override bool CanExecute(object? parameter) => _transactions.Contains(_originalTransaction);

        public override void Execute(object? parameter)
        {
            int index = _transactions.IndexOf(_originalTransaction);
            if (index >= 0)
            {
                _transactions[index] = _updatedTransaction;
            }
        }

        public override void Undo()
        {
            int index = _transactions.IndexOf(_updatedTransaction);
            if (index >= 0)
            {
                _transactions[index] = _originalTransaction;
            }
        }
    }

}
