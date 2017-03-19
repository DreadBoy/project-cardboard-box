using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIBehaviour : MonoBehaviour {

    public Text status;

    public void Loss()
    {
        status.text = "Ouch, you lost. :( \nBetter luck next time";
    }

    public void Win()
    {
        status.text = "Congratz, you won! \nIsn't that amazing?";
    }
}
