using LiteNetLib;
using ProjectCardboardBox;
using System;
using UnityEngine;
using System.Collections.Generic;

public class GameBehaviour : MonoBehaviour
{


    public NetworkBehaviour networkBehaviour;
    public UIBehaviour uiBehaviour;
    public LobbyUIBehaviour lobbyUiBehaviour;

    NetEndPoint remoteEndPoint;

    public enum State
    {
        lobby,
        game,
        victory,
        gameover
    }
    public State state = State.lobby;

    void OnEnable()
    {
        networkBehaviour = FindObjectOfType<NetworkBehaviour>();
        uiBehaviour = FindObjectOfType<UIBehaviour>();
        lobbyUiBehaviour = FindObjectOfType<LobbyUIBehaviour>();
    }

    void Update()
    {

    }

    public void GameFound(NetEndPoint remoteEndPoint)
    {
        if (this.remoteEndPoint != null)
            if (this.remoteEndPoint.Host == remoteEndPoint.Host)
                return;
        lobbyUiBehaviour.Found();
        this.remoteEndPoint = remoteEndPoint;
    }

    public void GameLost()
    {
        state = State.lobby;
        uiBehaviour.ChangeState(state);
    }

    public void JoinGame()
    {
        networkBehaviour.JoinGame(remoteEndPoint);
        lobbyUiBehaviour.Readying();
    }

    public void ReadyToPlay()
    {
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.READY));
        lobbyUiBehaviour.Waiting();
    }

    public void NotReadyToPlay()
    {
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.NOTREADY));
        lobbyUiBehaviour.Readying();
    }

    internal void OnCommandReceived(List<Command> commands)
    {
        foreach (var command in commands)
        {
            if (command.type == ProjectCardboardBox.Action.CONFIRMREADY)
            {
                state = State.game;
                uiBehaviour.ChangeState(state);
            }
            if(command.type == ProjectCardboardBox.Action.GAMEOVER || command.type == ProjectCardboardBox.Action.VICTORY)
            {
                if (command.type == ProjectCardboardBox.Action.GAMEOVER)
                    state = State.gameover;
                if (command.type == ProjectCardboardBox.Action.VICTORY)
                    state = State.victory;
                uiBehaviour.ChangeState(state);
            }
        }
    }
}
