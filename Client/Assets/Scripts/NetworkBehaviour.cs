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

    public GameBehaviour gameBehaviour { get; set; }
    public GameUIBehaviour gameUiBehaviour;

    void Start()
    {
        gameBehaviour = FindObjectOfType<GameBehaviour>();
        if (gameUiBehaviour == null)
            gameUiBehaviour = FindObjectOfType<GameUIBehaviour>();

        client = new NetManager(this, "SomeConnectionKey");
        client.Start();
        client.UpdateTime = 15;
    }

    void Update()
    {
        client.PollEvents();
        if (!connected)
        {

            NetDataWriter writer = new NetDataWriter();
            writer.Put("CLIENT 1 DISCOVERY REQUEST");
            client.SendDiscoveryRequest(writer, 9050);
        }
    }

    void OnDestroy()
    {
        if (client != null)
            client.Stop();
    }

    public void JoinGame(NetEndPoint remoteEndPoint)
    {
        connected = true;
        client.Connect(remoteEndPoint);
    }

    public void OnHandReceived(string message)
    {
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

    public void OnPeerConnected(NetPeer peer)
    {
        //Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        gameBehaviour.GameLost();
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        //Debug.Log("Got error" + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        MessageType type;
        if(MessageParser.TryParse(ref reader, out type))
        {
            if(type == MessageType.Chip)
            {
                var message = reader.GetString(1000);
                Debug.Log("Got chips " + message);
                var chips = message.Split('|').Select(c => new Chip(c)).ToList();
                gameUiBehaviour.OnHandReceived(chips);
            }
            else if(type == MessageType.Command)
            {
                var message = reader.GetString(1000);
                Debug.Log("Got command " + message);
                var commands = message.Split('|').Select(c => new Command(c)).ToList();
                gameBehaviour.OnCommandReceived(commands);
            }
        }
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
        //Debug.Log(string.Format("[Client] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, reader.GetString(1000)));
        if (messageType == UnconnectedMessageType.DiscoveryResponse)
        {
            gameBehaviour.GameFound(remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }
}
