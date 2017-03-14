using LiteNetLib;
using ProjectCardboardBox;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Linq;
using LiteNetLib.Utils;

public class NetworkBehaviour : MonoBehaviour
{
    EventBasedNetListener listener = new EventBasedNetListener();
    NetManager server;

    List<SmartConnection> conns = new List<SmartConnection>();

    //DI GameBehaviour
    GameBehaviour game;

    void Start()
    {
        game = FindObjectOfType<GameBehaviour>();
        server = new NetManager(listener, 10 /* maximum clients */, "SomeConnectionKey");
        server.DiscoveryEnabled = true;
        server.Start(9050 /* port */);
        Debug.Log("Server broadcasting");

        listener.PeerConnectedEvent += peer =>
        {
            var sconn = new SmartConnection(peer);
            game.PlayerConnect(sconn);
            conns.Add(sconn);
            //conn.Send(MessageType.Handshake, new StringMessage("Hello client!"));
            Debug.Log("Client connected as peer " + peer.ConnectId);
            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, SendOptions.ReliableOrdered);             // Send with reliability
        };

        listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
        {
            var sconn = conns.Find(sc => sc.HasPeer(peer));
            conns.Remove(sconn);
            game.PlayerDisconnect(sconn);
            Debug.Log("Client disconnected as peer " + peer.ConnectId);
        };

        listener.NetworkReceiveEvent += (peer, reader) =>
        {
            Debug.Log("Got: " + string.Join(", ", reader.Data.Select(b => b.ToString()).ToArray()));
        };

        listener.NetworkReceiveUnconnectedEvent += (NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType) =>
        {
            Debug.Log(string.Format("[Server] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, reader.GetString(100)));
            NetDataWriter writer = new NetDataWriter();
            writer.Put("SERVER DISCOVERY RESPONSE :)");
            server.SendDiscoveryResponse(writer, remoteEndPoint);
        };
    }

    void Update()
    {
        server.PollEvents();
    }

}
