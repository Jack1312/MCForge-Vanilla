using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Levels
{
    public class Update
    {
        public int b;
        public byte type;
        public string extraInfo = "";
        public Update(int b, byte type, string extraInfo = "")
        {
            this.b = b;
            this.type = type;
            this.extraInfo = extraInfo;
        }
    }
}
