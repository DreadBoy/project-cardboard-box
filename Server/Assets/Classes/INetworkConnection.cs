using ProjectCardboardBox;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public interface INetworkConnection
{
    SmartEvent<CommandArgs> CommandReceived { get; set; }
    PlayerBehaviour player { get; set; }
    void Send(short msgType, MessageBase message);
}

public class SmartConnection : INetworkConnection
{
    private NetworkConnection conn { get; set; }
    public SmartEvent<CommandArgs> CommandReceived { get; set; }
    public PlayerBehaviour player { get; set; }

    public SmartConnection(NetworkConnection connection)
    {
        CommandReceived = new SmartEvent<CommandArgs>();
        conn = connection;
        conn.RegisterHandler(MessageType.Command, OnCommandReceived);
        Debug.Log("Registered handler for connection " + conn.connectionId);
    }

    public void Send(short msgType, MessageBase message)
    {
        conn.Send(msgType, message);
    }

    public bool HasConnection(NetworkConnection conn)
    {
        return conn == this.conn;
    }

    void OnCommandReceived(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<StringMessage>().value;
        var comms = message.Split('|');
        var commands = comms.Select(p => new Command(p)).ToArray();
        foreach (var command in commands)
        {
            CommandReceived.RaiseEvent(new CommandArgs(command, player));
            Debug.Log("Received :" + command + " from connection " + conn.connectionId);
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

    public void Send(short msgType, MessageBase message)
    {
        Debug.Log("(Mock connection) Sending network message " + msgType + ": " + message.ToString());
    }
}
