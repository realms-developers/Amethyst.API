namespace Amethyst.Security.Threshold.Counting;

internal sealed class CounterThreshold<T> : IThreshold<T>, IDisposable where T : Enum
{
    private static readonly Timer _timer = new Timer((obj) => _timerCallback?.Invoke(obj), null, Timeout.Infinite, 1000);
    private static event TimerCallback? _timerCallback;

    private readonly int[] _limits;
    private readonly int[] _counters;
    private readonly bool _reset;

    public CounterThreshold(int[] limits, bool reset)
    {
        _limits = limits;
        _counters = new int[Convert.ToByte(Enum.GetValues(typeof(T)).Length, System.Globalization.CultureInfo.InvariantCulture)];

        _reset = reset;

        if (_reset)
        {
            _timerCallback += (obj) =>
            {
                for (int i = 0; i < _counters.Length; i++)
                {
                    _counters[i] = 0;
                }
            };
        }
    }

    public bool Fire(T index)
    {
        byte idx = Convert.ToByte(index, System.Globalization.CultureInfo.InvariantCulture);
        if (_counters[idx] < _limits[idx])
        {
            _counters[idx]++;
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        if (_reset)
        {
            _timerCallback -= (obj) =>
            {
                for (int i = 0; i < _counters.Length; i++)
                {
                    _counters[i] = 0;
                }
            };
        }
    }
}
