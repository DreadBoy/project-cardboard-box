using LiteNetLib;
using ProjectCardboardBox;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    public NetworkBehaviour networkBehaviour;
    public InitNickname initNickname;

    GameLobby lobby;
    GameMain game;
    NetEndPoint endpoint;

    List<DoAfterTimeout> events = new List<DoAfterTimeout>();

    void Start()
    {
        if (networkBehaviour == null)
            networkBehaviour = FindObjectOfType<NetworkBehaviour>();
        if (initNickname == null)
            initNickname = FindObjectOfType<InitNickname>();

        networkBehaviour.StartSearching(lobby);

        endpoint = new NetEndPoint("127.0.0.1", 1337);

        lobby = FindObjectOfType<GameLobby>();
        game = FindObjectOfType<GameMain>();

        lobby.GameFound(endpoint);

        //networkBehaviour.SendCommands(new Command[] { new Command("MOVE:2"), new Command("TURN:2") });

        //gameUiBehaviour.OnHandReceived(new List<Chip>() {
        //    new Chip("Action:MOVE"),
        //    new Chip("Number:5"),
        //    new Chip("Action:TURN"),
        //    new Chip("Number:1"),
        //    new Chip("Action:TURN"),
        //    new Chip("Number:3"),
        //    new Chip("Action:MOVE"),
        //    new Chip("Number:9")
        //});

        //initNickname.ExitToLeft();

        events.Add(new DoAfterTimeout(1, () =>
        {
            lobby.ReceiveCommand(new List<Command>() { new Command(Action.CONFIRMREADY) });
        }));

        events.Add(new DoAfterTimeout(5, () =>
        {
            game.ReceiveCommand(new List<Command>() { new Command(Action.YOURTURN) });
            //game.ReceiveCommand(new List<Command>() { new Command(Action.VICTORY) });
            //networkBehaviour.flowHandler.ServerDisconnected();
        }));


    }
    void Update()
    {
        foreach (var ev in events)
        {
            ev.Update(Time.deltaTime);
        }
    }
}
