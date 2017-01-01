using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour {

    public Game game;
    public int gridSize;
    public GameObject playerPrefab;

    void Awake()
    {
        game = new Game(gridSize);

        game.createPlayerEvent.Event += CreatePlayerEvent_Event;


        game.AddPlayer(new Player());
        game.AddPlayer(new Player());
    }

    private void CreatePlayerEvent_Event(object sender, CreatePlayerArgs e)
    {

    }

    void Start () {
		
	}
	
	void Update () {
		
	}
}
