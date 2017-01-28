using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCardboardBox
{
    public class Chip
    {
        public string value;
        public Type type;

        public enum Type
        {
            Action,
            Number
        }

        public Chip(string chip)
        {
            var parts = chip.Split(':');
            type = (Type)Enum.Parse(typeof(Type), parts[0]);
            value = parts[1];
        }

        public Chip(Type type, string value)
        {
            this.type = type;
            this.value = value;
        }

        public override string ToString()
        {
            return $"{type.ToString()}:{value}";
        }
    }
}
