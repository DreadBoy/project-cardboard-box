using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using ProjectCardboardBox;

public class NetworkBehaviour : NetworkManager
{
    NetworkConnection connection;
    OverriddenNetworkDiscovery discovery;

    public int key;

    GameBehaviour game;

    void Start()
    {
        discovery = gameObject.AddComponent<OverriddenNetworkDiscovery>() as OverriddenNetworkDiscovery;
        discovery.showGUI = false;
        discovery.Initialize();
        discovery.broadcastKey = key;
        discovery.StartAsClient();
        discovery.OnDiscovered += OnDiscovered;

        game = FindObjectOfType<GameBehaviour>();
    }


    private void OnDiscovered(string address)
    {
        discovery.StopBroadcast();
        game.GameFound(address);
    }

    public bool JoinGame(string address)
    {
        try
        {
            networkAddress = address;
            StartClient();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        this.connection = conn;
        conn.RegisterHandler(48, OnHandReceived);
    }

    public void OnHandReceived(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<StringMessage>();
        Debug.Log(message.value);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        StopClient();
        game.GameLost();
    }
}
