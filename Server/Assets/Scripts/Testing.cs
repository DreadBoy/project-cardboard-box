using LiteNetLib;
using LiteNetLib.Utils;
using ProjectCardboardBox;
using System.Collections.Generic;
using UnityEngine;

class Testing : MonoBehaviour
{
    GameBehaviour game;
    GridBehaviour grid;
    MockConnection conn1 = new MockConnection();
    MockConnection conn2 = new MockConnection();
    MockConnection conn3 = new MockConnection();

    List<Vector2> spawnPoints = new List<Vector2>()
    {
        new Vector2(6, 8),
        new Vector2(6, 9),
        new Vector2(5, 5),
    };

    void Start()
    {
        game = FindObjectOfType<GameBehaviour>();
        grid = FindObjectOfType<GridBehaviour>();

        grid.GetComponent<PlayerSpawner>().enabled = false;
        grid.GetComponent<PlayerSpawnerMock>().enabled = true;

        grid.GetComponent<PlayerSpawnerMock>().spawnPoints = new List<Vector2>(spawnPoints);
    }

    float time;
    bool triggered1 = false;
    bool triggered2 = false;

    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.5f && !triggered2)
        {
            triggered2 = true;

            game.PlayerConnect(conn1);
            conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.READY), conn1.Player));

            game.PlayerConnect(conn2);
            conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.READY), conn2.Player));

            //game.PlayerConnect(conn3);
            //conn3.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.READY), conn3.Player));
        }
        if (time > 1f && !triggered1)
        {
            triggered1 = true;


            conn1.HintReceived.RaiseEvent(new HintArgs("TURN:1|TURN:1|MOVE:5", conn1.Player));
            conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 5), conn1.Player));

            conn1.NicknameReceived.RaiseEvent(new StringArgs("Matic Leva", conn1.Player));

            //conn3.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.TURN, 1), conn3.Player));
            //conn3.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 1), conn3.Player));
            //conn3.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.TURN, 3), conn3.Player));
            //conn3.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 10), conn3.Player));

            //var free = grid.IsSpotFree((int)spawnPoints[0].x, (int)spawnPoints[0].y);

            //conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.TURN, 2), conn1.player));
            //conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 10), conn1.player));

            //conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.TURN, 2), conn2.player));
            //conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 10), conn2.player));
        }
    }
}
