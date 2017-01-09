using UnityEngine;

class Testing : MonoBehaviour
{
    GameBehaviour game;
    MockConnection conn1 = new MockConnection();
    MockConnection conn2 = new MockConnection();

    void Start()
    {
        game = FindObjectOfType<GameBehaviour>();

        game.PlayerConnect(conn1);
        game.PlayerConnect(conn2);

        //conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Command.Type.READY), conn1.player));
        //conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Command.Type.READY), conn2.player));


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

            game.ChangeToState_Game();
        }
        if (time > 2 && !triggered1)
        {
            triggered1 = true;
            conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Command.Type.MOVE, 2), conn1.player));
            conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Command.Type.TURN, 2), conn1.player));
            conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Command.Type.MOVE, 2), conn1.player));
        }
    }
}
