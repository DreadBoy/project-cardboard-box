using System;
using UnityEngine;

public class GameBehaviour : MonoBehaviour {


    public NetworkBehaviour network;
    public UIBehaviour uiBehaviour;

    string address;

    public enum State
    {
        lobby,
        game
    }
    public State state = State.lobby;

    void Start () {
        network = FindObjectOfType<NetworkBehaviour>();
        uiBehaviour = FindObjectOfType<UIBehaviour>();
    }
	
	void Update () {
		
	}

    public void GameFound(string address)
    {
        uiBehaviour.GameFound();
        this.address = address;
    }

    public void GameLost()
    {
        state = State.lobby;
        uiBehaviour.ChangeState(state);
    }

    public void JoinGame()
    {
        if (network.JoinGame(address))
        {
            state = State.game;
            uiBehaviour.ChangeState(state);
        }
    }
}
