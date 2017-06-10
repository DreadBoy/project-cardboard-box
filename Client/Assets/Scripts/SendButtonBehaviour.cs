using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class SendButtonBehaviour : MonoBehaviour
{

    public Button button;
    public Image image;
    public Sprite invalidSprite;
    Sprite validSprite;
    Text text;

    void OnEnable()
    {
        validSprite = image.sprite;
        if (image == null)
            image = GetComponent<Image>();
        if (button == null)
            button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        WaitingForTurn();
    }

    bool _valid = false;
    public bool Valid
    {
        get
        {
            return _valid;
        }
        set
        {
            _valid = value;
            button.interactable = _valid;
        }
    }

    public void WaitingForTurn()
    {
        text.text = "Waiting...";
        button.interactable = false;
    }

    public void YourTurn()
    {
        text.text = "Send";
        button.interactable = _valid;
    }
}
