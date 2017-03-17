using LiteNetLib;
using ProjectCardboardBox;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LiteNetLib.Utils;
using System;

public class NetworkBehaviour : MonoBehaviour, INetEventListener
{
    NetManager server;

    List<SmartConnection> conns = new List<SmartConnection>();

    //DI GameBehaviour
    GameBehaviour game;

    void Start()
    {
        game = FindObjectOfType<GameBehaviour>();
        server = new NetManager(this, 10 /* maximum clients */, "SomeConnectionKey");
        server.Start(9050 /* port */);
        server.DiscoveryEnabled = true;
        server.UpdateTime = 15;
        Debug.Log("Server broadcasting");
    }

    void Update()
    {
        server.PollEvents();
    }

    void OnDestroy()
    {
        if (server != null)
            server.Stop();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        var sconn = new SmartConnection(peer);
        conns.Add(sconn);
        game.PlayerConnect(sconn);
        Debug.Log("Client connected as peer " + peer.ConnectId);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        var sconn = conns.Find(sc => sc.HasPeer(peer));
        conns.Remove(sconn);
        game.PlayerDisconnect(sconn);
        Debug.Log("Client disconnected as peer " + peer.ConnectId);
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        Debug.Log("Got error" + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        var str = reader.GetString(1000);
        Debug.Log("Got " + str);
        conns.First(c => c.HasPeer(peer)).OnCommandReceived(str);
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
        Debug.Log(string.Format("[Server] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, reader.GetString(1000)));
        NetDataWriter writer = new NetDataWriter();
        writer.Put("SERVER DISCOVERY RESPONSE :)");
        server.SendDiscoveryResponse(writer, remoteEndPoint);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }
}
