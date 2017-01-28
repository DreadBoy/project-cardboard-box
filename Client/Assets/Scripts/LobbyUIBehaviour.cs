using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIBehaviour : MonoBehaviour {

    public Button joinButton;
    public Button readyButton;
    public Text status;

    public void Searching()
    {
        status.text = "Searching for match...";
        joinButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
    }

    public void Found()
    {
        status.text = "Match found!";
        joinButton.gameObject.SetActive(true);
    }

    public void Waiting()
    {
        status.text = "Ready to go?";
        joinButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(true);
    }
}
