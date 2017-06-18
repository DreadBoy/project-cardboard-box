using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LobbyUI : MonoBehaviour {

	GameBehaviour gameBehaviour = null;
    Animator animator = null;

	void Start () {
        if (gameBehaviour == null)
            gameBehaviour = FindObjectOfType<GameBehaviour>();
        if (animator == null)
            animator = GetComponent<Animator>();

        gameBehaviour.firstPlayerConnected.Event += FirstPlayerConnected;
        gameBehaviour.lastPlayerDisconnected.Event += LastPlayerDisconnected;
        gameBehaviour.playing.Event += Playing;
    }

    private void FirstPlayerConnected(object sender, EventArgs e)
    {
        animator.SetTrigger("FirstPlayerConnected");
    }

    private void LastPlayerDisconnected(object sender, EventArgs e)
    {
        animator.SetTrigger("LastPlayerDisconnected");
    }

    private void Playing(object sender, EventArgs e)
    {
        animator.SetTrigger("Playing");
    }

    void Update () {
		
	}
}
