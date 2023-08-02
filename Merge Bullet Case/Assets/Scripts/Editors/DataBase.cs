using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBase")]
public class DataBase : ScriptableObject
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