using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour {

    public Game game;
    public int gridSize;

    void Awake()
    {
        game = new Game(gridSize);
        game.AddPlayer(new Player());
        game.AddPlayer(new Player());
    }

	void Start () {
		
	}
	
	void Update () {
		
	}
}
