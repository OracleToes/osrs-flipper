using System.Collections.Concurrent;

namespace OsrsFlipper;

public sealed class FlipperThread : IDisposable
{
    private readonly float _updateIntervalSeconds;
    private readonly Thread _thread;
    private Flipper _flipper = null!;
    private volatile bool _shouldStop;
    
    public event Action? OnDumpsUpdated;
    public ConcurrentQueue<ItemDump> Dumps { get; } = new();


    public FlipperThread(float updateIntervalSeconds = 20f)
    {
        _updateIntervalSeconds = updateIntervalSeconds;
        _thread = new Thread(Run);
    }
    
    
    public void Start()
    {
        _thread.Start();
    }
    
    
    public void Stop()
    {
        _shouldStop = true;
        _thread.Join();
        Dispose();
    }


    private async void Run()
    {
        _flipper = await Flipper.Create();
        
        while (!_shouldStop)
        {
            await _flipper.RefreshCache();
            List<ItemDump> dumps = await _flipper.FindDumps();
            
            if (dumps.Count > 0)
            {
                foreach (ItemDump dump in dumps)
                {
                    Dumps.Enqueue(dump);
                }
                OnDumpsUpdated?.Invoke();
            }

            await Task.Delay(TimeSpan.FromSeconds(_updateIntervalSeconds));
        }
    }


    public void Dispose()
    {
        _flipper.Dispose();
    }
}