using ProjectCardboardBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipBehaviour : MonoBehaviour {

    public Chip chip;
    public RectTransform rectTransform;
    public Text text;

    GameUIBehaviour gameUiBehaviour;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponentInChildren<Text>();
    }


    void Start () {
        gameUiBehaviour = FindObjectOfType<GameUIBehaviour>();

        var button = GetComponent<Button>();
        button.onClick.AddListener(onClick);

        text.text = chip.value;
    }

    void Update() {
    }

    public void onClick()
    {
        gameUiBehaviour.TransferChip(this);
    }
}
