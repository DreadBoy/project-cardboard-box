using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectCardboardBox
{
    public class Command
    {
        public Action type;
        public int number;


        public Command(Action type, int number)
        {
            this.type = type;
            this.number = number;
        }

        public Command(Action type)
        {
            this.type = type;
        }

        public Command(string command)
        {
            var parts = command.Split(':'); ;
            type = (Action)Enum.Parse(typeof(Action), parts[0]);
            number = int.Parse(parts[1]);
        }

        public override string ToString()
        {
            return $"{type.ToString()}:{number}";
        }

        public static bool TryParse(ref List<Command> commands, Chip[] hand)
        {
            commands = new List<Command>();
            List<Chip> buffer = new List<Chip>();

            for (int i = 0; i < hand.Length; i++)
            {
                var chip = hand[i];

                if (chip.type == Chip.Type.Action)
                {
                    //First chip needs to be Action
                    if (i == 0)
                    {
                        buffer.Add(chip);
                        continue;
                    }
                    //Can't have two Actions in a row
                    else if (buffer.Last().type == Chip.Type.Action)
                        return false;


                    //At that point we have Action and some numbers already in buffer.
                    //First flush the buffer
                    int number = 0;
                    foreach (var curr in buffer)
                        if (curr.type == Chip.Type.Number)
                            number += int.Parse(curr.value);
                    commands.Add(new Command((Action)Enum.Parse(typeof(Action), buffer[0].value), number));
                    buffer.Clear();

                    buffer.Add(chip);
                }
                else if (chip.type == Chip.Type.Number)
                {
                    //If first chip is number, fail parsing
                    if (i == 0)
                        return false;
                    buffer.Add(chip);
                }
            }

            if (buffer.Last().type == Chip.Type.Number)
            {
                //We are the end and we still have valid command in buffer
                //Flush the buffer before you exit method
                int number = 0;
                foreach (var curr in buffer)
                    if (curr.type == Chip.Type.Number)
                        number += int.Parse(curr.value);
                commands.Add(new Command((Action)Enum.Parse(typeof(Action), buffer[0].value), number));
            }
            else if(buffer.Last().type == Chip.Type.Action)
            {
                //We can't finish with command being the last chip
                commands.Clear();
                return false;
            }

            return true;
        }
    }
}
