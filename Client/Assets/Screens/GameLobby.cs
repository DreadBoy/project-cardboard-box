using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;
using UnityEngine.UI;
using ProjectCardboardBox;
using System.Linq;

public class GameLobby : ScreenBehaviour, ICommandHandler, IFlowHandler
{
    State state = State.Uninit;
    NetEndPoint remoteEndPoint;
    NetworkBehaviour networkBehaviour;
    Text status;

    public Button joinButton;
    public Button readyButton;
    public Button notreadyButton;

    public GameObject helpGroup;

    float searchTimeoutMax = 1;
    float searchTimeout = 0;

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
    }

    public override void Update()
    {
        base.Update();
        if (searchTimeout > -1 && searchTimeout < searchTimeoutMax)
            searchTimeout += Time.deltaTime;
        if(searchTimeout >= searchTimeoutMax && state == State.Searching)
            helpGroup.SetActive(true);
    }

    public override void OnEnter(ScreenBehaviour from)
    {
        base.OnEnter(from);
        if (state == State.Uninit)
        {
            ChangeState(State.Searching);
            networkBehaviour.StartSearching(this);
        }
    }

    public void JoinGame()
    {
        networkBehaviour.JoinGame(remoteEndPoint);
        ChangeState(State.Joined);
        networkBehaviour.SendColour(PlayerPrefs.GetString(PlayerPreferences.Colour));
        networkBehaviour.SendNickname(PlayerPrefs.GetString(PlayerPreferences.Nickname));
    }

    public void ReadyGame()
    {
        networkBehaviour.SendColour(PlayerPrefs.GetString(PlayerPreferences.Colour));
        networkBehaviour.SendNickname(PlayerPrefs.GetString(PlayerPreferences.Nickname));
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.READY));
        ChangeState(State.Ready);
    }

    public void NotReadyGame()
    {
        networkBehaviour.SendCommand(new Command(ProjectCardboardBox.Action.NOTREADY));
        ChangeState(State.Joined);
    }

    public void GameFound(NetEndPoint remoteEndPoint)
    {
        if (this.remoteEndPoint != null)
            if (this.remoteEndPoint.Host == remoteEndPoint.Host)
                return;
        this.remoteEndPoint = remoteEndPoint;
        ChangeState(State.Found);
    }

    public void ReceiveCommand(List<Command> commands)
    {
        var com = commands.FirstOrDefault(c => c.type == ProjectCardboardBox.Action.CONFIRMREADY);
        if (com != null)
        {
            networkBehaviour.ChangeHandler((GameMain)transitionTo.FirstOrDefault(s => typeof(GameMain).IsInstanceOfType(s)));
            GoForward(transitionTo.FirstOrDefault(s => typeof(GameMain).IsInstanceOfType(s)));
            ChangeState(State.Uninit);
        }
    }

    void ChangeState(State newState)
    {
        this.state = newState;
        joinButton.gameObject.SetActive(false);
        notreadyButton.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);
        switch (newState)
        {
            case State.Searching:
                status.text = "Searching for match...";
                searchTimeout = 0;
                break;
            case State.Found:
                status.text = "Match found!";
                joinButton.gameObject.SetActive(true);
                searchTimeout = -1;
                break;
            case State.Joined:
                status.text = "Ready to go?";
                readyButton.gameObject.SetActive(true);
                break;
            case State.Ready:
                status.text = "Waiting for another players";
                notreadyButton.gameObject.SetActive(true);
                break;
        }
    }

    public void ServerDisconnected()
    {
        remoteEndPoint = null;
        ChangeState(State.Searching);
        networkBehaviour.StartSearching(this);
    }

    public void ReceiveChips(List<Chip> chips)
    {
        Debug.LogError("ReceiveChips called on GameLobby, that's unexpected!");
    }

    public void GameLost()
    {
        Debug.LogError("GameLost called on GameLobby, that's unexpected!");
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
