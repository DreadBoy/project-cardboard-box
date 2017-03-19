using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIBehaviour : MonoBehaviour {

    public Button joinButton;
    public Button readyButton;
    public Button notreadyButton;
    public Text status;

    public void Searching()
    {
        status.text = "Searching for match...";
        joinButton.gameObject.SetActive(false);
        notreadyButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
    }

    public void Found()
    {
        status.text = "Match found!";
        joinButton.gameObject.SetActive(true);
    }

    public void Readying()
    {
        status.text = "Ready to go?";
        joinButton.gameObject.SetActive(false);
        notreadyButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(true);
    }

    public void Waiting()
    {
        status.text = "Waiting for other players";
        notreadyButton.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(false);
    }
}
