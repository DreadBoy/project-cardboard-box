using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour {

    GameBehaviour game;
    UIBehaviour uiBehaviour;

	void Start () {
        game = FindObjectOfType<GameBehaviour>();
        uiBehaviour = FindObjectOfType<UIBehaviour>();

        game.state = GameBehaviour.State.game;
        uiBehaviour.ChangeState(game.state);
    }
	
	void Update () {
		
	}
}
