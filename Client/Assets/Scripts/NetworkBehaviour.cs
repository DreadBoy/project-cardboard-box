using System;
using UnityEngine;
using ProjectCardboardBox;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;

public class NetworkBehaviour : MonoBehaviour
{
    EventBasedNetListener listener = new EventBasedNetListener();
    NetManager client;

    public GameBehaviour gameBehaviour { get; set; }
    public GameUIBehaviour gameUiBehaviour;

    void Start()
    {
        gameBehaviour = FindObjectOfType<GameBehaviour>();
        if (gameUiBehaviour == null)
            gameUiBehaviour = FindObjectOfType<GameUIBehaviour>();

        client = new NetManager(listener, "SomeConnectionKey");
        client.Start();

        listener.NetworkReceiveEvent += (fromPeer, dataReader) =>
        {
            Debug.Log("We got: " + dataReader.GetString(100 /* max length of string */));
            OnHandReceived(dataReader.GetString(10000));
        };

        listener.NetworkReceiveUnconnectedEvent += (NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) =>
        {
            Debug.Log(string.Format("[Client] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, reader.GetString(100)));
            if (messageType == UnconnectedMessageType.DiscoveryResponse)
            {
                gameBehaviour.GameFound(remoteEndPoint);
                //conn.RegisterHandler(MessageType.Hand, OnHandReceived);
                //conn.RegisterHandler(MessageType.Handshake, OnHandshakeReceived);
            }
        };
        
        NetDataWriter writer = new NetDataWriter();
        writer.Put("CLIENT 1 DISCOVERY REQUEST");
        client.SendDiscoveryRequest(writer, 9050);
    }

    void Update()
    {
        client.PollEvents();
    }

    public void JoinGame(NetEndPoint remoteEndPoint)
    {
        client.Connect(remoteEndPoint);
    }

    public void OnHandReceived(string message)
    {
        var chips = message.Split('|').Select(c => new Chip(c)).ToList();
        gameUiBehaviour.OnHandReceived(chips);
    }

    public void SendCommands(Command[] commands)
    {
        var message = string.Join("|", commands.Select(c => c.ToString()).ToArray());
        NetDataWriter writer = new NetDataWriter();
        writer.Put(message);
        client.SendToAll(writer, SendOptions.ReliableOrdered);
    }

    public void SendCommand(Command command)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put(command.ToString());
        client.SendToAll(writer, SendOptions.ReliableOrdered);
    }
}
