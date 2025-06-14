namespace TosuSyncService;

using System.Diagnostics;
using System.Threading;

public class ConcurrencyDetector
{
    private object _locker = new object();
    public int EnterCount { get; private set; }
    public int MaxCount { get; set; } = 1;
    public event EventHandler? RateLimited;
    public event EventHandler? Entered;
    public event EventHandler? Exited;
    
    public void Enter()
    {
        lock(_locker)
        {
            if(EnterCount >= MaxCount)
            {
                RateLimited?.Invoke(this, EventArgs.Empty);
                return;
            }

            EnterCount++;
            Entered?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Exit()
    {
        lock(_locker)
        {
            if(EnterCount - 1 < 0)
            {
                throw new InvalidOperationException();
            }

            EnterCount--;
            Exited?.Invoke(this, EventArgs.Empty);
        }
    }
}
