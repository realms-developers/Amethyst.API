namespace Amethyst.Systems.Users.Base.Requests;

public delegate void RequestCallback<TContext>(UserRequest<TContext> request, TContext context) where TContext : class;
