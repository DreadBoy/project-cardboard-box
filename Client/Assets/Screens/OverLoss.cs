using LiteNetLib;
using ProjectCardboardBox;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverLoss : ScreenBehaviour, IFlowHandler, ICommandHandler
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

    public void GameFound(NetEndPoint remoteEndPoint)
    {
        Debug.LogError("Game Found called on OverLoss, that's unexpected!");
    }

    public void GameLost()
    {
        Debug.LogError("Game Lost called on OverLoss, that's unexpected!");
    }

    public void ReceiveCommand(List<Command> commands)
    {
        Debug.LogError("Receive command called on OverVictory, that's unexpected!");
    }
}