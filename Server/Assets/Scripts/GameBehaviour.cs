using Cardboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour {

    public Game game;
    public int gridSize;
    public PlayerBehaviour playerPrefab;

    void Awake()
    {
        game = new Game(gridSize);

        game.createPlayerEvent.Event += CreatePlayerEvent_Event;

        var player1 = new Player();
        game.AddPlayer(player1, 5, 5);
        //player1.MovePlayer(3, 0);
        player1.RotatePlayer(1);
    }

    private void CreatePlayerEvent_Event(object sender, CreatePlayerArgs e)
    {
        var pl = Instantiate(playerPrefab);
        pl.player = e.player;
    }

    void Start () {
		
	}
	
	void Update () {
		
	}
}
