namespace Amethyst.Hooks.Base;

public delegate void HookHandler<TArgs>(in TArgs args, HookResult<TArgs> result);
