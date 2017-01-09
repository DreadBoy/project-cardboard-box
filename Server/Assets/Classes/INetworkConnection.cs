using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public interface INetworkConnection
{
    SmartEvent<CommandArgs> CommandReceived { get; set; }
    PlayerBehaviour player { get; set; }
}

public class SmartConnection : INetworkConnection
{
    public NetworkConnection conn;
    public SmartEvent<CommandArgs> CommandReceived { get; set; }
    public PlayerBehaviour player { get; set; }

    public SmartConnection(NetworkConnection connection)
    {
        CommandReceived = new SmartEvent<CommandArgs>();
        conn = connection;
        conn.RegisterHandler(49, OnCommandReceived);
    }


    void OnCommandReceived(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<StringMessage>().value;
        var parts = message.Split('|').Select<string, int>(p => int.Parse(p)).ToArray();
        var command = new Command((Command.Type)parts[0], parts[1], parts[2]);
        CommandReceived.RaiseEvent(new CommandArgs(command, player));
        Debug.Log(command);
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
}
