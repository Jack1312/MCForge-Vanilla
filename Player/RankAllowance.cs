using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge
{
    public class RankAllowance
    {
        public string commandName;
        public LevelPermission lowestRank;
        public List<LevelPermission> disallow = new List<LevelPermission>();
        public List<LevelPermission> allow = new List<LevelPermission>();
    }
}
