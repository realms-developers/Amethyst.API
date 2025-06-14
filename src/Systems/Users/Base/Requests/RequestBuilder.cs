namespace Amethyst.Systems.Users.Base.Requests;

public sealed class RequestBuilder<TContext> where TContext : class
{
    private readonly string _name;
    private readonly int _index;
    private readonly TContext _context;
    private bool _autoRemove = true;
    private RequestCallback<TContext>? _acceptedCallback;
    private RequestCallback<TContext>? _rejectedCallback;
    private RequestCallback<TContext>? _cancelledCallback;
    private RequestCallback<TContext>? _timeoutCallback;
    private TimeSpan? _removeIn;

    public RequestBuilder(string name, int index, TContext context)
    {
        _name = name;
        _index = index;
        _context = context;
    }

    public RequestBuilder<TContext> WithAutoRemove(bool value)
    {
        _autoRemove = value;
        return this;
    }

    public RequestBuilder<TContext> WithRemoveIn(TimeSpan removeIn)
    {
        _removeIn = removeIn;
        return this;
    }

    public RequestBuilder<TContext> WithOnAccepted(RequestCallback<TContext> callback)
    {
        _acceptedCallback = callback;
        return this;
    }

    public RequestBuilder<TContext> WithOnRejected(RequestCallback<TContext> callback)
    {
        _rejectedCallback = callback;
        return this;
    }

    public RequestBuilder<TContext> WithOnCancelled(RequestCallback<TContext> callback)
    {
        _cancelledCallback = callback;
        return this;
    }

    public RequestBuilder<TContext> WithOnTimeout(RequestCallback<TContext> callback)
    {
        if (_removeIn == null)
        {
            throw new InvalidOperationException("RemoveIn must be set before setting a timeout callback.");
        }
        _timeoutCallback = callback;
        return this;
    }

    public UserRequest<TContext> Build()
    {
        return new UserRequest<TContext>(_name, _index, _context, _removeIn, _autoRemove,
            _timeoutCallback, _acceptedCallback, _rejectedCallback, _cancelledCallback);
    }
}
