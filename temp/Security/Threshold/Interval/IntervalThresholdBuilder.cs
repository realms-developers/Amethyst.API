namespace Amethyst.Security.Threshold.Interval;

public sealed class IntervalThresholdBuilder<T> : IThresholdBuilder<T> where T : Enum
{
    private readonly int _length = Convert.ToByte(Enum.GetValues(typeof(T)).Length, System.Globalization.CultureInfo.InvariantCulture);
    private TimeSpan[] _interval = null!;

    public IntervalThresholdBuilder<T> SetIntervals(TimeSpan interval)
    {
        _interval = new TimeSpan[_length];
        for (int i = 0; i < _interval.Length; i++)
        {
            _interval[i] = interval;
        }
        return this;
    }

    public IntervalThresholdBuilder<T> SetIntervals(IEnumerable<TimeSpan> interval)
    {
        _interval = interval.ToArray();

        if (_interval.Length != _length)
        {
            throw new ArgumentException($"Interval length must be {_length}");
        }

        return this;
    }

    public IntervalThresholdBuilder<T> SetIntervals(IEnumerable<int> interval)
    {
        _interval = interval.Select(p => TimeSpan.FromMilliseconds(p)).ToArray();

        if (_interval.Length != _length)
        {
            throw new ArgumentException($"Interval length must be {_length}");
        }

        return this;
    }

    public IThreshold<T> Build()
    {
        return new IntervalThreshold<T>(_interval);
    }
}
