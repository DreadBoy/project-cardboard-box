using LiteNetLib;
using LiteNetLib.Utils;
using ProjectCardboardBox;
using System.Linq;
using System.Text;
using UnityEngine;

public interface INetworkConnection
{
    SmartEvent<CommandArgs> CommandReceived { get; set; }
    PlayerBehaviour player { get; set; }
    void Send(MessageType type, string message);
}

public class SmartConnection : INetworkConnection
{
    public NetPeer peer { get; set; }
    public SmartEvent<CommandArgs> CommandReceived { get; set; }
    public PlayerBehaviour player { get; set; }

    public SmartConnection(NetPeer peer)
    {
        CommandReceived = new SmartEvent<CommandArgs>();
        this.peer = peer;
    }

    public void Send(MessageType type, string message)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)type);
        writer.Put(message);
        Debug.Log("Sending " + message);
        peer.Send(writer, SendOptions.ReliableOrdered);
    }

    public bool HasPeer(NetPeer peer)
    {
        return peer == this.peer;
    }

    public void OnMessageReceived(string message)
    {
        var messages = message.Split('|');
        var commands = messages.Select(p => new Command(p)).ToArray();
        foreach (var command in commands)
        {
            CommandReceived.RaiseEvent(new CommandArgs(command, player));
        }
    }
}

public class MockConnection : INetworkConnection
{
    public SmartEvent<CommandArgs> CommandReceived { get; set; }
    public PlayerBehaviour player { get; set; }

    public MockConnection()
    {
        CommandReceived = new SmartEvent<CommandArgs>();
    }

    public void Send(MessageType type, string message)
    {
        Debug.Log("(Mock connection) Sending network message " + type.ToString() + ": " + message.ToString());
    }
}
