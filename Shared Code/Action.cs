using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCardboardBox
{
    public enum Action
    {
        NEWGAME = -9,
        ENDTURN = -8,
        YOURTURN = -7,
        VICTORY = -6,
        GAMEOVER = -5,
        CONFIRMREADY = -4,
        REQUESTCHIPS = -3,
        NOTREADY = -2,
        READY = -1,
        MOVE = 0,
        TURN = 1
    }
}
