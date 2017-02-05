using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnerMock : PlayerSpawner
{

    public List<Vector2> spawnPoints = new List<Vector2>();

    protected override void Start()
    {
        base.Start();
    }

    public override bool SpawnPlayerOnGrid(PlayerBehaviour player)
    {
        //you already need reference to that player
        if (gridBehaviour.players.Find(pl => pl == player) == null)
            return false;

        //this spot is occupied
        if (gridBehaviour.players.Find(p => p.IsOnSpot((int)spawnPoints[0].x, (int)spawnPoints[0].y)) != null)
            return false;

        var point = spawnPoints[0];
        spawnPoints.RemoveAt(0);
        return SpawnPlayerOnGrid(player, (int)point.x, (int)point.y);


    }

}
