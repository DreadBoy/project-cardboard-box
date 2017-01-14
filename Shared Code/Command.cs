using System;

namespace ProjectCardboardBox
{
    public class Command
    {
        public Action type;
        public int number1, number2;

        public Command(Action type, int number1, int number2)
        {
            this.type = type;
            this.number1 = number1;
            this.number2 = number2;
        }

        public Command(Action type, int number1)
        {
            this.type = type;
            this.number1 = number1;
        }

        public Command(Action type)
        {
            this.type = type;
        }

        public Command(string command)
        {
            var parts = command.Split(':'); ;
            type = (Action)Enum.Parse(typeof(Action), parts[0]);
            number1 = int.Parse(parts[1]);
            number2 = int.Parse(parts[2]);
        }

        public override string ToString()
        {
            return $"{type.ToString()}:{number1}:{number2}"; 
        }
    }
}
