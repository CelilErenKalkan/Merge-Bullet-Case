using UnityEngine;

namespace Editors
{
    [CreateAssetMenu(fileName ="LevelEditor")]
    public class LevelEditor : ScriptableObject
    {
        [System.Serializable]
        public struct BulletData
        {
            public string name;
            public int hitValue;
            public int type;
            public int hp;
            public GameObject prefab;
        }

        public BulletData[] bulletDatas;

        public int gridRow,gridColumn;
        public Vector3 gridStartPoint;
        public int bulletPrice,creatingBulletType;
        public int startMoney;
    }
}