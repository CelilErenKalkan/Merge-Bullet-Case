using DG.Tweening;
using Gameplay;
using Managers;
using UnityEngine;
using Utils;
using Grid = Utils.Grid;

namespace Editors
{
    public static class BulletEditor
    {
        // Managers and instances
        private static LevelManager levelManager;
        private static GameManager gameManager;
        private static GridManager gridManager;
        private static CameraManager cameraManager;
        private static Pool pool;

        private static Bullet unbeatableBullet;

        // Initialize manager references
        public static void SetBulletEditor()
        {
            levelManager = LevelManager.Instance;
            gameManager = GameManager.Instance;
            gridManager = GridManager.Instance;
            cameraManager = CameraManager.Instance;
            pool = Pool.Instance;
        }

        // Create bullets on the grids
        public static void CreateBullets()
        {
            foreach (var bulletData in levelManager.dataBase.bulletDataList)
            {
                GameObject bulletObject = pool.SpawnObject(
                    gridManager.grids[bulletData.gridNum].transform.position,
                    PoolItemType.Bullets, gridManager.grids[bulletData.gridNum].transform, bulletData.type - 1);
                Bullet bulletScript = bulletObject.GetComponent<Bullet>();

                // Set bullet properties
                gameManager.bullets.Add(bulletScript);
                bulletScript.bulletType = levelManager.levelEditor.bulletDatas[bulletData.type - 1].type;
                bulletScript.hitValue = levelManager.levelEditor.bulletDatas[bulletData.type - 1].hitValue;
                bulletScript.hp = levelManager.levelEditor.bulletDatas[bulletData.type - 1].hp;

                // Get and set grid controller
                var targetGrid = gridManager.grids[bulletData.gridNum].GetComponent<Grid>();
                bulletScript.gridNum = bulletData.gridNum;

                // Set Grid Settings
                targetGrid.bulletType = bulletScript.bulletType;
                targetGrid.gridSit = GridSit.Fill;
                gridManager.emptyGrids.Remove(targetGrid.gameObject);

                // Parent bullet to the grid
                bulletObject.transform.SetParent(targetGrid.transform);
                bulletScript.GetGrid();
                bulletScript.pos = bulletScript.currentGrid.pos;
                bulletObject.transform.localPosition = Vector3.zero;
                bulletScript.transform.DORotate(new Vector3(60, -90, -90), 0.1f);
            }
        }

        // Create initial bullets
        public static int CreateInitialBullets(Transform startBulletsParent)
        {
            for (var i = 0; i < levelManager.dataBase.bulletDataList.Count; i++)
            {
                // Create new bullet
                var newBulletObject = pool.SpawnObject(levelManager.dataBase.bulletDataList[i].position, PoolItemType.Bullets,
                    startBulletsParent, levelManager.dataBase.bulletDataList[i].type - 1);
                
                if (newBulletObject.TryGetComponent(out Bullet newBullet))
                {
                    // Set new bullet properties
                    newBullet.bulletType = levelManager.dataBase.bulletDataList[i].type;
                    newBullet.hp = levelManager.dataBase.bulletDataList[i].healthPoints;
                    newBullet.pos = levelManager.dataBase.bulletDataList[i].position;
                    newBullet.hitValue = levelManager.dataBase.bulletDataList[i].hitValue;
                    newBullet.isGameBullet = true;

                    // Select Unbeatable bullet
                    if (unbeatableBullet == null)
                        unbeatableBullet = newBullet;
                    else
                        unbeatableBullet = newBullet.bulletType >= unbeatableBullet.bulletType ? newBullet : unbeatableBullet;

                    // Set Bullet Transform
                    newBulletObject.transform.localScale *= 2;
                    newBulletObject.transform.position = new Vector3(-4, 1, 0) + new Vector3(newBullet.pos.x * 2, 0, newBullet.pos.y * 2);

                    // Start Move
                    if (startBulletsParent.TryGetComponent(out StartBullets startBullets))
                        startBullets.isMoveForward = true;
                }
            }

            // Set the unbeatable bullet and camera target
            unbeatableBullet.isUnbeatable = true;
            cameraManager.SetTarget(unbeatableBullet.transform);

            // Set Wall Length based on unbeatable bullet's HP
            return unbeatableBullet.hp / 2;
        }

        // Add a new bullet to an empty grid
        public static void AddBullet()
        {
            if (gridManager.emptyGrids.Count > 0 &&
                levelManager.dataBase.money >= levelManager.levelEditor.bulletPrice)
            {
                // Deduct money from database
                levelManager.dataBase.money -= levelManager.levelEditor.bulletPrice;
                int randomGridIndex = Random.Range(0, gridManager.emptyGrids.Count);

                // Spawn a new bullet and set its properties
                var newBulletObject = pool.SpawnObject(Vector3.zero, PoolItemType.Bullets, null,
                    levelManager.levelEditor.creatingBulletType - 1);
                var newBullet = newBulletObject.GetComponent<Bullet>();
                var targetGrid = gridManager.emptyGrids[randomGridIndex];
                var targetGridController = targetGrid.GetComponent<Grid>();

                // Set grid properties
                targetGridController.bulletType = levelManager.levelEditor.bulletDatas[levelManager.levelEditor.creatingBulletType - 1].type;
                gridManager.emptyGrids.Remove(targetGrid);
                targetGridController.gridSit = GridSit.Fill;

                // Set bullet properties and parent to the grid
                newBulletObject.transform.SetParent(targetGrid.transform);
                newBulletObject.transform.localPosition = Vector3.zero;
                newBulletObject.transform.DORotate(new Vector3(60, -90, -90), 0.1f);
                newBullet.gridNum = gridManager.grids.IndexOf(targetGrid);
                newBullet.bulletType = levelManager.levelEditor.creatingBulletType;
                newBullet.hitValue = levelManager.levelEditor.bulletDatas[levelManager.levelEditor.creatingBulletType - 1].hitValue;
                newBullet.hp = levelManager.levelEditor.bulletDatas[levelManager.levelEditor.creatingBulletType - 1].hp;
                newBullet.pos = targetGridController.pos;
                newBullet.GetGrid();

                // Add bullet to the game manager's list and save the game
                gameManager.bullets.Add(newBullet);
                levelManager.SaveSystem();
            }
        }

        // Merge two bullets into a new bullet
        public static void Merge(Bullet firstBullet, Bullet secondBullet)
        {
            if (firstBullet == null || secondBullet == null)
            {
                Debug.LogError("Cannot merge null bullets.");
                return;
            }

            // Reset the grid of the first and second bullets
            firstBullet.ResetGrid();
            secondBullet.ResetGrid();

            // Create the merged bullet
            GameObject mergedBulletObject = Pool.Instance.SpawnObject(secondBullet.transform.position,
                PoolItemType.Bullets, secondBullet.transform.parent, firstBullet.bulletType);

            if (mergedBulletObject.TryGetComponent(out Bullet mergedBullet))
            {
                // Set properties of the merged bullet
                mergedBullet.bulletType = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].type;
                mergedBullet.hitValue = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].hitValue;
                mergedBullet.hp = levelManager.levelEditor.bulletDatas[firstBullet.bulletType].hp;
                mergedBullet.GetGrid();
                mergedBullet.transform.DORotate(new Vector3(60, -90, -90), 0.1f);
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
}