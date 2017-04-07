using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCardboardBox
{
    //http://www.somersault1824.com/wp-content/uploads/2015/02/color-blindness-palette.png
    public static class Colours
    {
        public const string Black = "000000"; //1
        public const string Cyan = "004949"; //2
        public const string Pink = "ff6db6"; //4
        public const string Violet = "490092"; //6
        public const string Blue = "006ddb"; //7
        public const string Red = "920000"; //11
        public const string Brown = "924900"; //12
        public const string Orange = "db6d00"; //13
        public const string Green = "24ff24"; //14
        public const string Yellow = "ffff6d"; //15

        public static readonly string[] AllColours = new string[] { Black, Cyan, Pink, Violet, Black, Red, Brown, Orange, Green, Yellow };
    }
}
