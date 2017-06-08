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
    public Image image;
    public Button button;

    public Sprite turnSprite, moveSprite;

    GameMain gameMain;

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
        if (text == null)
            text = GetComponentInChildren<Text>();
        if (image == null)
            image = GetComponentInChildren<Image>();
        if (button == null)
            button = GetComponent<Button>();
    }

    public void Init(Chip chip, GameMain game)
    {
        this.chip = chip;
        gameMain = game;
        transform.SetParent(game.transform);
    }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(onClick);

        if (chip.type == Chip.Type.Action)
        {
            if (chip.value == Action.MOVE.ToString())
                image.sprite = moveSprite;
            else if (chip.value == Action.TURN.ToString())
                image.sprite = turnSprite;
            image.color = Color.white;
        }
        else
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