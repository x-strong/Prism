using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Properties;

namespace Prism.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> whose delegates do not take any parameters for <see cref="Execute()"/> and <see cref="CanExecute()"/>.
    /// </summary>
    /// <see cref="DelegateCommandBase"/>
    /// <see cref="DelegateCommand{T}"/>
    public class DelegateCommand : DelegateCommandBase
    {
        Action _executeMethod;
        Func<bool> _canExecuteMethod;

        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Action"/> to invoke on execution.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute(object)"/> is called.</param>
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="DelegateCommand"/> with the <see cref="Action"/> to invoke on execution
        /// and a <see langword="Func" /> to query for determining if the command can execute.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to invoke when <see cref="ICommand.Execute"/> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{TResult}"/> to invoke when <see cref="ICommand.CanExecute"/> is called</param>
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base()
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException(nameof(executeMethod), Resources.DelegateCommandDelegatesCannotBeNull);

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        ///<summary>
        /// Executes the command.
        ///</summary>
        public void Execute()
        {
            try
            {
                _executeMethod();
            }
            catch (Exception ex)
            {
                if (!HandleException(ex))
                    throw;
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
                return _canExecuteMethod();
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
        protected override void Execute(object parameter)
        {
            Execute();
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
        /// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
        /// <param name="propertyExpression">The property expression. Example: ObservesProperty(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommand ObservesProperty<T>(Expression<Func<T>> propertyExpression)
        {
            ObservesPropertyInternal(propertyExpression);
            return this;
        }

        /// <summary>
        /// Observes a property that is used to determine if this command can execute, and if it implements INotifyPropertyChanged it will automatically call DelegateCommandBase.RaiseCanExecuteChanged on property changed notifications.
        /// </summary>
        /// <param name="canExecuteExpression">The property expression. Example: ObservesCanExecute(() => PropertyName).</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommand ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
        {
            _canExecuteMethod = canExecuteExpression.Compile();
            ObservesPropertyInternal(canExecuteExpression);
            return this;
        }

        /// <summary>
        /// Provides the ability to connect a delegate to catch exceptions encountered by CanExecute or the Execute methods of the DelegateCommand
        /// </summary>
        /// <param name="catch">TThe callback when a specific exception is encountered</param>
        /// <returns>The current instance of DelegateCommand</returns>
        public DelegateCommand Catch<TException>(Action<TException> @catch)
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
        public DelegateCommand Catch(Action<Exception> @catch)
        {
            AddExceptionHandlerInternal<Exception>(@catch);
            return this;
        }

        /// <summary>
        /// Create a new <see cref="DelegateCommand"/>
        /// </summary>
        /// <param name="executeMethod"></param>
        /// <returns>An instance of the <see cref="DelegateCommand"/>.</returns>
        public static DelegateCommand Create(Action executeMethod) =>
            new DelegateCommand(executeMethod);

        public static DelegateCommand Create(Action executeMethod, Func<bool> canExecute) =>
            new DelegateCommand(executeMethod, canExecute);

        public static DelegateCommand<T> Create<T>(Action<T> executeMethod) =>
            new DelegateCommand<T>(executeMethod);

        public static DelegateCommand<T> Create<T>(Action<T> executeMethod, Func<T, bool> canExecute) =>
            new DelegateCommand<T>(executeMethod, canExecute);

        public static AsyncDelegateCommand CreateFromTask(Func<Task> executeMethod) =>
            new AsyncDelegateCommand(executeMethod);

        public static AsyncDelegateCommand CreateFromTask(Func<CancellationToken, Task> executeMethod) =>
            new AsyncDelegateCommand(executeMethod);

        public static AsyncDelegateCommand CreateFromTask(Func<Task> executeMethod, Func<bool> canExecute) =>
            new AsyncDelegateCommand(executeMethod, canExecute);

        public static AsyncDelegateCommand CreateFromTask(Func<CancellationToken, Task> executeMethod, Func<bool> canExecute) =>
            new AsyncDelegateCommand(executeMethod, canExecute);

        public static AsyncDelegateCommand<T> CreateFromTask<T>(Func<T, Task> executeMethod) =>
            new AsyncDelegateCommand<T>(executeMethod);

        public static AsyncDelegateCommand<T> CreateFromTask<T>(Func<T, CancellationToken, Task> executeMethod) =>
            new AsyncDelegateCommand<T>(executeMethod);

        public static AsyncDelegateCommand<T> CreateFromTask<T>(Func<T, Task> executeMethod, Func<T, bool> canExecute) =>
            new AsyncDelegateCommand<T>(executeMethod, canExecute);

        /// <summary>
        /// Creates a new <see cref="AsyncDelegateCommand{T}" /> from a <see cref="CreateFromTask(Func{CancellationToken, Task})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executeMethod"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public static AsyncDelegateCommand<T> CreateFromTask<T>(Func<T, CancellationToken, Task> executeMethod, Func<T, bool> canExecute) =>
            new AsyncDelegateCommand<T>(executeMethod, canExecute);
    }
}
