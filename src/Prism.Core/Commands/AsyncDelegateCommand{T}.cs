using System;
using System.Threading.Tasks;
using Prism.Properties;
using System.Linq.Expressions;

namespace Prism.Commands
{
    public class AsyncDelegateCommand<T> : DelegateCommandBase
    {
        private bool _isExecuting;
        private readonly Func<Task> _executeMethod;
        private Func<T, bool> _canExecuteMethod;

        /// <summary>
        /// Creates a new instance of <see cref="AsyncDelegateCommand"/> with the <see cref="Func{Task}"/> to invoke on execution.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Func{Task}"/> to invoke when <see cref="ICommand.Execute(object)"/> is called.</param>
        public AsyncDelegateCommand(Func<Task> executeMethod)
            : this(executeMethod, _ => true)
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Func{Task}"/> to invoke on execution
        /// and a <see langword="Func" /> to query for determining if the command can execute.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Func{Task}"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T, bool}"/> to invoke when <see cref="ICommand.CanExecute"/> is called</param>
        public AsyncDelegateCommand(Func<Task> executeMethod, Func<T, bool> canExecuteMethod)
            : base()
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Gets the current state of the AsyncDelegateCommand
        /// </summary>
        public bool IsExecuting
        {
            get => _isExecuting;
            private set => SetProperty(ref _isExecuting, value, OnCanExecuteChanged);
        }

        ///<summary>
        /// Executes the command.
        ///</summary>
        public async Task Execute()
        {
            try
            {
                IsExecuting = true;
                await _executeMethod();
            }
            catch (Exception ex)
            {
                if (!HandleException(ex))
                    throw;
            }
            finally
            {
                IsExecuting = false;
            }
        }

        /// <summary>
        /// Determines if the command can be executed.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the command can execute,otherwise returns <see langword="false"/>.</returns>
        public bool CanExecute()
        {
            try
            {
                if (IsExecuting)
                    return false;

                return _canExecuteMethod?.Invoke() ?? true;
            }
            catch (Exception ex)
            {
                if (!HandleException(ex))
                    throw;

                return false;
            }
        }

        /// <summary>
        /// Handle the internal invocation of <see cref="ICommand.Execute(object)"/>
        /// </summary>
        /// <param name="parameter">Command Parameter</param>
        protected override async void Execute(object parameter)
        {
            await Execute();
        }

        /// <summary>
        /// Handle the internal invocation of <see cref="ICommand.CanExecute(object)"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns><see langword="true"/> if the Command Can Execute, otherwise <see langword="false" /></returns>
        protected override bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// Observes a property that implements INotifyPropertyChanged, and automatically calls DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <typeparam name="TType">The type of the return value of the method that this delegate encapsulates</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public AsyncDelegateCommand<T> ObservesProperty<TType>(Expression<Func<TType>> propertyExpression)
        {
            ObservesPropertyInternal(propertyExpression);
            return this;
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public AsyncDelegateCommand<T> ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
        {
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(canExecuteExpression.Body, Expression.Parameter(typeof(T), "o"));
            _canExecuteMethod = expression.Compile();
            ObservesPropertyInternal(canExecuteExpression);
            return this;
        }

        /// <summary>
        /// Provides the ability to connect a delegate to catch exceptions encountered by CanExecute or the Execute methods of the DelegateCommand
        /// </summary>
        /// <param name="catch">TThe callback when a specific exception is encountered</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public AsyncDelegateCommand<T> Catch<TException>(Action<TException> @catch)
            where TException : Exception
        {
            AddExceptionHandlerInternal<TException>(ex => @catch((TException)ex));
            return this;
        }

        /// <summary>
        /// Provides the ability to connect a delegate to catch exceptions encountered by CanExecute or the Execute methods of the DelegateCommand
        /// </summary>
        /// <param name="catch">The generic / default callback when an exception is encountered</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public AsyncDelegateCommand<T> Catch(Action<Exception> @catch)
        {
            AddExceptionHandlerInternal<Exception>(@catch);
            return this;
        }
    }
}

