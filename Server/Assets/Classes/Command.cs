using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardboard
{
    public class Command
    {
        public enum Type
        {
            MOVE,
            TURN
        }
        public int number1, number2;
    }
}
