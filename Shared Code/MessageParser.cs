using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCardboardBox
{
    public class MessageParser
    {
        public static bool TryParse(ref NetDataReader reader, out MessageType type)
        {
            int t = reader.GetInt();
            if (Enum.IsDefined(typeof(MessageType), t))
            {
                type = (MessageType)t;
                return true;
            }
            type = MessageType.Unknown;
            return false;
        }
    }
}
