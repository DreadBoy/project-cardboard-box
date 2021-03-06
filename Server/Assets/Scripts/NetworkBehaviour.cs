﻿using LiteNetLib;
using ProjectCardboardBox;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using LiteNetLib.Utils;
using System;

public class NetworkBehaviour : MonoBehaviour, INetEventListener
{
    public NetManager server;

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

    public void DropAllConnections()
    {
        foreach (var conn in conns)
            conn.DropConnection(server);
        conns.Clear();
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
        Debug.Log("Got error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        MessageType type;
        if (MessageParser.TryParse(ref reader, out type))
        {
            if (type == MessageType.Chip)
            {
                Debug.LogError("Server can't receive chips!");
            }
            else if (type == MessageType.Command)
            {
                var message = reader.GetString(1000);
                Debug.Log("Got commands " + message);
                conns.First(c => c.HasPeer(peer)).OnMessageReceived(message);
            }
            else if (type == MessageType.Hint)
            {
                var message = reader.GetString(1000);
                Debug.Log("Got hint " + message);
                conns.First(c => c.HasPeer(peer)).OnHintReceived(message);
            }
            else if (type == MessageType.Colour)
            {
                var colour = reader.GetString(1000);
                conns.First(c => c.HasPeer(peer)).OnColourReceived(colour);
            }
            else if (type == MessageType.Nickname)
            {
                var nickname = reader.GetString(1000);
                conns.First(c => c.HasPeer(peer)).OnNicknameReceived(nickname);
            }
        }
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
