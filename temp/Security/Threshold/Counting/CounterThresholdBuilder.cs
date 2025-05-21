namespace Amethyst.Security.Threshold.Counting;

public sealed class CounterThresholdBuilder<T> : IThresholdBuilder<T> where T : Enum
{
    private int[] _limits = null!;
    private bool _reset;

    private readonly int _length = Convert.ToByte(Enum.GetValues(typeof(T)).Length, System.Globalization.CultureInfo.InvariantCulture);

    public CounterThresholdBuilder<T> SetLimits(int limit)
    {
        _limits = new int[_length];

        for (int i = 0; i < _limits.Length; i++)
        {
            _limits[i] = limit;
        }

        return this;
    }

    public CounterThresholdBuilder<T> SetReset(bool value)
    {
        _reset = value;
        return this;
    }

    public IThreshold<T> Build()
    {
        return new CounterThreshold<T>(_limits, _reset);
    }
}
