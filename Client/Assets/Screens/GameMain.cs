using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib;
using ProjectCardboardBox;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : ScreenBehaviour, ICommandHandler, IFlowHandler
{
    NetworkBehaviour networkBehaviour;
    public SendButtonBehaviour sendButton;
    public Text hint;

    public RectTransform panel;
    public ChipBehaviour chipPrefab;

    List<ChipBehaviour> sourceChips = new List<ChipBehaviour>();
    List<ChipBehaviour> destinationChips = new List<ChipBehaviour>();

    Vector2 sourceStart = new Vector2(-410, 200);
    Vector2 destinationStart = new Vector2(-410, -100);

    Vector2 sourceSpace = new Vector2(102.5f, -100);
    Vector2 destinationSpace = new Vector2(102.5f, -100);

    public int chipPerRow = 5;

    bool yourTurn = false;
    float yourTurnStart = -1;

    string[] hints = {
        @"Other players are making their moves and soon it's going to be your turn. You can prepare your move though...
Each move consists of one action, followed by one or more modifiers. Modifers are summed together.",
    @"It's your turn! Quickly make your move and send it to playing field, your character is waiting!"};

    public override void Start()
    {
        base.Start();
        if (networkBehaviour == null)
            networkBehaviour = FindObjectOfType<NetworkBehaviour>();
        if (sendButton == null)
            sendButton = FindObjectOfType<SendButtonBehaviour>();
    }

    public void ReceiveChips(List<Chip> chips)
    {
        this.sourceChips.AddRange(chips.Select(c =>
        {
            var chip = Instantiate(chipPrefab);
            chip.Init(c, this);
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
        UpdateSendButton();
        SendHint();
    }

    private void SendHint()
    {
        if (destinationChips.Count == 0)
        {
            networkBehaviour.SendHint(new Command[0]);
            return;
        }
        List<Command> commands;
        if (Command.TryParseHand(destinationChips.Select(c => c.chip).ToArray(), out commands))
            networkBehaviour.SendHint(commands.ToArray());
    }

    internal void ClearAll()
    {
        DestroyChips(destinationChips);
        DestroyChips(sourceChips);
        destinationChips.Clear();
        sourceChips.Clear();
    }

    public void UpdateChips()
    {
        for (int i = 0; i < sourceChips.Count; i++)
        {
            var chip = sourceChips[i];
            chip.transform.SetParent(panel);
            chip.rectTransform.localScale = Vector3.one;
            chip.LerpTo(
                sourceStart +
                new Vector2(
                    (i % chipPerRow) * sourceSpace.x,
                    (i - i % chipPerRow) / chipPerRow * sourceSpace.y));
            chip.Valid = Command.IsNextChipValid(destinationChips.Select(c => c.chip).ToArray(), chip.chip);
        }

        for (int i = 0; i < destinationChips.Count; i++)
        {
            var chip = destinationChips[i];
            chip.transform.SetParent(panel);
            chip.rectTransform.localScale = Vector3.one;
            chip.LerpTo(
                destinationStart +
                new Vector2(
                    (i % chipPerRow) * destinationSpace.x,
                    (i - i % chipPerRow) / chipPerRow * destinationSpace.y));
        }
    }

    void UpdateSendButton()
    {
        List<Command> commands;
        var valid = Command.TryParseHand(destinationChips.Select(c => c.chip).ToArray(), out commands);
        sendButton.Valid = valid;
    }

    void DestroyChips(List<ChipBehaviour> chips)
    {
        chips.ForEach(c => Destroy(c.gameObject));
    }

    public void SendCommands()
    {
        List<Command> commands;
        if (Command.TryParseHand(destinationChips.Select(c => c.chip).ToArray(), out commands))
        {
            commands.Insert(0, new Command(ProjectCardboardBox.Action.REQUESTCHIPS, destinationChips.Count));
            networkBehaviour.SendCommands(commands.ToArray());
            DestroyChips(destinationChips);
            destinationChips.Clear();
            UpdateChips();
        }
    }

    public void GameLost()
    {
        Debug.Log("Game lost!");
    }

    public void ReceiveCommand(List<Command> commands)
    {
        foreach (var command in commands)
        {
            if (command.type == ProjectCardboardBox.Action.YOURTURN)
            {
                yourTurn = true;
                yourTurnStart = Time.time;
                sendButton.YourTurn();
            }
        }
    }

    public override void Update()
    {
        base.Update();
        if (yourTurnStart > 0)
        {
            if (yourTurnStart + 10 < Time.time)
            {
                yourTurn = false;
                yourTurnStart = -1;
                sendButton.WaitingForTurn();
            }
        }
    }

    #region unused

    public void GameFound(NetEndPoint remoteEndPoint)
    {
        Debug.LogError("Game Found called on GameMain, that's unexpected!");
    }

    #endregion
}