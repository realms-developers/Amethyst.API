namespace Amethyst.Security.Threshold;

internal sealed class IntervalThreshold<T> : IThreshold<T> where T : Enum
{
    private readonly TimeSpan[] _interval;
    private DateTime[] _lastFireTime;

    public IntervalThreshold(TimeSpan[] interval)
    {
        _interval = interval;
        _lastFireTime = new DateTime[Convert.ToByte(Enum.GetValues(typeof(T)).Length, System.Globalization.CultureInfo.InvariantCulture)];
        for (int i = 0; i < _lastFireTime.Length; i++)
        {
            _lastFireTime[i] = DateTime.MinValue;
        }
    }

    public bool Fire(T index)
    {
        var now = DateTime.UtcNow;
        var idx = Convert.ToByte(index, System.Globalization.CultureInfo.InvariantCulture);
        if (now - _lastFireTime[idx] >= _interval[idx])
        {
            _lastFireTime[idx] = now;
            return true;
        }
        return false;
    }
}
