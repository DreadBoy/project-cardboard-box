using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class PlayerSpawner : MonoBehaviour {

    protected GridBehaviour gridBehaviour;
    protected GameBehaviour gameBehaviour;

    protected Random random;

    protected virtual void Start()
    {
        if (gridBehaviour == null)
            gridBehaviour = GetComponent<GridBehaviour>();
        if (gameBehaviour == null)
            gameBehaviour = FindObjectOfType<GameBehaviour>();
        random = new Random();
    }

    public virtual bool SpawnPlayerOnGrid(PlayerBehaviour player)
    {
            //you already need reference to that player
            if (gridBehaviour.players.Find(pl => pl == player) == null)
                return false;

            List<int> x_all = Enumerable.Range(0, gameBehaviour.gridSize).ToList();
            List<int> y_all = Enumerable.Range(0, gameBehaviour.gridSize).ToList();

            while (x_all.Count > 0 && y_all.Count > 0)
            {
                var x = random.Next(x_all.Count);
                var y = random.Next(y_all.Count);

                if (gridBehaviour.IsSpotFree(x, y))
                    //if (grid[x, y] == null)
                    return false;

                x_all.Remove(x);
                y_all.Remove(y);
                return SpawnPlayerOnGrid(player, x, y);


            }

            return false;
    }

    public virtual bool SpawnPlayerOnGrid(PlayerBehaviour player, int x, int y)
    {
        //you already need reference to that player
        if (gridBehaviour.players.Find(pl => pl == player) == null)
            return false;

        if (x < 0 || x >= gameBehaviour.gridSize)
            return false;
        if (y < 0 || y >= gameBehaviour.gridSize)
            return false;

        if (gridBehaviour.players.Find(p => p.IsOnSpot(x, y)) != null)
            //if (grid[x, y] != null)
            return false;

        player.SpawnPlayer(x, y);
        return true;
    }
}
