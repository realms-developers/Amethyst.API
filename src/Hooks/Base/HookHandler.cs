using Amethyst.Hooks.Context;

namespace Amethyst.Hooks;

public delegate void HookHandler<TArgs>(in TArgs args, HookResult<TArgs> result);
