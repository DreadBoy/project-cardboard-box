using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ClientTest : NetworkManager
{
    NetworkConnection conn;
    OverriddenNetworkDiscovery discovery;

    public int key;

    void Start()
    {
        discovery = gameObject.AddComponent<OverriddenNetworkDiscovery>() as OverriddenNetworkDiscovery;
        discovery.showGUI = false;
        discovery.Initialize();
        discovery.broadcastKey = key;
        discovery.StartAsClient();
        discovery.OnDiscovered += OnDiscovered;
    }

    private void OnDiscovered(string address)
    {
        Status("Discovered: " + address + ", connecting");
        discovery.StopBroadcast();
        try {
            networkAddress = address;
            StartClient();
        }
        catch (Exception e)
        {
            Status("Error: " + e.Message);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Status("Client connected");
        base.OnClientConnect(conn);
        this.conn = conn;
        conn.RegisterHandler(48, OnHandReceived);
    }

    void OnHandReceived(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<StringMessage>();
        Status(message.value);
    }

    Text Status()
    {
        return GameObject.Find("Status").GetComponent<Text>();
    }


    void Status(string text)
    {
        GameObject.Find("Status").GetComponent<Text>().text = text;
        Debug.Log(text);
    }
}
