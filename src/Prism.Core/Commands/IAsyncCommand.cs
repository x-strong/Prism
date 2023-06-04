using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

#nullable enable
namespace Prism.Commands
{
    internal interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object? parameter);
        Task ExecuteAsync(object? parameter, CancellationToken cancellationToken);
    }

    public class DemoCommand : IAsyncCommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Func<CancellationToken, Task> _execute;

        public DemoCommand(Func<CancellationToken, Task> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public DemoCommand(Func<CancellationToken, Task> execute)
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

        public Task Execute() => Execute(default);

        public Task Execute(CancellationToken cancellationToken)
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

        Task IAsyncCommand.ExecuteAsync(object? parameter) => Execute();

        Task IAsyncCommand.ExecuteAsync(object? parameter, CancellationToken cancellationToken) => Execute(cancellationToken);
    }
}
