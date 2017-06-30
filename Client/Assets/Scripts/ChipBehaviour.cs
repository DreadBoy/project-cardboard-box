using ProjectCardboardBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipBehaviour : MonoBehaviour
{

    public Chip chip;
    public RectTransform rectTransform;
    public Button button;

    public bool IsSource;

    public GameMain gameMain;

    LerpHelper<Vector2> lerpPosition;

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
        if (button == null)
            button = GetComponent<Button>();
    }

    public void Init(GameMain game, GameObject parent, Vector2 position)
    {
        gameMain = game;
        transform.SetParent(parent.transform);
        rectTransform.anchoredPosition = position;
        IsSource = false;
    }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(onClick);
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
        if (IsSource && gameMain != null)
        {
            button.interactable = gameMain.CanMoveToDest(this);
        }
    }

    public void onClick()
    {
        gameMain.TransferChip(this);
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