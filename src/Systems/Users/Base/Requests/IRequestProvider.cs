namespace Amethyst.Systems.Users.Base.Requests;

public interface IRequestProvider
{
    int FindFreeIndex(string name);

    void AddRequest<TContext>(UserRequest<TContext> request) where TContext : class;

    void RemoveRequest<TContext>(UserRequest<TContext> request) where TContext : class;

    void RemoveRequests(string name);

    UserRequest<TContext>? FindRequest<TContext>(string name, int index) where TContext : class;
    IEnumerable<UserRequest<TContext>> FindRequests<TContext>(string name) where TContext : class;

    IEnumerable<UserRequest<IAmethystUser>> GetRequests();
}
