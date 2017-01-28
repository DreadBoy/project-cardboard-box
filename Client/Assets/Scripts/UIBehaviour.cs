using System;
using System.Collections.Generic;
using ProjectCardboardBox;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{

    public LobbyUIBehaviour lobbyState;
    public GameUIBehaviour gameState;
    public NetworkBehaviour networkBehaviour;

    void Start()
    {
        if (lobbyState == null)
            lobbyState = FindObjectOfType<LobbyUIBehaviour>();
        if (gameState == null)
            gameState = FindObjectOfType<GameUIBehaviour>();
        if (networkBehaviour == null)
            networkBehaviour = FindObjectOfType<NetworkBehaviour>();
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
        if (state == GameBehaviour.State.game)
        {
            lobbyState.gameObject.SetActive(false);
            gameState.gameObject.SetActive(true);
            networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.REQUESTCHIPS, 8));
        }
        if (state == GameBehaviour.State.lobby)
        {
            lobbyState.gameObject.SetActive(true);
            gameState.gameObject.SetActive(false);
            lobbyState.Searching();
            gameState.ClearAll();
        }
    }
}
