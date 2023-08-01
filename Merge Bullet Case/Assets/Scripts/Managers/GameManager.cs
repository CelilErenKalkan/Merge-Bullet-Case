using System.Collections.Generic;
using System.IO;
using Editors;
using Gameplay;
using UnityEngine;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public LevelEditor levelEditor;
        public DataBase dataBase;
        private GridCreator gridCreator;
    
        private GameObject currentBullet;
        private Bullet currentBulletController;
        public List<Bullet> bullets = new List<Bullet>();

        private void Awake()
        {
            ObjectManager.GameManager = this;
        }

        private void Start()
        {
            gridCreator = ObjectManager.GridCreator;
            CreateBullets();
        }

        private void Update()
        {
        
        }

        public void CreateBullets()
        {
            foreach (var bulletSave in dataBase.bulletSaves)
            {
                GameObject bulletPrefab = levelEditor.bulletDatas[bulletSave.type - 1].prefab;
                GameObject currentBullet = Instantiate(bulletPrefab);
                Bullet currentBulletController = currentBullet.GetComponent<Bullet>();
        
                // Set bullet properties
                bullets.Add(currentBulletController);
                currentBulletController.bulletType = levelEditor.bulletDatas[bulletSave.type - 1].type;
                currentBulletController.hitValue = levelEditor.bulletDatas[bulletSave.type - 1].hitValue;
                currentBulletController.hp = levelEditor.bulletDatas[bulletSave.type - 1].hp;
        
                // Get and set grid controller
                GridController currentGridController = gridCreator.grids[bulletSave.GridNum].GetComponent<GridController>();
                currentBulletController.GetGridController();
                currentBulletController.gridNum = bulletSave.GridNum;
                currentBulletController.pos = currentBulletController.currentGridController.pos;

                // Set Grid Settings
                currentGridController.bulletType = currentBulletController.bulletType;
                currentGridController.gridSit = GridSit.Fill;
                gridCreator.emptyGrids.Remove(currentGridController.gameObject);
        
                // Parent bullet to the grid
                currentBullet.transform.SetParent(currentGridController.transform);
                currentBullet.transform.localPosition = Vector3.zero;
            }
        }


        public void Merge(Bullet firstBullet, Bullet secondBullet)
        {
            if (firstBullet == null || secondBullet == null)
            {
                Debug.LogError("Cannot merge null bullets.");
                return;
            }

            // Reset the grid of the first bullet
            firstBullet.ResetGrid();

            // Set properties of the merged bullet
            Bullet mergedBullet = firstBullet;
            mergedBullet.bulletType = levelEditor.bulletDatas[firstBullet.bulletType].type;
            mergedBullet.hitValue = levelEditor.bulletDatas[firstBullet.bulletType].hitValue;
            mergedBullet.hp = levelEditor.bulletDatas[firstBullet.bulletType].hp;
            mergedBullet.pos = secondBullet.currentGridController.pos;
            mergedBullet.GetGridController();

            // Update the grid properties of the second bullet's current grid
            secondBullet.currentGridController.bulletType = mergedBullet.bulletType;

            // Remove the merged bullets from the bullets list
            List<Bullet> bulletsToRemove = new List<Bullet> { firstBullet, secondBullet };
            bullets.RemoveAll(b => bulletsToRemove.Contains(b));

            // Optionally, you may destroy the original bullets
            Destroy(firstBullet.gameObject);
            Destroy(secondBullet.gameObject);
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
            foreach (var bullet in bullets)
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
