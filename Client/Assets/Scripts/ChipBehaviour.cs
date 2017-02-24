using ProjectCardboardBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipBehaviour : MonoBehaviour
{

    public Chip chip;
    public RectTransform rectTransform;
    public Text text;
    public Button button;

    LerpHelper<Vector2> lerpPosition;

    GameUIBehaviour gameUiBehaviour;

    private bool _enabled = true;
    public bool Valid
    {
        get
        {
            return _enabled;
        }
        set
        {
            _enabled = value;
            button.interactable = value;
        }
    }

    void OnEnable()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (text == null)
            text = GetComponentInChildren<Text>();
        if (button == null)
            button = GetComponent<Button>();
    }


    void Start()
    {
        gameUiBehaviour = FindObjectOfType<GameUIBehaviour>();

        var button = GetComponent<Button>();
        button.onClick.AddListener(onClick);

        text.text = chip.value;
    }

    void Update()
    {
        if (lerpPosition != null)
        {
            lerpPosition.Update(Time.deltaTime);
            rectTransform.anchoredPosition = lerpPosition.Lerp();
            if (lerpPosition.IsDone())
                lerpPosition = null;
        }
    }

    public void onClick()
    {
        gameUiBehaviour.TransferChip(this);
    }

    public void LerpTo(Vector2 anchoredPosition)
    {
        lerpPosition = new LerpHelper<Vector2>(
            rectTransform.anchoredPosition,
            anchoredPosition,
            Vector2.Lerp,
            3000,
            Vector2.Distance(anchoredPosition, rectTransform.anchoredPosition));
    }


}