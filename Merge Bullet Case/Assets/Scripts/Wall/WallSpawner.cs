using Gameplay;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Wall
{
    public class WallSpawner : MonoSingleton<WallSpawner>
    {
        private LevelManager levelManager;
    
        public GameObject character;
        public Transform charactersparent;
    
    
        public GameObject[] walls;
        public int column;
        [HideInInspector] public int row;
        private GameObject tempWall;
        private int rndWallValue;
        public Vector3 startPos;
        public Transform wallsParent;

        private void Awake()
        {
            wallsParent = transform.GetChild(1);
        }

        private void Start()
        {
            levelManager = LevelManager.Instance;
        }


        #region WallSpawn
        //triggerred from GameBulletSpawnerSc
        public void SpawnWall()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    //Create Wall
                    rndWallValue = Random.Range(0, 100);
                    if (rndWallValue < 40)
                        tempWall = Instantiate(walls[0]);
                    else if (rndWallValue < 70)
                        tempWall = Instantiate(walls[1]);
                    else
                        tempWall = Instantiate(walls[2]);

                    //Set wall Pos
                    tempWall.transform.SetParent(wallsParent);
                    tempWall.transform.position = startPos + new Vector3(j * 2, 0, i * 1.5f);

                    //SetCharacters
                    if (i == row - 1)
                    {
                        GameObject tempChar = Instantiate(character);
                        tempChar.transform.SetParent(charactersparent);
                        tempChar.transform.position = tempWall.transform.position + Vector3.forward * 10;
                        levelManager.characterList.Add(tempChar.GetComponent<Character>());
                        character.transform.position = new Vector3(0, 0, tempWall.transform.position.z + 20);
                    }

                }
            }
            levelManager.DesignLevel(character.transform.position);
        }
        #endregion
    }
}
