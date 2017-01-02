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
            READY,
            MOVE,
            TURN
        }
        public Type type;
        public int number1, number2;

        public Command(Type type, int number1, int number2)
        {
            this.type = type;
            this.number1 = number1;
            this.number2 = number2;
        }

        public Command(Type type, int number1)
        {
            this.type = type;
            this.number1 = number1;
        }

        public Command(Type type)
        {
            this.type = type;
        }
    }
}
