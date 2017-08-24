using System;
using UnityEngine;
using ProjectCardboardBox;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;

public class NetworkBehaviour : MonoBehaviour, INetEventListener
{
    NetManager client;
    private bool connected;

    ICommandHandler commandHandler;
    public IFlowHandler flowHandler;

    void Start()
    {
    }

    public void StartSearching<T>(T screen) where T : ICommandHandler, IFlowHandler
    {
        commandHandler = screen;
        flowHandler = screen;

        if (client == null)
        {
            client = new NetManager(this, "SomeConnectionKey");
            client.Start();
            client.UpdateTime = 15;
        }
    }

    void Update()
    {
        if (client == null)
            return;
        client.PollEvents();
        if (!connected)
        {

            NetDataWriter writer = new NetDataWriter();
            writer.Put("CLIENT 1 DISCOVERY REQUEST");
            client.SendDiscoveryRequest(writer, 9050);
        }
    }

    public void JoinGame(NetEndPoint remoteEndPoint)
    {
        connected = true;
        client.Connect(remoteEndPoint);
    }

    void OnDestroy()
    {
        if (client != null)
            client.Stop();
    }

    public void ChangeHandler<T>(T handler) where T : ICommandHandler, IFlowHandler
    {
        commandHandler = handler;
        flowHandler = handler;
    }

    public void SendCommands(Command[] commands)
    {
        if (client == null)
            return;
        var message = string.Join("|", commands.Select(c => c.ToString()).ToArray());
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)MessageType.Command);
        writer.Put(message);
        client.SendToAll(writer, SendOptions.ReliableOrdered);
        Debug.Log("Sending commands " + message);
    }

    public void SendCommand(Command command)
    {
        if (client == null)
            return;
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)MessageType.Command);
        writer.Put(command.ToString());
        client.SendToAll(writer, SendOptions.ReliableOrdered);
        Debug.Log("Sending command " + command.ToString());
    }

    public void SendHint(Command[] commands)
    {
        if (client == null)
            return;
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)MessageType.Hint);
        var str = string.Join("|", commands.Select(c => c.ToString()).ToArray());
        writer.Put(str);
        client.SendToAll(writer, SendOptions.ReliableOrdered);
        Debug.Log("Sending hint " + str);
    }

    public void SendColour(string colour)
    {
        if (client == null)
            return;
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)MessageType.Colour);
        writer.Put(colour);
        client.SendToAll(writer, SendOptions.ReliableOrdered);
        Debug.Log("Sending colour " + colour);
    }

    public void SendNickname(string name)
    {
        if (client == null)
            return;
        NetDataWriter writer = new NetDataWriter();
        writer.Put((int)MessageType.Nickname);
        writer.Put(name);
        client.SendToAll(writer, SendOptions.ReliableOrdered);
        Debug.Log("Sending nickname " + name);
    }

    public void OnPeerConnected(NetPeer peer)
    {
        //Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (flowHandler != null)
            flowHandler.ServerDisconnected();
        connected = false;
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        //Debug.Log("Got error" + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        MessageType type;
        if (MessageParser.TryParse(ref reader, out type))
        {
            if (type == MessageType.Chip)
            {
                Debug.LogError("Client can't receive chip any more! Is server outdated?");
            }
            else if (type == MessageType.Command)
            {
                var message = reader.GetString(1000);
                Debug.Log("Got command " + message);
                var commands = message.Split('|').Select(c => new Command(c)).ToList();
                if (commandHandler != null)
                    commandHandler.ReceiveCommand(commands);
            }
        }
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
        //Debug.Log(string.Format("[Client] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, reader.GetString(1000)));
        if (messageType == UnconnectedMessageType.DiscoveryResponse)
        {
            if (flowHandler != null)
                flowHandler.GameFound(remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }
}
