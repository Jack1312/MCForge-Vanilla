using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge
{
    public enum LevelPermission : int
    {
        Banned = -20,
        Guest = 0,
        Builder = 30,
        AdvBuilder = 50,
        Operator = 80,
        Admin = 100,
        Nobody = 120,
        Null = 150
    }
}
