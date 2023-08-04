using System.Collections.Generic;
using UnityEngine;

namespace Editors
{
    [System.Serializable]
    public class DataBase
    {
        [System.Serializable]
        public struct BulletSave
        {
            public int type;
            public int GridNum;
            public int hp;
            public int hitValue;
            public Vector2 pos;
        }

        public List<BulletSave> bulletSaves = new List<BulletSave>();
        public int money;
        public float highScore;
    }
}