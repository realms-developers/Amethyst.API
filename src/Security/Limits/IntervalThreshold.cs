namespace Amethyst.Security;

public class IntervalThreshold : IThreshold
{
    public IntervalThreshold(int maxCounters, bool alwaysReset)
    {
        _alwaysReset = alwaysReset;
        _counters = new DateTime[maxCounters];
        _intervals = new int[maxCounters];
    }

    private DateTime[] _counters;
    private int[] _intervals;
    private bool _alwaysReset;
    private bool _disposed;

    public bool Fire(int index)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_intervals[index] == 0) return false;

        if (_counters[index].AddMilliseconds(_intervals[index]) > DateTime.UtcNow)
        {
            if (_alwaysReset)
                _counters[index] = DateTime.UtcNow;

            return true;
        }

        _counters[index] = DateTime.UtcNow;

        return false;
    }

    public void Setup(int index, int max)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _intervals[index] = max;
    }

    public void Dispose()
    {
        _disposed = true;
        _counters = [];
        _intervals = [];

        GC.SuppressFinalize(this);
    }
}
