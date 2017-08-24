using LiteNetLib;
using LiteNetLib.Utils;
using ProjectCardboardBox;
using System.Linq;
using System.Text;
using UnityEngine;
using System;

public interface INetworkConnection
{
    SmartEvent<CommandArgs> CommandReceived { get; set; }
    SmartEvent<HintArgs> HintReceived { get; set; }
    SmartEvent<StringArgs> ColourReceived { get; set; }
    SmartEvent<StringArgs> NicknameReceived { get; set; }
    PlayerBehaviour Player { get; set; }
    void Send(MessageType type, string message);
    void DropConnection(NetManager server);
}

public class SmartConnection : INetworkConnection
{
    public NetPeer Peer { get; set; }
    public SmartEvent<CommandArgs> CommandReceived { get; set; }
    public SmartEvent<HintArgs> HintReceived { get; set; }
    public SmartEvent<StringArgs> ColourReceived { get; set; }
    public SmartEvent<StringArgs> NicknameReceived { get; set; }
    public PlayerBehaviour Player { get; set; }

    public SmartConnection(NetPeer peer)
    {
        CommandReceived = new SmartEvent<CommandArgs>();
        HintReceived = new SmartEvent<HintArgs>();
        ColourReceived = new SmartEvent<StringArgs>();
        NicknameReceived = new SmartEvent<StringArgs>();
        this.Peer = peer;
    }

    public void Send(MessageType type, string message)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)type);
        writer.Put(message);
        Debug.Log("Sending " + message);
        Peer.Send(writer, SendOptions.ReliableOrdered);
    }

    public void DropConnection(NetManager server)
    {
        server.DisconnectPeer(Peer);
    }

    public bool HasPeer(NetPeer peer)
    {
        return peer == this.Peer;
    }

    public void OnMessageReceived(string message)
    {
        var messages = message.Split('|');
        var commands = messages.Select(p => new Command(p)).ToArray();
        foreach (var command in commands)
        {
            CommandReceived.RaiseEvent(new CommandArgs(command, Player));
        }
    }

    public void OnHintReceived(string hint)
    {
        HintReceived.RaiseEvent(new HintArgs(hint, Player));
    }

    public void OnColourReceived(string colour)
    {
        ColourReceived.RaiseEvent(new StringArgs(colour, Player));
    }

    public void OnNicknameReceived(string nickname)
    {
        NicknameReceived.RaiseEvent(new StringArgs(nickname, Player));
    }
}

public class MockConnection : INetworkConnection
{
    public SmartEvent<CommandArgs> CommandReceived { get; set; }
    public SmartEvent<HintArgs> HintReceived { get; set; }
    public SmartEvent<StringArgs> ColourReceived { get; set; }
    public SmartEvent<StringArgs> NicknameReceived { get; set; }
    public PlayerBehaviour Player { get; set; }

    public MockConnection()
    {
        CommandReceived = new SmartEvent<CommandArgs>();
        HintReceived = new SmartEvent<HintArgs>();
        ColourReceived = new SmartEvent<StringArgs>();
        NicknameReceived = new SmartEvent<StringArgs>();
    }

    public void Send(MessageType type, string message)
    {
        Debug.Log("(Mock connection) Sending network message " + type.ToString() + ": " + message.ToString());
    }

    public void DropConnection(NetManager server)
    {
        Debug.Log("(Mock connection) Dropping connection");
    }
}
