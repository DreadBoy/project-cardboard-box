using ProjectCardboardBox;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public interface INetworkConnection
{
    NetworkConnection conn { get; set; }
    SmartEvent<CommandArgs> CommandReceived { get; set; }
    PlayerBehaviour player { get; set; }
}

public class SmartConnection : INetworkConnection
{
    public NetworkConnection conn { get; set; }
    public SmartEvent<CommandArgs> CommandReceived { get; set; }
    public PlayerBehaviour player { get; set; }

    public SmartConnection(NetworkConnection connection)
    {
        CommandReceived = new SmartEvent<CommandArgs>();
        conn = connection;
        conn.RegisterHandler(MessageType.Command, OnCommandReceived);
    }


    void OnCommandReceived(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<StringMessage>().value;
        var comms = message.Split('|');
        var commands = comms.Select(p => new Command(p)).ToArray();
        foreach (var command in commands)
        {
            CommandReceived.RaiseEvent(new CommandArgs(command, player));
            Debug.Log("Received :" + command);
        }
    }
}

public class MockConnection : INetworkConnection
{
    public NetworkConnection conn { get; set; }
    public SmartEvent<CommandArgs> CommandReceived { get; set; }
    public PlayerBehaviour player { get; set; }

    public MockConnection()
    {
        CommandReceived = new SmartEvent<CommandArgs>();
        conn = new NetworkConnection();
    }
}
