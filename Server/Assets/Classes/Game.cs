using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace Cardboard
{
    internal class Game
    {
        System.Random random = new System.Random();

        public Game(int gridsize)
        {
            //grid = new Player[gridsize, gridsize];
            //gridCompiledEvent.RaiseEvent(new GridCompiledArgs(gridsize));
            
            //changeStateEvent.RaiseEvent(new changeStateArgs(state));
        }

        //public bool SpawnPlayerOnGrid(Player player)
        //{
        //    //you already need reference to that player
        //    if (players.Find(pl => pl == player) == null)
        //        return false;

        //    List<int> x_all = Enumerable.Range(0, gridsize).ToList();
        //    List<int> y_all = Enumerable.Range(0, gridsize).ToList();

        //    while (x_all.Count > 0 && y_all.Count > 0)
        //    {
        //        var x = random.Next(x_all.Count);
        //        var y = random.Next(y_all.Count);

        //        if (players.Find(p => p.IsOnSpot(x, y)) != null)
        //            //if (grid[x, y] == null)
        //            return false;

        //        x_all.Remove(x);
        //        y_all.Remove(y);
        //        return SpawnPlayerOnGrid(player, x, y);


        //    }

        //    return false;
        //}

        //public bool SpawnPlayerOnGrid(Player player, int x, int y)
        //{
        //    //you already need reference to that player
        //    if (players.Find(pl => pl == player) == null)
        //        return false;

        //    if (x < 0 || x >= gridsize)
        //        return false;
        //    if (y < 0 || y >= gridsize)
        //        return false;

        //    if (players.Find(p => p.IsOnSpot(x, y)) != null)
        //        //if (grid[x, y] != null)
        //        return false;

        //    player.position = new UnityEngine.Vector3(x, 0, y);
        //    spawnPlayerOnGridEvent.RaiseEvent(new SpawnPlayerOnGridArgs(player));
        //    return true;
        //}



    }

}