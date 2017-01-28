using ProjectCardboardBox;
using System;
using System.Collections.Generic;
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
    public PlayerBehaviour player;

    public SpawnPlayerOnGridArgs(PlayerBehaviour player)
    {
        this.player = player;
    }
}

public class CommandArgs : EventArgs
{
    public Command command;
    public PlayerBehaviour player;

    public CommandArgs(Command command, PlayerBehaviour player)
    {
        this.command = command;
        this.player = player;
    }
}

public class changeStateArgs : EventArgs
{
    public GameBehaviour.State state;

    public changeStateArgs(GameBehaviour.State state)
    {
        this.state = state;
    }
}

public class ChipsArgs : EventArgs
{
    public List<Chip> chips;
    public PlayerBehaviour player;

    public ChipsArgs(List<Chip> chips, PlayerBehaviour player)
    {
        this.chips = chips;
        this.player = player;
    }
}

