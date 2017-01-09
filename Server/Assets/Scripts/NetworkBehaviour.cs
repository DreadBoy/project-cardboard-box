using Cardboard;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class NetworkBehaviour : NetworkManager
{
    NetworkDiscovery discovery;

    List<SmartConnection> conns = new List<SmartConnection>();

    //DI GameBehaviour
    GameBehaviour game;

    void Start()
    {
        game = FindObjectOfType<GameBehaviour>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        discovery = gameObject.AddComponent<NetworkDiscovery>() as NetworkDiscovery;
        discovery.showGUI = false;
        discovery.Initialize();
        discovery.StartAsServer();
        Debug.Log("Server broadcasting");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        //new client connected
        base.OnServerConnect(conn);
        var sconn = new SmartConnection(conn);
        game.PlayerConnect(sconn);
        conns.Add(sconn);
        conn.Send(48, new StringMessage("Hello client!"));
        Debug.Log("Client connected");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        var sconn = conns.Find(sc => sc.conn == conn);
        game.PlayerDisconnect(sconn);
        Debug.Log("Client disconnected");
    }

}
