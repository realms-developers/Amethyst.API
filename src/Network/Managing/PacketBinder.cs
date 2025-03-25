using Amethyst.Network.Packets;

namespace Amethyst.Network.Managing;

public sealed class PacketBinder
{
    internal PacketBinder(NetworkInstance instance)
    {
        _instance = instance;
    }

    private NetworkInstance _instance;

    public void AddInPacket(PacketTypes packetId, PacketHandler<IncomingPacket> handler) 
        => AddInPacket((byte)packetId, handler);
    public bool AddInPacket(byte packetId, PacketHandler<IncomingPacket> handler)
    {
        if (_instance.incoming[packetId].Contains(handler)) return false;

        _instance.incoming[packetId].Insert(0, handler);
        return true;
    }

    public void AddInModule(ModuleTypes moduleId, PacketHandler<IncomingModule> handler) 
        => AddInModule((byte)moduleId, handler);
    public bool AddInModule(byte moduleId, PacketHandler<IncomingModule> handler)
    {
        if (_instance.incomingModules[moduleId].Contains(handler)) return false;

        _instance.incomingModules[moduleId].Insert(0, handler);
        return true;
    }
    
    public void AddOutPacket(PacketTypes packetId, PacketHandler<OutcomingPacket> handler) 
        => AddOutPacket((byte)packetId, handler);
    public bool AddOutPacket(byte packetId, PacketHandler<OutcomingPacket> handler)
    {
        if (_instance.outcoming[packetId].Contains(handler)) return false;

        _instance.outcoming[packetId].Insert(0, handler);
        return true;
    }

    public void RemoveInPacket(PacketTypes packetId, PacketHandler<IncomingPacket> handler) 
        => RemoveInPacket((byte)packetId, handler);
    public bool RemoveInPacket(byte packetId, PacketHandler<IncomingPacket> handler)
    {
        return _instance.incoming[packetId].Remove(handler);
    }

    public void RemoveInModule(ModuleTypes moduleId, PacketHandler<IncomingModule> handler) 
        => RemoveInModule((byte)moduleId, handler);
    public bool RemoveInModule(byte moduleId, PacketHandler<IncomingModule> handler)
    {
        return _instance.incomingModules[moduleId].Remove(handler);
    }
    
    public void RemoveOutPacket(PacketTypes packetId, PacketHandler<OutcomingPacket> handler) 
        => RemoveOutPacket((byte)packetId, handler);
    public bool RemoveOutPacket(byte packetId, PacketHandler<OutcomingPacket> handler)
    {
        return _instance.outcoming[packetId].Remove(handler);
    }

    public void ReplaceInPacket(PacketTypes packetId, PacketHandler<IncomingPacket>? handler) 
        => ReplaceInPacket((byte)packetId, handler);
    public void ReplaceInPacket(byte packetId, PacketHandler<IncomingPacket>? handler)
    {
        _instance.incomingReplace[packetId] = handler;
    }
    

    public void ReplaceInModule(ModuleTypes moduleId, PacketHandler<IncomingModule>? handler) 
        => ReplaceInModule((byte)moduleId, handler);
    public void ReplaceInModule(byte moduleId, PacketHandler<IncomingModule>? handler)
    {
        _instance.incomingModuleReplace[moduleId] = handler;
    }
    
    public void ReplaceOutPacket(PacketTypes packetId, PacketHandler<OutcomingPacket>? handler) 
        => ReplaceOutPacket((byte)packetId, handler);
    public void ReplaceOutPacket(byte packetId, PacketHandler<OutcomingPacket>? handler)
    {
        _instance.outcomingReplace[packetId] = handler;
    }
}