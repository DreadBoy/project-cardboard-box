using ProjectCardboardBox;
using System;
using UnityEngine;

public class GameBehaviour : MonoBehaviour {


    public NetworkBehaviour networkBehaviour;
    public UIBehaviour uiBehaviour;
    public LobbyUIBehaviour lobbyUiBehaviour;

    string address;

    public enum State
    {
        lobby,
        game
    }
    public State state = State.lobby;

    void Start () {
        networkBehaviour = FindObjectOfType<NetworkBehaviour>();
        uiBehaviour = FindObjectOfType<UIBehaviour>();
        lobbyUiBehaviour = FindObjectOfType<LobbyUIBehaviour>();
    }
	
	void Update () {
		
	}

    public void GameFound(string address)
    {
        lobbyUiBehaviour.Found();
        this.address = address;
    }

    public void GameLost()
    {
        state = State.lobby;
        uiBehaviour.ChangeState(state);
    }

    public void JoinGame()
    {
        if (networkBehaviour.JoinGame(address))
            lobbyUiBehaviour.Waiting();
    }

    public void ReadyToPlay()
    {
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.READY));
        state = State.game;
        uiBehaviour.ChangeState(state);
    }
}
