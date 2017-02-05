using ProjectCardboardBox;
using System.Collections.Generic;
using UnityEngine;

class Testing : MonoBehaviour
{
    GameBehaviour game;
    GridBehaviour grid;
    MockConnection conn1 = new MockConnection();
    MockConnection conn2 = new MockConnection();
    public List<Vector2> spawnPoints = new List<Vector2>();

    void Start()
    {
        game = FindObjectOfType<GameBehaviour>();
        grid = FindObjectOfType<GridBehaviour>();

        grid.GetComponent<PlayerSpawner>().enabled = false;
        grid.GetComponent<PlayerSpawnerMock>().enabled = true;

        grid.GetComponent<PlayerSpawnerMock>().spawnPoints = spawnPoints;

        game.PlayerConnect(conn1);
        game.PlayerConnect(conn2);

        conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.READY), conn1.player));

        conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.READY), conn2.player));



    }

    float time;
    bool triggered1 = false;
    bool triggered2 = false;

    void Update()
    {
        time += Time.deltaTime;
        if (time > 1 && !triggered2)
        {
            triggered2 = true;

            //game.ChangeToState_Game();
        }
        if (time > 1.5f && !triggered1)
        {
            triggered1 = true;
            //conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 10), conn1.player));
            conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.TURN, 2), conn1.player));
            conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 10), conn1.player));

            conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 10), conn2.player));
        }
    }
}
