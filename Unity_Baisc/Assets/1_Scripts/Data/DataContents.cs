using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Data
{
    // Stat Data
    #region Stat

    [Serializable]
    public class Stat
    {
        public int level;
        public int hp;
        public int attack;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat>
    {
        public List<Stat> Stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDic() => Stats.ToDictionary(x => x.level, x => x);
    }

    #endregion
}
