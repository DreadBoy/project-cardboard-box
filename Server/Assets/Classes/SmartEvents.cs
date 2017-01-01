using System;
using UnityEngine;

public class GridCompiledArgs : EventArgs
{
    public int size;

    public GridCompiledArgs(int size)
    {
        this.size = size;
    }
}

public class SpawnPlayerOnGridArgs : EventArgs
{
    public Vector3 position;

    public SpawnPlayerOnGridArgs(Vector3 position)
    {
        this.position = position;
    }
}

public class CreatePlayerArgs : EventArgs
{
    public Player player;

    public CreatePlayerArgs(Player player)
    {
        this.player = player;
    }
}