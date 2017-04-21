﻿using UnityEngine;
using UnityEngine.UI;
using ProjectCardboardBox;
using System.Collections.Generic;

public class InitColour : ScreenBehaviour
{
    Button confirm;
    public Sprite spriteSwatch;
    public Sprite spriteSwatchSelected;
    string colourSelected = "";

    List<GameObject> swatches = new List<GameObject>();

    public override void OnEnable()
    {
        base.OnEnable();
        confirm = transform.Find("Panel/Confirm").GetComponent<Button>();
    }

    public override void Start()
    {
        base.Start();
        var parent = transform.Find("Panel");
        for (int i = 0; i < Colours.AllColours.Length; i++)
        {
            var swatch = CreateSwatch(parent, Colours.AllColours[i], i);
            swatches.Add(swatch);
        }
    }

    public override void Update()
    {
        base.Update();
        confirm.interactable = !string.IsNullOrEmpty(colourSelected);
    }

    GameObject CreateSwatch(Transform parent, string colour, int index)
    {
        var swatch = new GameObject();
        swatch.AddComponent<RectTransform>();
        swatch.AddComponent<Image>();
        swatch.AddComponent<Button>();

        swatch.name = "Swatch #" + colour;

        swatch.transform.SetParent(parent, false);

        var rect = swatch.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);
        var x = (index % 5) * 120 - 240;
        var y = index < 5 ? 75 : -50;
        rect.anchoredPosition = new Vector2(x, y);

        var rgb = Colours.HexToRgb(colour);
        swatch.GetComponent<Image>().color = new Color(rgb[0] / 255.0f, rgb[1] / 255.0f, rgb[2] / 255.0f, 1);
        swatch.GetComponent<Image>().sprite = spriteSwatch;

        var listener = new SwatchClicked(this)
        {
            Swatch = swatch,
            Colour = colour
        };

        swatch.GetComponent<Button>().onClick.AddListener(listener.Clicked);

        return swatch;
    }

    void SelectSwatch(GameObject selectedSwatch, string colour)
    {
        foreach (var swatch in swatches)
            swatch.GetComponent<Image>().sprite = spriteSwatch;
        selectedSwatch.GetComponent<Image>().sprite = spriteSwatchSelected;
        colourSelected = colour;
    }

    class SwatchClicked
    {
        public string Colour { get; set; }
        public GameObject Swatch { get; set; }

        InitColour initColour;

        public SwatchClicked(InitColour initColour)
        {
            this.initColour = initColour;
        }

        public void Clicked()
        {
            initColour.SelectSwatch(Swatch, Colour);
        }
    }
}