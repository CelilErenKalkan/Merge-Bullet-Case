using Gameplay;
using Managers;
using UnityEngine;
using Utils;
using Grid = Utils.Grid;

namespace Editors
{
    public static class BulletEditor
    {
        private static LevelManager levelManager;
        private static GameManager gameManager;
        private static GridManager gridManager;
        private static CameraManager cameraManager;
        private static Pool pool;

        private static Bullet unbeatableBullet;

        public static void SetBulletEditor()
        {
            levelManager = LevelManager.Instance;
            gameManager = GameManager.Instance;
            gridManager = GridManager.Instance;
            cameraManager = CameraManager.Instance;
            pool = Pool.Instance;
        }
        
        public static void CreateBullets()
        {
            foreach (var bulletSave in levelManager.dataBase.bulletSaves)
            {
                GameObject currentBulletObject = pool.SpawnObject(gridManager.grids[bulletSave.GridNum].transform.position, PoolItemType.Bullets,gridManager.grids[bulletSave.GridNum].transform, bulletSave.type - 1);
                Bullet currentBulletScript = currentBulletObject.GetComponent<Bullet>();
        
                // Set bullet properties
                gameManager.bullets.Add(currentBulletScript);
                currentBulletScript.bulletType = levelManager.levelEditor.bulletDatas[bulletSave.type - 1].type;
                currentBulletScript.hitValue = levelManager.levelEditor.bulletDatas[bulletSave.type - 1].hitValue;
                currentBulletScript.hp = levelManager.levelEditor.bulletDatas[bulletSave.type - 1].hp;
        
                // Get and set grid controller
                var currentGridController = gridManager.grids[bulletSave.GridNum].GetComponent<Grid>();
                currentBulletScript.GetGrid();
                currentBulletScript.gridNum = bulletSave.GridNum;
                currentBulletScript.pos = currentBulletScript.currentGrid.pos;

                // Set Grid Settings
                currentGridController.bulletType = currentBulletScript.bulletType;
                currentGridController.gridSit = GridSit.Fill;
                gridManager.emptyGrids.Remove(currentGridController.gameObject);
        
                // Parent bullet to the grid
                currentBulletObject.transform.SetParent(currentGridController.transform);
                currentBulletObject.transform.localPosition = Vector3.zero;
            }
        }
        
        public static int CreateInitialBullets(Transform startBulletsParent)
        {
            for (var i = 0; i < levelManager.dataBase.bulletSaves.Count; i++)
            {
                //Create Bullet
                var tempBullet = pool.SpawnObject(levelManager.dataBase.bulletSaves[i].pos, PoolItemType.Bullets,
                    startBulletsParent, levelManager.dataBase.bulletSaves[i].type - 1);
                if (tempBullet.TryGetComponent(out Bullet bullet))

                //Set Bullet Properties
                bullet.bulletType = levelManager.dataBase.bulletSaves[i].type;
                bullet.hp = levelManager.dataBase.bulletSaves[i].hp;
                bullet.pos = levelManager.dataBase.bulletSaves[i].pos;
                bullet.hitValue = levelManager.dataBase.bulletSaves[i].hitValue;
                bullet.isGameBullet = true;

                //Select Unbeatable bullet
                if (unbeatableBullet == null)
                    unbeatableBullet = bullet;
                else 
                    unbeatableBullet = bullet.bulletType >= unbeatableBullet.bulletType ? bullet : unbeatableBullet;

                //Set Bullet Transform
                tempBullet.transform.localScale *= 2;
                tempBullet.transform.position = new Vector3(-4, 1, 0) + new Vector3(bullet.pos.x * 2, 0, bullet.pos.y * 2);

                //Start Move
                if (startBulletsParent.TryGetComponent(out StartBullets startBullets))
                    startBullets.isMoveForward = true;
            }
            
            unbeatableBullet.isUnbeatable = true;
            cameraManager.SetTarget(unbeatableBullet.transform);
            
            //Set Wall Length
            return unbeatableBullet.hp / 2;
        }
        
        public static void AddBullet()
        {
            if (gridManager.emptyGrids.Count > 0 &&
                levelManager.dataBase.money >= levelManager.levelEditor.bulletPrice)
            {
                levelManager.dataBase.money -= levelManager.levelEditor.bulletPrice;
                int rndGrid = Random.Range(0, gridManager.emptyGrids.Count);
                
                var tempBullet = pool.SpawnObject(Vector3.zero, PoolItemType.Bullets, null,
                    levelManager.levelEditor.creatingBulletType - 1);
                var bullet = tempBullet.GetComponent<Bullet>();
                var tempGrid = gridManager.emptyGrids[rndGrid];
                var grid = tempGrid.GetComponent<Grid>();

                //Set grid properties
                grid.bulletType = levelManager.levelEditor.bulletDatas[levelManager.levelEditor.creatingBulletType - 1]
                    .type;
                gridManager.emptyGrids.Remove(tempGrid);
                grid.gridSit = GridSit.Fill;

                //Set bullet properties
                tempBullet.transform.SetParent(tempGrid.transform);
                tempBullet.transform.localPosition = Vector3.zero;
                bullet.gridNum = gridManager.grids.IndexOf(tempGrid);
                bullet.bulletType = levelManager.levelEditor.creatingBulletType;
                bullet.hitValue = levelManager.levelEditor.bulletDatas[levelManager.levelEditor.creatingBulletType - 1]
                    .hitValue;
                bullet.hp = levelManager.levelEditor.bulletDatas[levelManager.levelEditor.creatingBulletType - 1].hp;
                bullet.pos = grid.pos;
                bullet.GetGrid();

                gameManager.bullets.Add(bullet);
                
                levelManager.SaveSystem();
            }
        }
        
        public static void Merge(Bullet firstBullet, Bullet secondBullet)
        {
            if (firstBullet == null || secondBullet == null)
            {
                Debug.LogError("Cannot merge null bullets.");
                return;
            }

            // Reset the grid of the first bullet
            firstBullet.ResetGrid();
            secondBullet.ResetGrid();

            // Create the merged bullet
            GameObject mergedBulletObject = Pool.Instance.SpawnObject(secondBullet.transform.position,
                PoolItemType.Bullets, secondBullet.transform.parent, firstBullet.bulletType);

            if (!mergedBulletObject.TryGetComponent(out Bullet bullet)) return;
            Bullet mergedBullet = bullet;
            
            // Set properties of the merged bullet
            mergedBullet.bulletType = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].type;
            mergedBullet.hitValue = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].hitValue;
            mergedBullet.hp = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].hp;
            mergedBullet.GetGrid();
            gameManager.bullets.Add(mergedBullet);
            
            // Remove the merged bullets from the bullets list
            gameManager.bullets.Remove(firstBullet);
            gameManager.bullets.Remove(secondBullet);

            // Deactivate the merged bullets
            pool.DeactivateObject(firstBullet.gameObject, PoolItemType.Bullets);
            pool.DeactivateObject(secondBullet.gameObject, PoolItemType.Bullets);
        }
    }
}
