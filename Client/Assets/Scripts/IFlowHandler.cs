using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IFlowHandler
{
    void GameFound(NetEndPoint remoteEndPoint);
    void GameLost();
}
