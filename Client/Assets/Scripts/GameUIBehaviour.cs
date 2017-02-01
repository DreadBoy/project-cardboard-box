using System;
using System.Collections;
using System.Collections.Generic;
using ProjectCardboardBox;
using UnityEngine;
using System.Linq;

public class GameUIBehaviour : MonoBehaviour
{
    NetworkBehaviour networkBehaviour;

    public ChipBehaviour chipPrefab;

    List<ChipBehaviour> sourceChips = new List<ChipBehaviour>();
    List<ChipBehaviour> destinationChips = new List<ChipBehaviour>();

    Vector2 sourceStart = new Vector2(-450, 275);
    Vector2 destinationStart = new Vector2(-450, -145);

    Vector2 sourceSpace = new Vector2(300, -130);
    Vector2 destinationSpace = new Vector2(300, -130);

    void Start()
    {
        networkBehaviour = FindObjectOfType<NetworkBehaviour>();
    }

    public void OnHandReceived(List<Chip> chips)
    {
        this.sourceChips.AddRange(chips.Select(c =>
        {
            var chip = Instantiate<ChipBehaviour>(chipPrefab);
            chip.chip = c;
            chip.transform.SetParent(transform);
            return chip;
        }));

        UpdateChips();
    }

    public void TransferChip(ChipBehaviour chip)
    {
        if (sourceChips.Contains(chip))
        {
            sourceChips.Remove(chip);
            destinationChips.Add(chip);
        }
        else if (destinationChips.Contains(chip))
        {
            destinationChips.Remove(chip);
            sourceChips.Add(chip);
        }
        UpdateChips();
    }

    internal void ClearAll()
    {
        DestroyChips(destinationChips);
        DestroyChips(sourceChips);
        destinationChips.Clear();
        sourceChips.Clear();
    }

    //Vector2 GetChipLocalPosition(ChipBehaviour chip)
    //{
    //    var index = -1;
    //    index = sourceChips.IndexOf(chip);
    //    if (index >= 0)
    //    {
    //        return new Vector2(
    //            (index % 4) * sourceSpace.x,
    //            (index - index % 4) / 4 * sourceSpace.y);
    //    }
    //    else
    //        index = destinationChips.IndexOf(chip);
    //    if(index >= 0)
    //    {
    //        return Vector2.zero;
    //    }
    //    return Vector2.zero;
    //}

    public void UpdateChips()
    {
        for (int i = 0; i < sourceChips.Count; i++)
        {
            var chip = sourceChips[i];
            chip.transform.SetParent(transform);
            chip.rectTransform.localScale = Vector3.one;
            chip.LerpTo(
                sourceStart +
                new Vector2(
                    (i % 4) * sourceSpace.x,
                    (i - i % 4) / 4 * sourceSpace.y));
        }

        for (int i = 0; i < destinationChips.Count; i++)
        {
            var chip = destinationChips[i];
            chip.transform.SetParent(transform);
            chip.rectTransform.localScale = Vector3.one;
            chip.LerpTo(
                destinationStart +
                new Vector2(
                    (i % 4) * destinationSpace.x,
                    (i - i % 4) / 4 * destinationSpace.y));
        }
    }

    void DestroyChips(List<ChipBehaviour> chips)
    {
        chips.ForEach(c => Destroy(c.gameObject));
    }

    public void SendCommands()
    {
        List<Command> commands = new List<Command>();
        if (Command.TryParse(ref commands, destinationChips.Select(c => c.chip).ToArray()))
        {
            commands.Insert(0, new Command(ProjectCardboardBox.Action.REQUESTCHIPS, destinationChips.Count));
            networkBehaviour.SendCommands(commands.ToArray());
            DestroyChips(destinationChips);
            destinationChips.Clear();
            UpdateChips();
        }
    }
}
