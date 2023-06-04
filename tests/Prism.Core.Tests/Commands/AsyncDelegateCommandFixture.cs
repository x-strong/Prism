using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Xunit;

namespace Prism.Core.Tests.Commands
{
    public class AsyncDelegateCommandFixture
    {
        [Fact]
        public void WhenConstructedWithDelegate_InitializesValues()
        {
            var actual = new AsyncDelegateCommand(() => default);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task CannotExecuteWhileExecuting()
        {
            var tcs = new TaskCompletionSource<object>();
            var command = new AsyncDelegateCommand(async () => await tcs.Task);

            Assert.True(command.CanExecute());
            var task = command.Execute();
            Assert.False(command.CanExecute());
            tcs.SetResult("complete");
            await task;
            Assert.True(command.CanExecute());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldExecuteCommandAsynchronously()
        {
            // Arrange
            bool executed = false;
            var tcs = new TaskCompletionSource<object>();
            var command = new AsyncDelegateCommand(async (_) =>
            {
                await tcs.Task;
                executed = true;
            });

            // Act
            var task = command.Execute();
            Assert.False(executed);
            tcs.SetResult("complete");
            await task;

            // Assert
            Assert.True(executed);
        }

        [Fact]
        public async Task ExecuteAsync_WithCancellationToken_ShouldExecuteCommandAsynchronously()
        {
            // Arrange
            bool executed = false;
            var command = new AsyncDelegateCommand(async (cancellationToken) =>
            {
                await Task.Delay(100, cancellationToken); // Simulate some asynchronous operation
                executed = true;
            });

            // Act
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(50); // Cancel after 50 milliseconds
                await command.Execute(cancellationTokenSource.Token);
            }

            // Assert
            Assert.True(executed);
        }
    }
}
