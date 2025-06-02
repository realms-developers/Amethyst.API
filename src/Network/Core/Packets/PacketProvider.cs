using Amethyst.Server.Entities.Players;
using Amethyst.Network.Core.Delegates;
using System.Reflection.Emit;
using System.Reflection;

namespace Amethyst.Network.Core.Packets;

internal sealed class PacketProvider<TPacket>
{
    internal List<PacketHook<TPacket>> _securityHandlers = new List<PacketHook<TPacket>>();
    internal List<PacketHook<TPacket>> _handlers = new List<PacketHook<TPacket>>();
    internal PacketHook<TPacket>? _mainHandler;
    internal IPacket<TPacket> _packet = Activator.CreateInstance<IPacket<TPacket>>();
    internal bool _wasHooked;

    internal PacketHook<TPacket>[] _ivkSecurityHandlers = [];
    internal PacketHook<TPacket>[] _ivkHandlers = [];

    public void Hookup()
    {
        if (_wasHooked)
            return;

        _wasHooked = true;

        var method = CreateDynamicInvoke();
        var invoker = method.CreateDelegate(typeof(PacketInvokeHandler));

        NetworkManager.InvokeHandlers[(byte)_packet.PacketID] = invoker as PacketInvokeHandler;
    }

    internal void SetMainHandler(PacketHook<TPacket>? hook)
    {
        _mainHandler = hook;
    }

    internal void RegisterHandler(PacketHook<TPacket> handler, int priority = 0)
    {
        if (_handlers.Contains(handler))
            return;

        _handlers.Add(handler);
        _handlers.Sort((x, y) => priority.CompareTo(0));

        _ivkHandlers = _handlers.ToArray();
    }
    internal void UnregisterHandler(PacketHook<TPacket> handler)
    {
        _handlers.Remove(handler);

        _ivkHandlers = _handlers.ToArray();
    }

    internal void RegisterSecurityHandler(PacketHook<TPacket> handler, int priority = 0)
    {
        if (_securityHandlers.Contains(handler))
            return;

        _securityHandlers.Add(handler);
        _securityHandlers.Sort((x, y) => priority.CompareTo(0));

        _ivkSecurityHandlers = _securityHandlers.ToArray();
    }
    internal void UnregisterSecurityHandler(PacketHook<TPacket> handler)
    {
        _securityHandlers.Remove(handler);

        _ivkSecurityHandlers = _securityHandlers.ToArray();
    }

    // internal void Invoke(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore)
    // {
    //     if (_handlers.Count == 0)
    //         return;

    //     var packet = _packet.Deserialize(data);

    //     for (int i = 0; i < _ivkSecurityHandlers.Length; i++)
    //     {
    //         _ivkSecurityHandlers[i](plr, ref packet, data, ref ignore);
    //         if (ignore)
    //             return;
    //     }

    //     for (int i = 0; i < _ivkHandlers.Length; i++)
    //     {
    //         _ivkHandlers[i](plr, ref packet, data, ref ignore);
    //     }

    //     if (ignore)
    //         return;

    //     _mainHandler?.Invoke(plr, ref packet, data, ref ignore);
    // }

    internal DynamicMethod CreateDynamicInvoke()
    {
        DynamicMethod dynamicInvoke = new DynamicMethod(
            $"Invoke_{typeof(TPacket).Name}",
            typeof(void),
            new[] { typeof(PlayerEntity), typeof(ReadOnlySpan<byte>), typeof(bool).MakeByRefType() },
            typeof(PacketProvider<TPacket>),
            true
        );
        ILGenerator il = dynamicInvoke.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_2);

        MethodInfo? deserializeMethod = typeof(IPacket<TPacket>).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static);
        if (deserializeMethod == null)
        {
            throw new InvalidOperationException($"Deserialize method not found for packet type {typeof(TPacket).Name}");
        }
        il.Emit(OpCodes.Call, deserializeMethod);
        LocalBuilder packetLocal = il.DeclareLocal(typeof(TPacket));
        il.Emit(OpCodes.Stloc, packetLocal);

        LocalBuilder iLocal = il.DeclareLocal(typeof(int));

        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Stloc, iLocal);

        Label loopCheckLabel = il.DefineLabel();
        Label loopBodyLabel = il.DefineLabel();
        Label afterLoopLabel = il.DefineLabel();

        il.Emit(OpCodes.Br_S, loopCheckLabel);

        il.MarkLabel(loopBodyLabel);

        il.Emit(OpCodes.Ldarg_0);
        FieldInfo ivkSecurityHandlersField = typeof(PacketProvider<TPacket>).GetField("_ivkSecurityHandlers", BindingFlags.NonPublic | BindingFlags.Instance)!;
        il.Emit(OpCodes.Ldfld, ivkSecurityHandlersField);
        il.Emit(OpCodes.Ldloc, iLocal);
        il.Emit(OpCodes.Ldelem_Ref);

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldloca_S, packetLocal);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_2);

        MethodInfo invokeMethod = typeof(PacketHook<TPacket>).GetMethod("Invoke")!;
        il.Emit(OpCodes.Callvirt, invokeMethod);

        il.Emit(OpCodes.Ldarg_2);
        il.Emit(OpCodes.Ldind_I1);
        il.Emit(OpCodes.Brtrue_S, afterLoopLabel);

        il.Emit(OpCodes.Ldloc, iLocal);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Stloc, iLocal);

        il.MarkLabel(loopCheckLabel);

        il.Emit(OpCodes.Ldloc, iLocal);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, ivkSecurityHandlersField);
        il.Emit(OpCodes.Ldlen);
        il.Emit(OpCodes.Conv_I4);
        il.Emit(OpCodes.Blt_S, loopBodyLabel);

        il.MarkLabel(afterLoopLabel);

        LocalBuilder jLocal = il.DeclareLocal(typeof(int));

        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Stloc, jLocal);

        Label handlerLoopCheckLabel = il.DefineLabel();
        Label handlerLoopBodyLabel = il.DefineLabel();
        Label handlerAfterLoopLabel = il.DefineLabel();

        il.Emit(OpCodes.Br_S, handlerLoopCheckLabel);

        il.MarkLabel(handlerLoopBodyLabel);

        il.Emit(OpCodes.Ldarg_0);
        FieldInfo ivkHandlersField = typeof(PacketProvider<TPacket>).GetField("_ivkHandlers", BindingFlags.NonPublic | BindingFlags.Instance)!;
        il.Emit(OpCodes.Ldfld, ivkHandlersField);
        il.Emit(OpCodes.Ldloc, jLocal);
        il.Emit(OpCodes.Ldelem_Ref);

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldloca_S, packetLocal);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_2);

        il.Emit(OpCodes.Callvirt, invokeMethod);

        il.Emit(OpCodes.Ldloc, jLocal);
        il.Emit(OpCodes.Ldc_I4_1);
        il.Emit(OpCodes.Add);
        il.Emit(OpCodes.Stloc, jLocal);

        il.MarkLabel(handlerLoopCheckLabel);

        il.Emit(OpCodes.Ldloc, jLocal);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, ivkHandlersField);
        il.Emit(OpCodes.Ldlen);
        il.Emit(OpCodes.Conv_I4);
        il.Emit(OpCodes.Blt_S, handlerLoopBodyLabel);

        il.MarkLabel(handlerAfterLoopLabel);

        Label endLabel = il.DefineLabel();
        il.Emit(OpCodes.Ldarg_2);
        il.Emit(OpCodes.Brfalse_S, endLabel);
        il.Emit(OpCodes.Ret);
        il.MarkLabel(endLabel);

        // if (ignore) return;
        il.Emit(OpCodes.Ldarg_2);
        il.Emit(OpCodes.Ldind_I1);
        Label skipMainHandlerLabel = il.DefineLabel();
        il.Emit(OpCodes.Brtrue_S, skipMainHandlerLabel);

        il.Emit(OpCodes.Ldarg_0);
        FieldInfo mainHandlerField = typeof(PacketProvider<TPacket>).GetField("_mainHandler", BindingFlags.NonPublic | BindingFlags.Instance)!;
        il.Emit(OpCodes.Ldfld, mainHandlerField);

        Label endOfMethodLabel = il.DefineLabel();
        il.Emit(OpCodes.Dup);
        il.Emit(OpCodes.Brfalse_S, endOfMethodLabel);

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldloca_S, packetLocal);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_2);

        MethodInfo mainInvokeMethod = typeof(PacketHook<TPacket>).GetMethod("Invoke")!;
        il.Emit(OpCodes.Callvirt, mainInvokeMethod);

        il.MarkLabel(endOfMethodLabel);
        il.Emit(OpCodes.Pop);

        il.MarkLabel(skipMainHandlerLabel);
        il.Emit(OpCodes.Ret);

        return dynamicInvoke;
    }
}
