using ProjectCardboardBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using System;

public class GameBehaviour : MonoBehaviour
{

    public int gridSize;
    public PlayerBehaviour playerPrefab;

    LobbyBehaviour lobby;
    GridBehaviour grid;
    PodiumBehaviour podium;
    NetworkBehaviour networkBehaviour;
    PlayersTurn playersTurn;


    public enum State
    {
        lobby,
        game,
        ending
    }
    public State state = State.lobby;

    List<PlayerBehaviour> players = new List<PlayerBehaviour>();
    List<INetworkConnection> connections = new List<INetworkConnection>();

    public SmartEvent<ChangeStateArgs> changeStateEvent = new SmartEvent<ChangeStateArgs>();
    public SmartEvent<GridCompiledArgs> gridCompiledEvent = new SmartEvent<GridCompiledArgs>();
    public SmartEvent<EventArgs> firstPlayerConnected = new SmartEvent<EventArgs>();
    public SmartEvent<EventArgs> playing = new SmartEvent<EventArgs>();
    public SmartEvent<EventArgs> lastPlayerDisconnected = new SmartEvent<EventArgs>();

    PlayerBehaviour playerOnTurn;

    void Awake()
    {
        lobby = FindObjectOfType<LobbyBehaviour>();
        grid = FindObjectOfType<GridBehaviour>();
        podium = FindObjectOfType<PodiumBehaviour>();
        networkBehaviour = FindObjectOfType<NetworkBehaviour>();
        playersTurn = FindObjectOfType<PlayersTurn>();
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
        connection.Player = player;
        lobby.AddPlayerToLobby(player);
        players.Add(player);
        connections.Add(connection);
        player.name = "Player " + players.Count;
        player.ReceiveNickname("Player " + players.Count);

        //subscribe to player's connection
        connection.CommandReceived.Event += CommandReceived_Event;
        connection.HintReceived.Event += HintReceived_Event;
        connection.ColourReceived.Event += ColourReceived_Event;
        connection.NicknameReceived.Event += NicknameReceived_Event;

        if (players.Count == 1)
            firstPlayerConnected.RaiseEvent(new EventArgs());

        return player;
    }

    private void CommandReceived_Event(object sender, CommandArgs e)
    {
        if (e.command.type == ProjectCardboardBox.Action.NEWGAME)
        {
            ChangeToState_NewGame();
            return;
        }
        else if (e.command.type == ProjectCardboardBox.Action.ENDTURN)
        {
            var index = players.IndexOf(e.player);
            index++;
            if (index >= connections.Count)
                index = 0;
            connections[index].Send(MessageType.Command, new Command(ProjectCardboardBox.Action.YOURTURN).ToString());
            playersTurn.NewTurn(connections[index].Player.name);
        }
        e.player.ReceiveCommand(e.command);
    }

    private void HintReceived_Event(object sender, HintArgs e)
    {
        e.player.ReceiveHint(e.hint);
    }

    private void ColourReceived_Event(object sender, StringArgs e)
    {
        e.player.ReceiveColour(e.data);
    }

    private void NicknameReceived_Event(object sender, StringArgs e)
    {
        e.player.ReceiveNickname(e.data);
    }

    public void PlayerDisconnect(INetworkConnection connection)
    {
        //Abort, abort!
        if (players.Count != connections.Count)
            return;

        var index = connections.IndexOf(connection);
        if (index < 0)
            return;

        var player = players[index];
        lobby.RemovePlayerFromLobby(player);
        players.RemoveAt(index);
        connections.RemoveAt(index);

        player.DestroyPlayer();

        if (players.Count == connections.Count && players.Count == 0)
        {
            state = State.lobby;
            changeStateEvent.RaiseEvent(new ChangeStateArgs(state));
            lastPlayerDisconnected.RaiseEvent(new EventArgs());
        }
    }

    private void Start()
    {
        changeStateEvent.RaiseEvent(new ChangeStateArgs(state));
    }

    void Update()
    {
        if (state == State.lobby && players.Count > 0 && players.Find(pl => pl.state != PlayerBehaviour.State.ready) == null)
        {
            ChangeToState_Game();
            playing.RaiseEvent(new EventArgs());
        }
        if (state == State.game && players.Count > 1 && players.Count(pl => pl.state != PlayerBehaviour.State.ending) == 1)
        {
            ChangeToState_Ending();
        }
    }

    public void ChangeToState_Ending()
    {
        connections.First(c => c.Player.state != PlayerBehaviour.State.ending).Send(MessageType.Command, new Command(ProjectCardboardBox.Action.VICTORY).ToString());
        connections.First(c => c.Player.state != PlayerBehaviour.State.ending).Player.YouWon();
        state = State.ending;
        changeStateEvent.RaiseEvent(new ChangeStateArgs(state));
    }

    public void ChangeToState_NewGame()
    {
        networkBehaviour.DropAllConnections();
        foreach (var player in players)
            podium.RemovePlayerFromPodium(player);
        players.Clear();
        connections.Clear();
        state = State.lobby;
        changeStateEvent.RaiseEvent(new ChangeStateArgs(state));
        lastPlayerDisconnected.RaiseEvent(null);
    }

    public void ChangeToState_Game()
    {
        foreach (var player in players)
        {
            player.state = PlayerBehaviour.State.ingame;
            lobby.RemovePlayerFromLobby(player);
            grid.AddPlayerToGrid(player);
        }
        state = State.game;
        changeStateEvent.RaiseEvent(new ChangeStateArgs(state));
        foreach (var conn in connections)
        {
            conn.Send(MessageType.Command, new Command(ProjectCardboardBox.Action.CONFIRMREADY).ToString());
        }
        playerOnTurn = players[0];
        connections[0].Send(MessageType.Command, new Command(ProjectCardboardBox.Action.YOURTURN).ToString());
        playersTurn.NewTurn(connections[0].Player.name);
    }

    internal void PlayerDied(PlayerBehaviour player)
    {
        connections.First(c => c.Player == player).Send(MessageType.Command, new Command(ProjectCardboardBox.Action.GAMEOVER).ToString());
    }
}
