using Amethyst.Systems.Chat.Misc.Base;

namespace Amethyst.Systems.Chat.Misc;

public sealed class MiscMessageProvider<T> where T : class
{
    internal MiscMessageProvider(IMiscMessageRenderer<T>? renderer)
    {
        Renderer = renderer;
    }

    public IMiscMessageRenderer<T>? Renderer { get; private set; }
    public IReadOnlyDictionary<string, IMiscOutput<T>> Outputs => _outputsInternal;

    private Dictionary<string, IMiscOutput<T>> _outputsInternal = new();

    public void Invoke(T ctx)
    {
        if (Renderer is null)
        {
            return;
        }

        var message = Renderer.Render(ctx);

        if (message is null)
        {
            return;
        }

        foreach (var output in _outputsInternal)
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
