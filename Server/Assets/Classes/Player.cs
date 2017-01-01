using UnityEngine;
using UnityEngine.Networking;

public class Player {

    NetworkConnection connection;

    public Vector3 position;
    public Vector3 offset = Vector3.zero;

    public Player()
    {
    }

    public Player(NetworkConnection connection)
    {
        this.connection = connection;
    }
}
