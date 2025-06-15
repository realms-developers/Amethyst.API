using Amethyst.Systems.Chat.Base.Misc.Base;

namespace Amethyst.Systems.Chat.Base.Misc;

public sealed class MiscMessageProvider<T> where T : class
{
    internal MiscMessageProvider(IMiscMessageRenderer<T>? renderer)
    {
        Renderer = renderer;
    }

    public IMiscMessageRenderer<T>? Renderer { get; private set; }
    public IReadOnlyDictionary<string, IMiscOutput<T>> Outputs => _outputsInternal;

    private readonly Dictionary<string, IMiscOutput<T>> _outputsInternal = [];

    public void Invoke(T ctx)
    {
        if (Renderer is null)
        {
            return;
        }

        MiscRenderedMessage<T>? message = Renderer.Render(ctx);

        if (message is null)
        {
            return;
        }

        foreach (KeyValuePair<string, IMiscOutput<T>> output in _outputsInternal)
        {
            output.Value.OutputMessage(message);
        }
    }

    public void SetRenderer(IMiscMessageRenderer<T>? renderer)
    {
        if (renderer is not null && Renderer is not null)
        {
            throw new InvalidOperationException("Renderer is already set.");
        }

        Renderer = renderer;
    }

    public void AddOutput(IMiscOutput<T> output)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (_outputsInternal.ContainsKey(output.Name))
        {
            throw new InvalidOperationException($"Output with name '{output.Name}' already exists.");
        }

        _outputsInternal[output.Name] = output;
    }

    public void RemoveOutput(IMiscOutput<T> output)
    {
        ArgumentNullException.ThrowIfNull(output);

        if (!_outputsInternal.Remove(output.Name))
        {
            throw new InvalidOperationException($"Output with name '{output.Name}' does not exist.");
        }
    }

    public void ClearOutputs()
    {
        _outputsInternal.Clear();
    }
}
