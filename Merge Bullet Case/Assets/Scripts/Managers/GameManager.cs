using System.Collections.Generic;
using Editors;
using Gameplay;
using UnityEngine;
using Utils;
using Grid = UnityEngine.Grid;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private LevelManager levelManager;
        private GridCreator gridCreator;

        [HideInInspector] public bool isPlayable;
    
        private GameObject currentBullet;
        private Bullet currentBulletController;
        public List<Bullet> bullets = new List<Bullet>();

        private void Start()
        {
            levelManager = LevelManager.Instance;
            gridCreator = GridCreator.Instance;
            CreateBullets();
        }

        private void Update()
        {
        
        }

        public void SetPlayable(bool isSet)
        {
            isPlayable = isSet;
        }

        private void CreateBullets()
        {
            foreach (var bulletSave in levelManager.dataBase.bulletSaves)
            {
                GameObject bulletPrefab = levelManager.levelEditor.bulletDatas[bulletSave.type - 1].prefab;
                GameObject currentBulletObject = Instantiate(bulletPrefab, gridCreator.grids[bulletSave.GridNum].transform, true);
                Bullet currentBulletScript = currentBulletObject.GetComponent<Bullet>();
        
                // Set bullet properties
                bullets.Add(currentBulletScript);
                currentBulletScript.bulletType = levelManager.levelEditor.bulletDatas[bulletSave.type - 1].type;
                currentBulletScript.hitValue = levelManager.levelEditor.bulletDatas[bulletSave.type - 1].hitValue;
                currentBulletScript.hp = levelManager.levelEditor.bulletDatas[bulletSave.type - 1].hp;
        
                // Get and set grid controller
                var currentGridController = gridCreator.grids[bulletSave.GridNum].GetComponent<Utils.Grid>();
                currentBulletScript.GetGridController();
                currentBulletScript.gridNum = bulletSave.GridNum;
                currentBulletScript.pos = currentBulletScript.currentGridController.pos;

                // Set Grid Settings
                currentGridController.bulletType = currentBulletScript.bulletType;
                currentGridController.gridSit = GridSit.Fill;
                gridCreator.emptyGrids.Remove(currentGridController.gameObject);
        
                // Parent bullet to the grid
                currentBulletObject.transform.SetParent(currentGridController.transform);
                currentBulletObject.transform.localPosition = Vector3.zero;
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
            mergedBullet.bulletType = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].type;
            mergedBullet.hitValue = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].hitValue;
            mergedBullet.hp = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].hp;
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
    }
}
