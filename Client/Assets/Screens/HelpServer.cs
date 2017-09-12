using System;
using System.Collections.Generic;
using LiteNetLib;
using ProjectCardboardBox;
using UnityEngine;
using System.Linq;

public class HelpServer: ScreenBehaviour, IFlowHandler, ICommandHandler
{

    public override void Start()
    {
        base.Start();
    }

    public void ServerDisconnected()
    {
        Debug.LogError("Server Disconnected called on HelpServer, that's unexpected!");
    }

    public void NextGame()
    {
        Debug.LogError("Next Game called on HelpServer, that's unexpected!");
    
    }

    public void GameFound(NetEndPoint remoteEndPoint)
    {
        Debug.LogError("Game Found called on HelpServer, that's unexpected!");
    }

    public void GameLost()
    {
        Debug.LogError("Game Lost called on HelpServer, that's unexpected!");
    }

    public void ReceiveCommand(List<Command> commands)
    {
        Debug.LogError("Receive command called on HelpServer, that's unexpected!");
    }
}