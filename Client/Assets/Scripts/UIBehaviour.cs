using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{

    public Button joinButton;
    public Text status;

    public LobbyUIBehaviour lobbyState;
    public GameUIBehaviour gameState;

    void Start()
    {
        if (lobbyState == null)
            lobbyState = FindObjectOfType<LobbyUIBehaviour>();
        if (gameState == null)
            gameState = FindObjectOfType<GameUIBehaviour>();
    }

    void Update()
    {

    }

    public void ChangeState(GameBehaviour.State state)
    {
        if (state == GameBehaviour.State.game)
        {
            lobbyState.gameObject.SetActive(false);
            gameState.gameObject.SetActive(true);
        }
        if (state == GameBehaviour.State.lobby)
        {
            lobbyState.gameObject.SetActive(true);
            gameState.gameObject.SetActive(false);
            joinButton.gameObject.SetActive(false);
            status.text = "Match found!";
        }
    }

    public void GameFound()
    {
        joinButton.gameObject.SetActive(true);
        status.text = "Match found!";
    }
    
}
