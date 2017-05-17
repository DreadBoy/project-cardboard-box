using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using UnityEngine.UI;
using ProjectCardboardBox;

public class GameLobby : ScreenBehaviour
{
    State state = State.Uninit;
    NetEndPoint remoteEndPoint;
    NetworkBehaviour networkBehaviour;
    Text status;

    public Button joinButton;
    public Button readyButton;
    public Button notreadyButton;

    public override void OnEnable()
    {
        base.OnEnable();
        status = transform.Find("Panel/Status").GetComponent<Text>();
        networkBehaviour = FindObjectOfType<NetworkBehaviour>();

        joinButton = transform.Find("Panel/joinButton").GetComponent<Button>();
        readyButton = transform.Find("Panel/readyButton").GetComponent<Button>();
        notreadyButton = transform.Find("Panel/notreadyButton").GetComponent<Button>();
    }

    public override void Start()
    {
        base.Start();
        status.text = "Searching for match...";
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnEnter(ScreenBehaviour from)
    {
        base.OnEnter(from);
        if (state == State.Uninit)
        {
            changeState(State.Searching);
            networkBehaviour.StartSearching(this);
        }
    }

    public void JoinGame()
    {
        networkBehaviour.JoinGame(remoteEndPoint);
        changeState(State.Joined);
    }

    public void ReadyGame()
    {
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.READY));
        changeState(State.Ready);
    }

    public void NotReadyGame()
    {
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.NOTREADY));
        changeState(State.Joined);
    }

    internal void GameFound(NetEndPoint remoteEndPoint)
    {
        if (this.remoteEndPoint != null)
            if (this.remoteEndPoint.Host == remoteEndPoint.Host)
                return;
        this.remoteEndPoint = remoteEndPoint;
        changeState(State.Found);
    }

    void changeState(State newState)
    {
        this.state = newState;
        joinButton.gameObject.SetActive(false);
        notreadyButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
        switch (newState)
        {
            case State.Searching:
                status.text = "Searching for match...";
                break;
            case State.Found:
                status.text = "Match found!";
                joinButton.gameObject.SetActive(true);
                break;
            case State.Joined:
                status.text = "Ready to go?";
                readyButton.gameObject.SetActive(true);
                break;
            case State.Ready:
                status.text = "Waiting for \nother players";
                notreadyButton.gameObject.SetActive(true);
                break;
        }
    }

    enum State
    {
        Uninit,
        Searching,
        Found,
        Joined,
        Ready
    }
}
