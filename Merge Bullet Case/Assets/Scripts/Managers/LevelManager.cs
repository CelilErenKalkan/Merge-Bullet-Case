using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Editors;
using Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        // private variables.
        private CameraManager cameraManager;
        private GameManager gameManager;
        private Pool pool;
        private StartBullets startBullets;
        
        [HideInInspector] public Vector3 playerStartPos;

        [Header("Character Variables")]
        private Character character;
        private bool isStartCharacterMovement;
        public List<Character> characterList = new List<Character>();

        [Header("Serialization")]
        public GameObject finishLine;
        public Transform platform;
        
        [Header("Level Editor and Database Variables")]
        public LevelEditor levelEditor;
        public DataBase dataBase;

        [Header("Bullet Variables")]
        public float bulletSize, fireRate;

        [Header("Doors Variables")]
        public int doorNum, doorDistance;
        private GameObject currentDoor;
        [SerializeField] private GameObject levelObject;

        [Header("Boxes Variables")]
        public int boxNum;
        public int boxDistance, boxHp;
        
        //Box
        private GameObject currenBox;
        private float boxXPos;

        [Header("Score Variables")]
        public int goldValue;
        public GameObject highScoreObject;

        private void Awake()
        {
            DOTween.SetTweensCapacity(500, 50);
        }

        private void OnEnable()
        {
            Actions.LevelStart += StartLevel;
        }
        
        private void OnDisable()
        {
            Actions.LevelStart -= StartLevel;
        }

        private void Start()
        {
            BulletEditor.SetBulletEditor();
            gameManager = GameManager.Instance;
            pool = Pool.Instance;
        }

        public void StartCharacterMovement(Character currentCharacter)
        {
            character = currentCharacter;
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

            //Set Bullet Settings
            startBullets = StartBullets.Instance;
            startBullets.isMoveForward = false;
            startBullets.gameObject.SetActive(false);

            //Set Character Settings
            for (var i = characterList.Count - 1; i >= 0; i--)
            {
                if (characterList[i].isPlay)
                {
                    var z = characterList[i].transform.position.z;
                    characterList[i].transform.DOMove(new Vector3(0, 1, z + 5), 0.95f);
                }
            }
            
            yield return new WaitForSeconds(1);
            
            cameraManager.SetTarget(character.transform);
            gameManager.SetPlayable(true);
        }

        private void StartLevel()
        {
            FileHandler.SaveToJson(dataBase, "data");  
            if (gameManager.bullets.Count > 0)
            {
                DOTween.KillAll();
                levelObject.SetActive(true);
                cameraManager = CameraManager.Instance;
                BulletEditor.SetBulletEditor();
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
            for (var i = 0; i < doorNum; i++)
            {
                currentDoor = pool.SpawnObject(Vector3.zero, PoolItemType.Doors, null);

                currentDoor.transform.position = Random.Range(0, 2) switch
                {
                    //Put Left
                    0 => new Vector3(-2.5f, -2.25f, playerStartPos.z + doorDistance + (i * doorDistance)),
                    //Put Right
                    1 => new Vector3(2.5f, -2.25f, playerStartPos.z + doorDistance + (i * doorDistance)),
                    _ => currentDoor.transform.position
                };
            }

            finishLine.transform.position = new Vector3(0, 0.5f, currentDoor.transform.position.z + doorDistance);
        }

        private void SetBoxes()
        {
            for (var i = 0; i < boxNum; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    currenBox = pool.SpawnObject(Vector3.zero, PoolItemType.Boxes, null);
                    if (currenBox.TryGetComponent(out Box box)) box.hp = boxHp + boxHp * i;

                    boxXPos = j switch
                    {
                        0 => -3.5f,
                        1 => 0,
                        _ => 3.5f
                    };
                    currenBox.transform.position = new Vector3(boxXPos, 2.55f, finishLine.transform.position.z + boxDistance + boxDistance * i);
                }
            }
            
            finishLine.transform.position = new Vector3(0, 0.5f, currenBox.transform.position.z + boxDistance * 3);
            platform.localScale = new Vector3(platform.localScale.x, platform.localScale.y, currenBox.transform.position.z + 100);
        }
        
        private void SetHighScore()
        {
            if (dataBase.highScore == 0) return;
            
            highScoreObject.transform.position = new Vector3(-5, 0, dataBase.highScore);
            highScoreObject.SetActive(true);
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