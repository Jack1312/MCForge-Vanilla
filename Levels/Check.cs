using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Levels
{
    public class Check
    {
        public int b;
        public byte time;
        public string extraInfo = "";
        public Check(int b, string extraInfo = "")
        {
            this.b = b;
            time = 0;
            this.extraInfo = extraInfo;
        }
    }
}
