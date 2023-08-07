using Editors;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class Spawner : MonoSingleton<Spawner>
    {
        // References to managers
        private LevelManager levelManager;
        private Pool pool;

        // Parent transforms for organization
        private Transform charactersParent;
        private Transform startBulletsParent;
        private Transform wallsParent;

        // Prefabs and spawner settings
        public GameObject character;
        public GameObject[] walls;
        public int column;
        [HideInInspector] public int row;
        public Vector3 startPos;

        private GameObject tempWall;
        private int rndWallValue;

        private void Start()
        {
            // Initialize references to managers
            levelManager = LevelManager.Instance;
            pool = Pool.Instance;

            // Create parent transforms
            CreateParentTransforms();

            // Create initial bullets and spawn walls
            row = BulletEditor.CreateInitialBullets(startBulletsParent);
            SpawnWall();
        }

        private void CreateParentTransforms()
        {
            // Create parent transforms for organization
            charactersParent = CreateParentTransform("CharactersParent");
            startBulletsParent = CreateParentTransform("StartBulletsParent").gameObject.AddComponent<StartBullets>().transform;
            wallsParent = CreateParentTransform("WallsParent");
        }

        private Transform CreateParentTransform(string name)
        {
            return new GameObject
            {
                name = name,
                transform =
                {
                    parent = transform,
                    position = Vector3.zero
                }
            }.transform;
        }

        private void SpawnWall()
        {
            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    // Create Wall
                    rndWallValue = Random.Range(0, 100);
                    int childIndex;
                    if (rndWallValue < 40)
                        childIndex = 0;
                    else if (rndWallValue < 70)
                        childIndex = 1;
                    else
                        childIndex = 2;

                    tempWall = pool.SpawnObject(startPos + new Vector3(j * 2, 0, i * 1.5f), PoolItemType.Walls, wallsParent, childIndex);

                    // Set Characters
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