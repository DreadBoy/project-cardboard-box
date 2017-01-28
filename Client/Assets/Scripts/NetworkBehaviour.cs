using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using ProjectCardboardBox;
using System.Linq;

public class NetworkBehaviour : NetworkManager
{
    NetworkConnection connection;
    OverriddenNetworkDiscovery discovery;

    public int key;

    GameBehaviour game;
    public GameUIBehaviour gameUiBehaviour;

    void Start()
    {
        discovery = gameObject.AddComponent<OverriddenNetworkDiscovery>() as OverriddenNetworkDiscovery;
        discovery.showGUI = false;
        discovery.Initialize();
        discovery.broadcastKey = key;
        discovery.StartAsClient();
        discovery.OnDiscovered += OnDiscovered;

        game = FindObjectOfType<GameBehaviour>();
        if (gameUiBehaviour == null)
            gameUiBehaviour = FindObjectOfType<GameUIBehaviour>();
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
        conn.RegisterHandler(MessageType.Hand, OnHandReceived);
        conn.RegisterHandler(MessageType.Handshake, OnHandshakeReceived);
    }

    public void OnHandReceived(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<StringMessage>().value;
        Debug.Log(message);
        var chips = message.Split('|').Select(c => new Chip(c)).ToList();
        gameUiBehaviour.OnHandReceived(chips);
        Debug.Log(message);
    }

    public void OnHandshakeReceived(NetworkMessage netMsg)
    {
        var message = netMsg.ReadMessage<StringMessage>().value;
        Debug.Log("Handshake :" + message);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        StopClient();
        game.GameLost();
        discovery.Initialize();
        discovery.StartAsClient();
    }

    public void SendCommands(Command[] commands)
    {
        var message = string.Join("|", commands.Select(c => c.ToString()).ToArray());
        Debug.Log(message);
        if (connection != null)
            connection.Send(MessageType.Command, new StringMessage(message));
    }

    public void SendCommand(Command command)
    {
        connection.Send(MessageType.Command, new StringMessage(command.ToString()));
    }
}
