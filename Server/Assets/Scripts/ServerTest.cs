using Cardboard;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ServerTest : NetworkManager
{
    NetworkDiscovery discovery;
    
    float timer = 0;
    float count = 0;

    List<SmartConnection> conns = new List<SmartConnection>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        discovery = gameObject.AddComponent<NetworkDiscovery>() as NetworkDiscovery;
        discovery.showGUI = false;
        discovery.Initialize();
        discovery.StartAsServer();
        Debug.Log("server broadcasting");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        //new client connected
        base.OnServerConnect(conn);
        var sconn = new SmartConnection(conn);
        FindObjectOfType<GameBehaviour>().game.PlayerConnect(sconn);
        conns.Add(sconn);
        conn.Send(48, new StringMessage("Hello client!"));
        Debug.Log("Client connected");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        var sconn = conns.Find(sc => sc.conn == conn);
        FindObjectOfType<GameBehaviour>().game.PlayerDisconnect(sconn);
        Debug.Log("Client disconnected");
    }
}
