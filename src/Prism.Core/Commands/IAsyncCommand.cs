using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

#nullable enable
namespace Prism.Commands
{
    internal interface IAsyncCommand : ICommand
    {
        ValueTask ExecuteAsync(object? parameter);
        ValueTask ExecuteAsync(object? parameter, CancellationToken cancellationToken);
    }

    public class DemoCommand : IAsyncCommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Func<CancellationToken, ValueTask> _execute;

        public DemoCommand(Func<CancellationToken, ValueTask> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DemoCommand(Func<CancellationToken, ValueTask> execute)
            : this(execute, () => true)
        {
        }

        private bool _isExecuting;
        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                _isExecuting = value;
                OnCanExecuteChanged();
            }
        }

        public event EventHandler CanExecuteChanged;

        private void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute() => !IsExecuting && _canExecute();

        public ValueTask Execute() => Execute(default);

        public ValueTask Execute(CancellationToken cancellationToken)
        {
            try
            {
                IsExecuting = true;
                return _execute(cancellationToken);
            }
            finally
            {
                IsExecuting = false;
            }
        }

        bool ICommand.CanExecute(object? parameter) => CanExecute();

        async void ICommand.Execute(object? parameter) => await Execute();

        ValueTask IAsyncCommand.ExecuteAsync(object? parameter) => Execute();

        ValueTask IAsyncCommand.ExecuteAsync(object? parameter, CancellationToken cancellationToken) => Execute(cancellationToken);
    }
}
