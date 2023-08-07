using Editors;
using Gameplay;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Wall
{
    public class Spawner : MonoSingleton<Spawner>
    {
        private LevelManager levelManager;
        private Pool pool;
    
        public GameObject character;
        private Transform charactersParent;
        private Transform startBulletsParent;
        private Transform wallsParent;

        public int column;
        [HideInInspector] public int row;
        public GameObject[] walls;
        private GameObject tempWall;
        private int rndWallValue;
        public Vector3 startPos;

        private void Start()
        {
            levelManager = LevelManager.Instance;
            pool = Pool.Instance;

            charactersParent = new GameObject
            {
                name = "CharactersParent",
                transform =
                {
                    parent = transform,
                    position = Vector3.zero
                }
            }.transform;
            
            startBulletsParent = new GameObject
            {
                name = "StartBulletsParent",
                transform =
                {
                    parent = transform,
                    position = Vector3.zero
                }
            }.AddComponent<StartBullets>().transform;

            wallsParent = new GameObject
            {
                name = "WallsParent",
                transform =
                {
                    parent = transform,
                    position = Vector3.zero
                }
            }.transform;

            row = BulletEditor.CreateInitialBullets(startBulletsParent);
            SpawnWall();
        }
        
        private void SpawnWall()
        {
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    //Create Wall
                    rndWallValue = Random.Range(0, 100);
                    int childIndex;
                    if (rndWallValue < 40)
                        childIndex = 0;
                    else if (rndWallValue < 70)
                        childIndex = 1;
                    else
                        childIndex = 2;

                    tempWall = pool.SpawnObject(startPos + new Vector3(j * 2, 0, i * 1.5f), PoolItemType.Walls, wallsParent, childIndex);

                    //Set Characters
                    if (i == row - 1)
                    {
                        var tempChar = pool.SpawnObject(tempWall.transform.position + Vector3.forward * 10,
                            PoolItemType.Character, charactersParent);
                        if (tempChar.TryGetComponent(out Character ch)) levelManager.characterList.Add(ch);
                        character.transform.position = new Vector3(0, 0, tempWall.transform.position.z + 20);
                    }

                }
            }
            
            levelManager.DesignLevel(character.transform.position);
        }
    }
}
