using System;
using System.Collections.Generic;
using LiteNetLib;
using ProjectCardboardBox;
using UnityEngine;
using System.Linq;

public class OverVictory : ScreenBehaviour, IFlowHandler, ICommandHandler
{
    NetworkBehaviour networkBehaviour;

    public override void Start()
    {
        base.Start();
        if (networkBehaviour == null)
            networkBehaviour = FindObjectOfType<NetworkBehaviour>();
    }

    public void ServerDisconnected()
    {
        networkBehaviour.ChangeHandler((GameLobby)transitionTo.FirstOrDefault(s => typeof(GameLobby).IsInstanceOfType(s)));
        GoForward(transitionTo.FirstOrDefault(s => typeof(GameLobby).IsInstanceOfType(s)));
    }

    public void NextGame()
    {
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.NEWGAME));
    }

    public void GameFound(NetEndPoint remoteEndPoint)
    {
        Debug.LogError("Game Found called on OverVictory, that's unexpected!");
    }

    public void GameLost()
    {
        Debug.LogError("Game Lost called on OverVictory, that's unexpected!");
    }

    public void ReceiveCommand(List<Command> commands)
    {
        Debug.LogError("Receive command called on OverVictory, that's unexpected!");
    }
}