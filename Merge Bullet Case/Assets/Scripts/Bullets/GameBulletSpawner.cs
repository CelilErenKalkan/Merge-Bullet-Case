using System.Collections;
using System.Collections.Generic;
using Editors;
using Gameplay;
using Managers;
using UnityEngine;
using Wall;

public class GameBulletSpawner : MonoBehaviour
{
    private LevelManager levelManager;
    private WallSpawner wallSpawner;
    
    private GameObject tempBullet;
    private Bullet tempBulletController, unbeatableBullet;
    private Transform startBulletsParent;
    public Vector3 startPos;

    private void Start()
    {
        levelManager = LevelManager.Instance;
        wallSpawner = WallSpawner.Instance;
        
        startBulletsParent = transform.GetChild(0);

        CreateInitialBullets();
    }

    void Update()
    {
        
    }

    private void CreateInitialBullets()
    {
        for (int i = 0; i < levelManager.dataBase.bulletSaves.Count; i++)
        {
            //Create Bullet
            tempBullet = Instantiate(levelManager.levelEditor.bulletDatas[levelManager.dataBase.bulletSaves[i].type - 1].prefab);
            tempBullet.transform.SetParent(startBulletsParent);

            //Set Bullet Properties
            tempBulletController = tempBullet.GetComponent<Bullet>();
            tempBulletController.bulletType = levelManager.dataBase.bulletSaves[i].type;
            tempBulletController.hp = levelManager.dataBase.bulletSaves[i].hp;
            tempBulletController.pos = levelManager.dataBase.bulletSaves[i].pos;
            tempBulletController.hitValue = levelManager.dataBase.bulletSaves[i].hitValue;
            tempBulletController.isGameBullet = true;

            //Select Unbeatable bullet
            if (unbeatableBullet == null)
                unbeatableBullet = tempBulletController;
            else
            {
                unbeatableBullet = tempBulletController.bulletType >= unbeatableBullet.bulletType ? tempBulletController : unbeatableBullet;
            }

            //Set Bullet Transform
            tempBullet.transform.localScale = tempBullet.transform.localScale * 2;
            tempBullet.transform.position =startPos+ new Vector3(tempBulletController.pos.x *2, 0, tempBulletController.pos.y*2);
            tempBullet.transform.rotation = Quaternion.Euler(90, 0, 0);

            //Start Move
            startBulletsParent.GetComponent<StartBullets>().isMoveForward = true;
        }
        unbeatableBullet.isUnbeatable = true;
        //Set WallLength
        wallSpawner.row = unbeatableBullet.hp + 2;
        wallSpawner.SpawnWall();
    }
}
