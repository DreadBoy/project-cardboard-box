using System;
using UnityEngine;
using Cardboard;

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

public class spawnPlayerOnGridArgs : EventArgs
{
    public Player player;

    public spawnPlayerOnGridArgs(Player player)
    {
        this.player = player;
    }
}

//TODO very similar name, fix that!

public class CommandArgs : EventArgs
{
    public Command command;

    public CommandArgs(Command command)
    {
        this.command = command;
    }
}