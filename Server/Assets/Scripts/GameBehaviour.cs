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
        //FindObjectOfType<ServerTest>().StartServer();

        game.spawnPlayerOnGridEvent.Event += spawnPlayerOnGridEvent;

        var conn1 = new MockConnection();
        var player1 = game.PlayerConnect(conn1);
        conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Command.Type.READY), player1));


        //var player1 = new Player();
        //game.SpawnPlayerOnGrid(player1, 5, 5);
        ////player1.MovePlayer(3, 0);
        //player1.RotatePlayer(1);
    }

    private void spawnPlayerOnGridEvent(object sender, spawnPlayerOnGridArgs e)
    {
        var pl = Instantiate(playerPrefab);
        pl.player = e.player;
    }

    void Start () {
		
	}
	
	void Update () {
        game.Update(Time.deltaTime);
	}
}
