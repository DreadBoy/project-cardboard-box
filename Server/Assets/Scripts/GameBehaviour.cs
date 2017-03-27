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
        player.name = "Player " + players.Count;
        connection.player = player;
        lobby.AddPlayerToLobby(player);
        players.Add(player);
        connections.Add(connection);

        //subscribe to player's connection
        connection.CommandReceived.Event += CommandReceived_Event;
        connection.HintReceived.Event += HintReceived_Event;
        player.chipsEvent.Event += ChipsEvent;

        return player;
    }

    private void ChipsEvent(object sender, ChipsArgs e)
    {
        var index = players.IndexOf(e.player);
        string str = string.Join("|", e.chips.Select(c => c.ToString()).ToArray());
        connections[index].Send(MessageType.Chip, str);
    }

    private void CommandReceived_Event(object sender, CommandArgs e)
    {
        e.player.ReceiveCommand(e.command);
    }

    private void HintReceived_Event(object sender, HintArgs e)
    {
        e.player.ReceiveHint(e.hint);
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
            changeStateEvent.RaiseEvent(new changeStateArgs(state));
        }
    }

    void Update()
    {
        if (state == State.lobby && players.Count > 0 && players.Find(pl => pl.state != PlayerBehaviour.State.ready) == null)
        {
            ChangeToState_Game();
        }
        if (state == State.game && players.Count > 1 && players.Count(pl => pl.state != PlayerBehaviour.State.dead) == 1)
        {
            ChangeToState_Ending();
        }
    }

    public void ChangeToState_Ending()
    {
        connections.First(c => c.player.state != PlayerBehaviour.State.dead).Send(MessageType.Command, new Command(ProjectCardboardBox.Action.VICTORY).ToString());
        foreach (var player in players)
        {
            player.state = PlayerBehaviour.State.ending;
            grid.RemovePlayerFromGrid(player);
        }
        state = State.ending;
        changeStateEvent.RaiseEvent(new changeStateArgs(state));
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
        changeStateEvent.RaiseEvent(new changeStateArgs(state));
        foreach (var conn in connections)
        {
            conn.Send(MessageType.Command, new Command(ProjectCardboardBox.Action.CONFIRMREADY).ToString());
        }

    }

    internal void playerDied(PlayerBehaviour player)
    {
        connections.First(c => c.player == player).Send(MessageType.Command, new Command(ProjectCardboardBox.Action.GAMEOVER).ToString());
    }
}
