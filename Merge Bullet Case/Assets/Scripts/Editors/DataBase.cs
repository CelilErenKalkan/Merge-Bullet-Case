using System.Collections.Generic;
using UnityEngine;

namespace Editors
{
    [System.Serializable]
    public class DataBase
    {
        [System.Serializable]
        public struct BulletData
        {
            public int type;
            public int gridNum;
            public int healthPoints;
            public int hitValue;
            public Vector2 position;
        }

        // List of saved bullet data
        public List<BulletData> bulletDataList = new List<BulletData>();

        // Player's in-game currency
        public int money;

        // Player's high score
        public float highScore;
    }
}