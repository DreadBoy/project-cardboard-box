using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Game
{
    System.Random random = new System.Random();

    List<Player> players = new List<Player>();

    Player[,] grid;
    int gridsize;

    public SmartEvent<GridCompiledArgs> gridCompiledEvent = new SmartEvent<GridCompiledArgs>();
    public SmartEvent<CreatePlayerArgs> createPlayerEvent = new SmartEvent<CreatePlayerArgs>();

    public Game(int gridsize)
    {
        grid = new Player[gridsize, gridsize];
        this.gridsize = gridsize;

        gridCompiledEvent.RaiseEvent(new GridCompiledArgs(gridsize));
    }

    public bool AddPlayer(Player player)
    {
        List<int> x_all = Enumerable.Range(0, gridsize + 1).ToList();
        List<int> y_all = Enumerable.Range(0, gridsize + 1).ToList();

        while (x_all.Count > 0 && y_all.Count > 0)
        {
            var x = random.Next(x_all.Count);
            var y = random.Next(y_all.Count);

            if (grid[x, y] == null)
            {
                x_all.Remove(x);
                y_all.Remove(y);
                return AddPlayer(x, y, player);
            }

        }

        return false;

    }

    public bool AddPlayer(int x, int y, Player player)
    {
        if (x < 0 || x >= gridsize)
            return false;
        if (y < 0 || y >= gridsize)
            return false;

        if (grid[x, y] != null)
            return false;

        players.Add(player);
        player.position = new UnityEngine.Vector3(x, y, 0);
        createPlayerEvent.RaiseEvent(new CreatePlayerArgs(player));

        return true;
    }


}
