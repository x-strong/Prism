using System;
using System.Threading.Tasks;

namespace Prism.Regions;

public struct RegionNavigationCallback
{
    private readonly MulticastDelegate _delegate;

    public RegionNavigationCallback(MulticastDelegate @delegate)
    {
        _delegate = @delegate;
    }

    public static RegionNavigationCallback Empty => new RegionNavigationCallback(() => { });

    public async void Invoke(NavigationResult result) =>
        await InvokeAsync(result);

    public async Task InvokeAsync(NavigationResult result)
    {
        switch(_delegate)
        {
            case Action<NavigationResult> actionResult:
                actionResult(result);
                break;
            case Action action:
                action();
                break;
            case Func<Task> asyncTask:
                await asyncTask();
                break;
            case Func<NavigationResult, Task> asyncResultTask:
                await asyncResultTask(result);
                break;
        }
    }
}
