
using System.Collections.Concurrent;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Base.Requests;

namespace Amethyst.Systems.Users.Common.Requests;

public sealed class CommonRequestProvider : IRequestProvider
{
    private ConcurrentDictionary<string, object> _requests = new();

    private int _curIndex;
    public int FindFreeIndex(string name)
    {
        int i = 0;
        while (i < int.MaxValue)
        {
            if (!_requests.ContainsKey($"{i}$_{name}"))
            {
                return i;
            }
            i++;
        }

        if (_curIndex < int.MaxValue)
        {
            return _curIndex++;
        }
        else
        {
            _curIndex = 0;
            return _curIndex++;
        }
    }

    public void AddRequest<TContext>(UserRequest<TContext> request) where TContext : class
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "Request cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Request name cannot be null or whitespace.", nameof(request));
        }

        _requests.AddOrUpdate($"{request.Index}$_{request.Name}", request, (key, oldValue) => request);
    }

    public void RemoveRequest<TContext>(UserRequest<TContext> request) where TContext : class
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request), "Request cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Request name cannot be null or whitespace.", nameof(request));
        }

        if (!_requests.TryRemove($"{request.Index}$_{request.Name}", out _))
        {
            AmethystLog.System.Warning($"CommonRequestProvider", $"Failed to remove request {request.Name} at index {request.Index}. Request not found.");
        }
        else
        {
            AmethystLog.System.Info($"CommonRequestProvider", $"Request {request.Name} at index {request.Index} removed successfully.");
        }
    }

    public void RemoveRequests(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Request name cannot be null or whitespace.", nameof(name));
        }

        var keysToRemove = _requests.Keys
            .Where(key => key.EndsWith($"$_{name}"))
            .ToList();

        foreach (var key in keysToRemove)
        {
            if (_requests.TryRemove(key, out _))
            {
                AmethystLog.System.Info($"CommonRequestProvider", $"Request {name} removed successfully.");
            }
            else
            {
                AmethystLog.System.Warning($"CommonRequestProvider", $"Failed to remove request {name}. Request not found.");
            }
        }
    }

    public UserRequest<TContext>? FindRequest<TContext>(string name, int index) where TContext : class
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Request name cannot be null or whitespace.", nameof(name));
        }

        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");
        }

        _requests.TryGetValue($"{index}$_{name}", out var request);
        return request == null ? null : (UserRequest<TContext>)request;
    }

    public IEnumerable<UserRequest<TContext>> FindRequests<TContext>(string name) where TContext : class
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Request name cannot be null or whitespace.", nameof(name));
        }

        return _requests
            .Where(kvp => kvp.Key.EndsWith($"$_{name}"))
            .Select(kvp => (UserRequest<TContext>)kvp.Value);
    }

    public IEnumerable<UserRequest<IAmethystUser>> GetRequests()
    {
        return _requests
            .Where(kvp => kvp.Value is UserRequest<IAmethystUser>)
            .Select(kvp => (UserRequest<IAmethystUser>)kvp.Value);
    }
}
