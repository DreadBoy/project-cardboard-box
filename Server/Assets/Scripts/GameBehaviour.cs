using Cardboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour {
    
    public int gridSize;
    public PlayerBehaviour playerPrefab;

    //DI LobbyBehaviour
    LobbyBehaviour lobby;
    GridBehaviour grid;


    public enum State
    {
        lobby,
        game,
        ending
    }
    public State state = State.lobby;

    List<PlayerBehaviour> players = new List<PlayerBehaviour>();
    List<INetworkConnection> connections = new List<INetworkConnection>();

    public SmartEvent<changeStateArgs> changeStateEvent = new SmartEvent<changeStateArgs>();
    public SmartEvent<GridCompiledArgs> gridCompiledEvent = new SmartEvent<GridCompiledArgs>();

    void Awake()
    {
        lobby = FindObjectOfType<LobbyBehaviour>();
        grid = FindObjectOfType<GridBehaviour>();
    }

    public bool PlayerConnect(INetworkConnection connection)
    {
        //Abort, abort!
        if (players.Count != connections.Count)
            return false;

        if (state != State.lobby)
            return false;

        //Create players and add them to lobby
        var player = Instantiate(playerPrefab) as PlayerBehaviour;
        lobby.AddPlayerToLobby(player);
        players.Add(player);
        connections.Add(connection);
        connection.player = player;

        //subscribe to player's connection
        connection.CommandReceived.Event += CommandReceived_Event;

        return player;
    }

    private void CommandReceived_Event(object sender, CommandArgs e)
    {
        e.player.ReceiveCommand(e.command);
    }

    public void PlayerDisconnect(INetworkConnection connection)
    {
        //Abort, abort!
        if (players.Count != connections.Count)
            return;

        var index = connections.IndexOf(connection);

        players.RemoveAt(index);
        connections.RemoveAt(index);

    }

    float readyTimer = 0;

    void Update()
    {
        if (state == State.lobby && players.Find(pl => pl.state != PlayerBehaviour.State.ready) == null)
        {
            if (readyTimer < 3)
                readyTimer += Time.deltaTime;
            if (readyTimer >= 3)
            {
                ChangeToState_Game();
            }
        }
    }

    public void ChangeToState_Game()
    {
        readyTimer = 0;
        players.ForEach(delegate (PlayerBehaviour player)
        {
            player.state = PlayerBehaviour.State.ingame;
            lobby.RemovePlayerFromLobby(player);
            grid.AddPlayerToGrid(player);
        });
        state = State.game;
        changeStateEvent.RaiseEvent(new changeStateArgs(State.game));
    }
}
