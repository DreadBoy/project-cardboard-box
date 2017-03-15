using LiteNetLib;
using ProjectCardboardBox;
using System;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{


    public NetworkBehaviour networkBehaviour;
    public UIBehaviour uiBehaviour;
    public LobbyUIBehaviour lobbyUiBehaviour;

    NetEndPoint remoteEndPoint;

    public enum State
    {
        lobby,
        game
    }
    public State state = State.lobby;

    void Start()
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
        lobbyUiBehaviour.Waiting();
    }

    public void ReadyToPlay()
    {
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.READY));
        state = State.game;
        uiBehaviour.ChangeState(state);
    }
}
