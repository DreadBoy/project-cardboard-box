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

    NetManager client;

    List<Vector2> spawnPoints = new List<Vector2>()
    {
        new Vector2(5, 2),
        new Vector2(5, 0)
    };

    void Start()
    {
        game = FindObjectOfType<GameBehaviour>();
        grid = FindObjectOfType<GridBehaviour>();

        //grid.GetComponent<PlayerSpawner>().enabled = false;
        //grid.GetComponent<PlayerSpawnerMock>().enabled = true;

        //grid.GetComponent<PlayerSpawnerMock>().spawnPoints = new List<Vector2>(spawnPoints);

        //game.PlayerConnect(conn1);
        //game.PlayerConnect(conn2);

        //conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.READY), conn1.player));

        //conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.READY), conn2.player));

        EventBasedNetListener listener = new EventBasedNetListener();
        client = new NetManager(listener, "SomeConnectionKey");
        client.Start();
        //client.Connect("localhost" /* host ip or name */, 9050 /* port */);
        listener.NetworkReceiveEvent += (fromPeer, dataReader) =>
        {
            Debug.Log("We got: " + dataReader.GetString(100 /* max length of string */));
        };

        listener.NetworkReceiveUnconnectedEvent += (NetEndPoint remoteEndPoint, LiteNetLib.Utils.NetDataReader reader, UnconnectedMessageType messageType) =>
        {
            Debug.Log(string.Format("[Client] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, reader.GetString(100)));
            if (messageType == UnconnectedMessageType.DiscoveryResponse)
            {
                client.Connect(remoteEndPoint);
            }
        };
    }

    float time;
    bool triggered1 = false;
    bool triggered2 = false;

    void Update()
    {
        time += Time.deltaTime;
        client.PollEvents();
        if (time > 1 && !triggered2)
        {
            triggered2 = true;


            NetDataWriter writer = new NetDataWriter();
            writer.Put("CLIENT 1 DISCOVERY REQUEST");
            client.SendDiscoveryRequest(writer, 9050);



            //game.ChangeToState_Game();
        }
        if (time > 5f && !triggered1)
        {
            triggered1 = true;
            //conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.READY), conn1.player));
            //var free = grid.IsSpotFree((int)spawnPoints[0].x, (int)spawnPoints[0].y);

            //conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.TURN, 2), conn1.player));
            //conn1.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 10), conn1.player));

            //conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.TURN, 2), conn2.player));
            //conn2.CommandReceived.RaiseEvent(new CommandArgs(new Command(Action.MOVE, 10), conn2.player));
        }
    }
}
