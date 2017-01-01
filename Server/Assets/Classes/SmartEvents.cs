using System;

public class GridCompiledEventArgs : EventArgs
{
    public int size;

    public GridCompiledEventArgs(int size)
    {
        this.size = size;
    }
}