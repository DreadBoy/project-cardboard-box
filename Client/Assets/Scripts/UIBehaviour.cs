using System;
using System.Collections.Generic;
using ProjectCardboardBox;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{

    public GameUIBehaviour gameState;
    public GameOverUIBehaviour gameOverState;
    public NetworkBehaviour networkBehaviour;

    MonoBehaviour[] allStates;

    void OnEnable()
    {
        if (gameState == null)
            gameState = FindObjectOfType<GameUIBehaviour>();
        if (gameOverState == null)
            gameOverState = FindObjectOfType<GameOverUIBehaviour>();
        if (networkBehaviour == null)
            networkBehaviour = FindObjectOfType<NetworkBehaviour>();

        allStates = new MonoBehaviour[] { gameState, gameOverState };

        foreach (var state in allStates)
            state.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    public void ChangeState(GameBehaviour.State state)
    {
        foreach (var s in allStates)
            s.gameObject.SetActive(false);
        if (state == GameBehaviour.State.game)
        {
            gameState.gameObject.SetActive(true);
            gameOverState.gameObject.SetActive(false);
            networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.REQUESTCHIPS, 8));
        }
        if (state == GameBehaviour.State.lobby)
        {
            gameState.ClearAll();
        }
        if (state == GameBehaviour.State.gameover || state == GameBehaviour.State.victory)
        {
            gameOverState.gameObject.SetActive(true);
            if (state == GameBehaviour.State.gameover)
                gameOverState.Loss();
            else
                gameOverState.Win();
        }
    }
}
