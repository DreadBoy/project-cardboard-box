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

    void OnEnable()
    {
        validSprite = image.sprite;
        if (image == null)
            image = GetComponent<Image>();
        if (button == null)
            button = GetComponent<Button>();
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
            if (_valid)
                image.sprite = validSprite;
            else
                image.sprite = invalidSprite;
            button.interactable = _valid;
        }
    }

    void Start()
    {
    }



    void Update()
    {

    }
}
