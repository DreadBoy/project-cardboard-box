using ProjectCardboardBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColouredMaterial : Singleton<ColouredMaterial>
{

    protected ColouredMaterial() { }

    void OnEnable()
    {
        materials.Add(Colours.Black, Black);
        materials.Add(Colours.Cyan, Cyan);
        materials.Add(Colours.Pink, Pink);
        materials.Add(Colours.Violet, Violet);
        materials.Add(Colours.Blue, Blue);
        materials.Add(Colours.Red, Red);
        materials.Add(Colours.Brown, Brown);
        materials.Add(Colours.Orange, Orange);
        materials.Add(Colours.Green, Green);
        materials.Add(Colours.Yellow, Yellow);

        availableColours = materials.Keys.ToList();
        random = new System.Random();
    }

    public Material Black;
    public Material Cyan;
    public Material Pink;
    public Material Violet;
    public Material Blue;
    public Material Red;
    public Material Brown;
    public Material Orange;
    public Material Green;
    public Material Yellow;

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
}
