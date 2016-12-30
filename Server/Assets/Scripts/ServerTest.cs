using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ServerTest : NetworkManager
{
    List<NetworkConnection> connections = new List<NetworkConnection>();
    NetworkDiscovery discovery;
    
    float timer = 0;
    float count = 0;

    Text Status()
    {
        return GameObject.Find("Status").GetComponent<Text>();
    }

    void Status(string text)
    {
        GameObject.Find("Status").GetComponent<Text>().text = text;
        Debug.Log(text);
    }

    void Start()
    {
        StartServer();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        discovery = gameObject.AddComponent<NetworkDiscovery>() as NetworkDiscovery;
        discovery.showGUI = false;
        discovery.Initialize();
        discovery.StartAsServer();
        Status("server broadcasting");
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        connections.Add(conn);
        conn.Send(48, new StringMessage("Hello client!"));
        Status("Client connected");
        conn.RegisterHandler(49, OnCommandReceived);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        connections.Remove(conn);
        Status("Client disconnected");
    }

    void OnCommandReceived(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<StringMessage>();
        Status(message.value);
    }

    void Update()
    {
        if (connections.Count == 0)
            return;
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer -= 1;
            foreach (var conn in connections)
            {
                conn.Send(48, new StringMessage("Count: " + count));
            }
            Status("Sending " + count);
            count++;
        }
    }
}
