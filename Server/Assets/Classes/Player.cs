using UnityEngine;
using UnityEngine.Networking;

public class Player {

    NetworkConnection connection;

    Vector3 _position;
    public Vector3 position
    {
        get
        {
            return _position;
        }
        set
        {
            _position.Set(value.x, value.y, value.z);
            spawnPlayerOnGridEvent.RaiseEvent(new SpawnPlayerOnGridArgs(_position));
        }
    }
    public Vector3 offset = Vector3.zero;
    
    public SmartEvent<SpawnPlayerOnGridArgs> spawnPlayerOnGridEvent = new SmartEvent<SpawnPlayerOnGridArgs>();

    public Player()
    {
    }

    public Player(NetworkConnection connection)
    {
        this.connection = connection;
    }

    public void SetPosition(Vector3 position)
    {
        this.position = position;

    }
}
