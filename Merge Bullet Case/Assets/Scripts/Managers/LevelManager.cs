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
        private CameraManager cameraManager;
        private GameManager gameManager;
        private Pool pool;
        private StartBullets startBullets;

        // Collected money and player start position
        [HideInInspector] public int collectedMoney;
        [HideInInspector] public Vector3 playerStartPos;

        // Character variables
        [Header("Character Variables")] private Character character;
        public bool isStartCharacterMovement, isTripleShot;
        public List<Character> characterList = new List<Character>();

        // Serialization variables
        [Header("Serialization")] public GameObject finishLine;
        public Transform platform;

        // Level Editor and Database variables
        [Header("Level Editor and Database Variables")]
        public LevelEditor levelEditor;
        public DataBase dataBase;

        // Bullet variables
        [Header("Bullet Variables")] public float bulletSize, fireRate;

        // Doors variables
        [Header("Doors Variables")] public int doorNum, doorDistance;
        private GameObject currentDoor;
        [SerializeField] private GameObject levelObject;

        // Boxes variables
        [Header("Boxes Variables")] public int boxNum;
        public int boxDistance, boxHp;
        private GameObject currenBox;
        private float boxXPos;

        // Score variables
        [Header("Score Variables")] public int goldValue;
        public GameObject highScoreObject;

        private void Awake()
        {
            DOTween.SetTweensCapacity(500, 50);
            
            gameManager = GameManager.Instance;
            pool = Pool.Instance;
            
            LoadData();
            BulletEditor.SetBulletEditor();
        }

        private void OnEnable()
        {
            Actions.LevelStart += StartLevel;
        }

        private void OnDisable()
        {
            Actions.LevelStart -= StartLevel;
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

            startBullets = StartBullets.Instance;
            startBullets.isMoveForward = false;
            startBullets.gameObject.SetActive(false);

            float characterCount = 0;
            bool isRight = true;
            
            for (var i = characterList.Count - 1; i >= 0; i--)
            {
                if (characterList[i].isPlay)
                {
                    var z = characterList[i].transform.position.z;
                    float x;
                    if (isRight)
                    {
                        isRight = false;
                        x = characterCount * 1.5f;
                    }
                    else
                    {
                        isRight = true;
                        characterCount++;
                        x = characterCount * -1.5f;
                    }
                    characterList[i].transform.DOMove(new Vector3(x, 1, z + 5), 0.95f);
                }
            }

            yield return new WaitForSeconds(1);

            cameraManager.SetTarget(character.transform);
            gameManager.SetPlayable(true);
        }

        private void StartLevel()
        {
            SaveSystem();

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
            var isTripleShotDoor = false;

            for (var i = 0; i < doorNum; i++)
            {
                int randomDoorType;
                if (!isTripleShotDoor)
                {
                    var tripleChance = Random.Range(0, 100);
                    if (tripleChance <= 40)
                        isTripleShotDoor = true;
                    randomDoorType = 3;
                }
                else
                {
                    randomDoorType = Random.Range(0, 2);
                }

                currentDoor = pool.SpawnObject(Vector3.zero, PoolItemType.Doors, null, randomDoorType);

                currentDoor.transform.position = Random.Range(0, 2) switch
                {
                    0 => new Vector3(-2.5f, -2.25f, playerStartPos.z + doorDistance + (i * doorDistance)),
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
                    currenBox.transform.position = new Vector3(boxXPos, 2.55f,
                        finishLine.transform.position.z + boxDistance + boxDistance * i);
                }
            }
            finishLine.transform.position = new Vector3(0, 0.5f, currenBox.transform.position.z + boxDistance * 3);
            platform.localScale = new Vector3(platform.localScale.x, platform.localScale.y,
                currenBox.transform.position.z + 100);
        }

        private void SetHighScore()
        {
            if (dataBase.highScore == 0) return;

            highScoreObject.transform.position = new Vector3(-5, 0, dataBase.highScore);
            highScoreObject.SetActive(true);
        }

        public void SaveSystem()
        {
            dataBase ??= new DataBase
            {
                money = levelEditor.startMoney
            };

            dataBase.bulletDataList.Clear();

            foreach (var bullet in gameManager.bullets)
            {
                if (bullet == null)
                {
                    Debug.LogWarning("Null bullet found. Skipping save for this bullet.");
                    continue;
                }
                dataBase.bulletDataList.Add(new DataBase.BulletData
                {
                    type = bullet.bulletType,
                    gridNum = bullet.gridNum,
                    position = bullet.pos,
                    healthPoints = bullet.hp,
                    hitValue = bullet.hitValue
                });
            }
            
            FileHandler.SaveToJson(dataBase, "data.json");
        }

        private void LoadData()
        {
            dataBase = FileHandler.ReadFromJson<DataBase>("data.json");

            if (dataBase != null) return;
            SaveSystem();
            dataBase.money = 1000;
        }
    }
}