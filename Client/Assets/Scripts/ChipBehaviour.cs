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

    LerpHelper<Vector2> lerpPosition;

    GameUIBehaviour gameUiBehaviour;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponentInChildren<Text>();
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

    public void AnchorLeftTop()
    {
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
    }

    public void AnchorLeftBottom()
    {
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
    }
}
