using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Editors;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Grid = Utils.Grid;

namespace Managers
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        private Character character;
        private CameraManager cameraManager;
        private GameManager gameManager;
        private GridCreator gridCreator;
        public LevelEditor levelEditor;
        public DataBase dataBase;

        private bool isStartCharacterMovement;
        public List<Character> characterList = new List<Character>();
        public Transform camPos;


        private StartBullets startBullets;
        private int GunNum;
        public float bulletSize, fireRate;


        [HideInInspector] public Vector3 playerStartPos;
        public GameObject finishLine;
        public Transform platform;

        //Door
        public int doorNum, doorDistance;
        private int doorRndPos, rndDoor;
        public GameObject[] doors;
        private GameObject currentDoor;

        //Box
        public int boxNum, boxDistance, boxHp;
        public GameObject boxPrefab;
        private GameObject currenBox;
        private float boxXPos;
        public int goldValue;

        //High Score
        public GameObject highScoreObject;

        private void Awake()
        {
            DOTween.SetTweensCapacity(500, 50);
        }

        private void Start()
        {
            character = Character.Instance;
            cameraManager = CameraManager.Instance;
            gameManager = GameManager.Instance;
            gridCreator = GridCreator.Instance;
        }

        public void StartCharacterMovement()
        {
            if (!isStartCharacterMovement)
            {
                isStartCharacterMovement = true;
                StartCoroutine(WaitCharacterMovement());
            }
        }

        private IEnumerator WaitCharacterMovement()
        {
            cameraManager.isFollow = false;
            yield return new WaitForSeconds(0.75f);
            
            //Set cam Settings
            cameraManager.transform.DOMove(camPos.position, 0.95f);
            cameraManager.transform.DORotate(camPos.transform.eulerAngles, 0.75f);

            //Set Bullet Settings
            startBullets = StartBullets.Instance;
            startBullets.isMoveForward = false;
            startBullets.gameObject.SetActive(false);

            //Set Character Settings
            for (int i = characterList.Count - 1; i >= 0; i--)
            {
                if (characterList[i].isPlay)
                {
                    characterList[i].transform.SetParent(character.transform.GetChild(0));
                    characterList[i].transform.DOMove(character.transform.GetChild(1).GetChild(GunNum).position, 0.95f);
                    GunNum++;
                    characterList[i].GetComponent<Collider>().enabled = false;
                }
            }
            yield return new WaitForSeconds(1);
            cameraManager.SetTarget(character.transform);
            gameManager.SetPlayable(true);
        }

        public void StartLevel()
        {
            FileHandler.SaveToJson(dataBase, "data");  
            if (gameManager.bullets.Count > 0)
            {
                DOTween.KillAll();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        
        public void DesignLevel(Vector3 playerPos)
        {
            playerStartPos = playerPos;
            SetDoors();
            SetBoxes();
            SetHighScore();
        }

        private void SetDoors()
        {
            for (int i = 0; i < doorNum; i++)
            {
                rndDoor = Random.Range(0, doors.Length);
                currentDoor = Instantiate(doors[rndDoor], null, true);

                doorRndPos = Random.Range(0, 2);

                if (doorRndPos == 0) //Put Left
                    currentDoor.transform.position = new Vector3(-2.5f, -2.25f, playerStartPos.z + doorDistance + (i * doorDistance));
                if (doorRndPos == 1) //Put Right
                    currentDoor.transform.position = new Vector3(2.5f, -2.25f, playerStartPos.z + doorDistance + (i * doorDistance));
            }

            finishLine.transform.position = new Vector3(0, 0.5f, currentDoor.transform.position.z + doorDistance);
        }

        private void SetBoxes()
        {
            for (int i = 0; i < boxNum; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    currenBox = Instantiate(boxPrefab, null, true);
                    currenBox.GetComponent<Box>().hp = boxHp + boxHp * i;

                    if (j == 0)
                        boxXPos = -3.5f;
                    else if (j == 1)
                        boxXPos = 0;
                    else
                        boxXPos = 3.5f;
                    currenBox.transform.position = new Vector3(boxXPos, 2.55f, finishLine.transform.position.z + boxDistance + boxDistance * i);
                }
            }
            
            finishLine.transform.position = new Vector3(0, 0.5f, currenBox.transform.position.z + boxDistance * 3);
            platform.localScale = new Vector3(platform.localScale.x, platform.localScale.y, currenBox.transform.position.z + 100);
        }
        
        private void SetHighScore()
        {
            if (dataBase.highScore != 0)
            {
                highScoreObject.transform.position = new Vector3(-5, 0, dataBase.highScore);
                highScoreObject.SetActive(true);
            }
        }
        
        public void OpenMergeScene()
        {
            FileHandler.SaveToJson(dataBase, "data");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        
        public void SaveSystem()
        {
            if (dataBase == null)
            {
                Debug.LogError("No dataBase reference found. Unable to save.");
                return;
            }

            // Clear the existing bullet saves (if needed)
            dataBase.bulletSaves.Clear();

            // Save the bullet data
            foreach (var bullet in gameManager.bullets)
            {
                if (bullet == null)
                {
                    Debug.LogWarning("Null bullet found. Skipping save for this bullet.");
                    continue;
                }

                dataBase.bulletSaves.Add(new DataBase.BulletSave
                {
                    type = bullet.bulletType,
                    GridNum = bullet.gridNum,
                    pos = bullet.pos,
                    hp = bullet.hp,
                    hitValue = bullet.hitValue
                });
            }

            // Save the dataBase to a JSON file
            FileHandler.SaveToJson(dataBase, "data");
        }
    }
}