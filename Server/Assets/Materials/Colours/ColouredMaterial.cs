﻿using ProjectCardboardBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColouredMaterial : Singleton<ColouredMaterial>
{

    protected ColouredMaterial() { }

    void OnEnable()
    {
        foreach (var colour in Colours.AllColours)
        {
            var mat = new Material(BaseMaterial);
            mat.color = HexToColor(colour);
            materials.Add(colour, mat);
        }

        availableColours = Colours.AllColours.ToList();
        random = new System.Random();
    }

    public Material BaseMaterial;

    Dictionary<string, Material> materials = new Dictionary<string, Material>();
    List<string> availableColours = new List<string>();
    System.Random random;

    public Material GetNewColour()
    {
        if (availableColours.Count == 0)
            availableColours = materials.Keys.ToList();
        var index = random.Next(0, availableColours.Count);
        var ret = materials[availableColours[index]];
        availableColours.RemoveAt(index);
        return ret;
    }

    private static Color HexToColor(string colour)
    {
        return new Color(
            int.Parse(colour.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
            int.Parse(colour.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
            int.Parse(colour.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
    }
}
