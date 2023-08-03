using Gameplay;
using Managers;
using UnityEngine;
using Utils;
using Grid = Utils.Grid;

namespace Editors
{
    public static class BulletEditor
    {
        public static void CreateBullets()
        {
            foreach (var bulletSave in LevelManager.Instance.dataBase.bulletSaves)
            {
                //GameObject bulletPrefab = LevelManager.Instance.levelEditor.bulletDatas[bulletSave.type - 1].prefab;
                //GameObject currentBulletObject = Instantiate(bulletPrefab, GridCreator.Instance.grids[bulletSave.GridNum].transform, true);
                GameObject currentBulletObject = Pool.Instance.SpawnObject(GridCreator.Instance.grids[bulletSave.GridNum].transform.position, PoolItemType.Bullets,GridCreator.Instance.grids[bulletSave.GridNum].transform, bulletSave.type - 1);
                Bullet currentBulletScript = currentBulletObject.GetComponent<Bullet>();
        
                // Set bullet properties
                GameManager.Instance.bullets.Add(currentBulletScript);
                currentBulletScript.bulletType = LevelManager.Instance.levelEditor.bulletDatas[bulletSave.type - 1].type;
                currentBulletScript.hitValue = LevelManager.Instance.levelEditor.bulletDatas[bulletSave.type - 1].hitValue;
                currentBulletScript.hp = LevelManager.Instance.levelEditor.bulletDatas[bulletSave.type - 1].hp;
        
                // Get and set grid controller
                var currentGridController = GridCreator.Instance.grids[bulletSave.GridNum].GetComponent<Utils.Grid>();
                currentBulletScript.GetGridController();
                currentBulletScript.gridNum = bulletSave.GridNum;
                currentBulletScript.pos = currentBulletScript.currentGridController.pos;

                // Set Grid Settings
                currentGridController.bulletType = currentBulletScript.bulletType;
                currentGridController.gridSit = GridSit.Fill;
                GridCreator.Instance.emptyGrids.Remove(currentGridController.gameObject);
        
                // Parent bullet to the grid
                currentBulletObject.transform.SetParent(currentGridController.transform);
                currentBulletObject.transform.localPosition = Vector3.zero;
            }
        }
        
        public static void AddBullet()
        {
            if (GridCreator.Instance.emptyGrids.Count > 0 &&
                LevelManager.Instance.dataBase.money >= LevelManager.Instance.levelEditor.bulletPrice)
            {
                int rndGrid = Random.Range(0, GridCreator.Instance.emptyGrids.Count);

                //Create Bullet
                //var tempBullet = Instantiate(levelEditor
                    //.bulletDatas[LevelManager.Instance.levelEditor.CreateingBulletType - 1].prefab);
                var tempBullet = Pool.Instance.SpawnObject(Vector3.zero, PoolItemType.Bullets, null,
                    LevelManager.Instance.levelEditor.CreateingBulletType - 1);
                var bullet = tempBullet.GetComponent<Bullet>();
                var tempGrid = GridCreator.Instance.emptyGrids[rndGrid];
                var grid = tempGrid.GetComponent<Grid>();

                //Set grid properties
                grid.bulletType = LevelManager.Instance.levelEditor.bulletDatas[LevelManager.Instance.levelEditor.CreateingBulletType - 1]
                    .type;
                GridCreator.Instance.emptyGrids.Remove(tempGrid);
                grid.gridSit = GridSit.Fill;

                //Set bullet properties
                tempBullet.transform.SetParent(tempGrid.transform);
                tempBullet.transform.localPosition = Vector3.zero;
                bullet.gridNum = GridCreator.Instance.grids.IndexOf(tempGrid);
                bullet.bulletType = LevelManager.Instance.levelEditor.CreateingBulletType;
                bullet.hitValue = LevelManager.Instance.levelEditor.bulletDatas[LevelManager.Instance.levelEditor.CreateingBulletType - 1]
                    .hitValue;
                bullet.hp = LevelManager.Instance.levelEditor.bulletDatas[LevelManager.Instance.levelEditor.CreateingBulletType - 1].hp;
                bullet.pos = grid.pos;
                bullet.GetGridController();

                GameManager.Instance.bullets.Add(bullet);
                
                LevelManager.Instance.SaveSystem();
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

            // Create the merged bullet
            GameObject mergedBulletObject = Pool.Instance.SpawnObject(secondBullet.transform.position,
                PoolItemType.Bullets, secondBullet.transform.parent, firstBullet.bulletType);

            if (!mergedBulletObject.TryGetComponent(out Bullet bullet)) return;
            Bullet mergedBullet = bullet;
            
            // Set properties of the merged bullet
            mergedBullet.bulletType = LevelManager.Instance.levelEditor.bulletDatas[firstBullet.bulletType].type;
            mergedBullet.hitValue = LevelManager.Instance.levelEditor.bulletDatas[firstBullet.bulletType].hitValue;
            mergedBullet.hp = LevelManager.Instance.levelEditor.bulletDatas[firstBullet.bulletType].hp;
            mergedBullet.GetGridController();
            GameManager.Instance.bullets.Add(mergedBullet);
            
            // Remove the merged bullets from the bullets list
            GameManager.Instance.bullets.Remove(firstBullet);
            GameManager.Instance.bullets.Remove(secondBullet);

            // Deactivate the merged bullets
            Pool.Instance.DeactivateObject(firstBullet.gameObject, PoolItemType.Bullets);
            Pool.Instance.DeactivateObject(secondBullet.gameObject, PoolItemType.Bullets);
        }
    }
}
