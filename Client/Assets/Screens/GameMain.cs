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
    public Text timeCounter;

    public RectTransform panel;
    public ChipBehaviour chipPrefab;

    List<ChipBehaviour> sourceChips = new List<ChipBehaviour>();

    List<ChipBehaviour> destinationChips = new List<ChipBehaviour>();
    Vector2[] destinationSlots = {
            new Vector2(525, -180),
            new Vector2(625, -180),
            new Vector2(725, -180),
            new Vector2(825, -180),
            new Vector2(925, -180),
            new Vector2(1025, -180),
            new Vector2(1125, -180),
        };


    float yourTurnStart = -1;

    string[] hints = {
        @"Other players are making their moves and soon it's going to be your turn. You can prepare your commands though...

Each command consists of one action, followed by one or more multipliers. Multipliers are summed together and you can create 2 commands at once.",
    @"It's your turn! Quickly make your move and send it to playing field, your character is waiting!"};

    string timeCount = "Time remaining: ";

    public override void Start()
    {
        base.Start();
        if (networkBehaviour == null)
            networkBehaviour = FindObjectOfType<NetworkBehaviour>();
        if (sendButton == null)
            sendButton = FindObjectOfType<SendButtonBehaviour>();
    }

    public void TransferChip(ChipBehaviour chip)
    {
        if (destinationChips.Contains(chip))
        {
            Destroy(chip.gameObject);
            destinationChips.Remove(chip);
            ReorderChips(destinationChips);
            return;
        }

        if (!canMoveToDest(chip))
            return;

        var chipNew = Instantiate(chip.gameObject).GetComponent<ChipBehaviour>();
        chipNew.Init(this, panel.gameObject, destinationSlots[destinationChips.Count]);

        destinationChips.Add(chipNew.GetComponent<ChipBehaviour>());
        ReorderChips(destinationChips);
        UpdateSendButton();
        SendHint();
    }

    public void ReorderChips(List<ChipBehaviour> chips)
    {
        // We don't have action in our command
        if(chips.FirstOrDefault(c => c.chip.type == Chip.Type.Action) == null)
        {
            ClearAll();
            return;
        }
        var action = chips.FirstOrDefault(c => c.chip.type == Chip.Type.Action);
        chips.Remove(action);
        chips.Insert(0, action);

        for (int i = 0; i < chips.Count; i++)
        {
            chips[i].LerpTo(destinationSlots[i]);
        }
    }

    public bool canMoveToDest(ChipBehaviour chip)
    {
        // We want to insert action and command already have action
        if (chip.chip.type == Chip.Type.Action)
            if (destinationChips.FirstOrDefault(c => c.chip.type == Chip.Type.Action) != null)
                return false;

        // We want to insert number and command is already full
        if (chip.chip.type == Chip.Type.Number)
        {
            if (destinationChips.Count == 7)
                return false;
            // or command doesn't have action yet
            if (destinationChips.FirstOrDefault(c => c.chip.type == Chip.Type.Action) == null)
                return false;
        }
        return true;
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
        destinationChips.Clear();
    }

    void UpdateSendButton()
    {
        List<Command> commands;
        var valid = Command.TryParseHand(destinationChips.Select(c => c.chip).ToArray(), out commands);
        sendButton.Valid = valid && yourTurnStart > 0;
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
            networkBehaviour.SendCommands(commands.ToArray());
            DestroyChips(destinationChips);
            destinationChips.Clear();
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
                Debug.Log("It's my turn!");
                yourTurnStart = Time.time;
                sendButton.YourTurn();
                hint.text = hints[1];
            }
        }
    }

    public override void Update()
    {
        base.Update();
        if (yourTurnStart > 0)
        {
            timeCounter.text = timeCount + Math.Floor(11 + yourTurnStart - Time.time);
            if (yourTurnStart + 11 < Time.time)
            {
                yourTurnStart = -1;
                sendButton.WaitingForTurn();
                hint.text = hints[0];
                networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.MOVE, 0));
            }
        }
        else
            timeCounter.text = "";
    }

    #region unused

    public void GameFound(NetEndPoint remoteEndPoint)
    {
        Debug.LogError("Game Found called on GameMain, that's unexpected!");
    }

    #endregion
}