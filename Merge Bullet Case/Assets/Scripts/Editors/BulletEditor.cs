using Gameplay;
using Managers;
using UnityEngine;
using Utils;
using Grid = Utils.Grid;

namespace Editors
{
    public static class BulletEditor
    {
        private static LevelManager _levelManager;
        private static GameManager _gameManager;
        private static GridManager _gridManager;
        private static CameraManager _cameraManager;
        private static Pool _pool;

        private static Bullet unbeatableBullet;

        public static void SetBulletEditor()
        {
            _levelManager = LevelManager.Instance;
            _gameManager = GameManager.Instance;
            _gridManager = GridManager.Instance;
            _cameraManager = CameraManager.Instance;
            _pool = Pool.Instance;
        }
        
        public static void CreateBullets()
        {
            foreach (var bulletSave in _levelManager.dataBase.bulletSaves)
            {
                GameObject currentBulletObject = _pool.SpawnObject(_gridManager.grids[bulletSave.GridNum].transform.position, PoolItemType.Bullets,_gridManager.grids[bulletSave.GridNum].transform, bulletSave.type - 1);
                Bullet currentBulletScript = currentBulletObject.GetComponent<Bullet>();
        
                // Set bullet properties
                _gameManager.bullets.Add(currentBulletScript);
                currentBulletScript.bulletType = _levelManager.levelEditor.bulletDatas[bulletSave.type - 1].type;
                currentBulletScript.hitValue = _levelManager.levelEditor.bulletDatas[bulletSave.type - 1].hitValue;
                currentBulletScript.hp = _levelManager.levelEditor.bulletDatas[bulletSave.type - 1].hp;
        
                // Get and set grid controller
                var currentGridController = _gridManager.grids[bulletSave.GridNum].GetComponent<Grid>();
                currentBulletScript.GetGridController();
                currentBulletScript.gridNum = bulletSave.GridNum;
                currentBulletScript.pos = currentBulletScript.currentGrid.pos;

                // Set Grid Settings
                currentGridController.bulletType = currentBulletScript.bulletType;
                currentGridController.gridSit = GridSit.Fill;
                _gridManager.emptyGrids.Remove(currentGridController.gameObject);
        
                // Parent bullet to the grid
                currentBulletObject.transform.SetParent(currentGridController.transform);
                currentBulletObject.transform.localPosition = Vector3.zero;
            }
        }
        
        public static int CreateInitialBullets(Transform startBulletsParent)
        {
            for (var i = 0; i < _levelManager.dataBase.bulletSaves.Count; i++)
            {
                //Create Bullet
                var tempBullet = _pool.SpawnObject(_levelManager.dataBase.bulletSaves[i].pos, PoolItemType.Bullets,
                    startBulletsParent, _levelManager.dataBase.bulletSaves[i].type - 1);
                if (tempBullet.TryGetComponent(out Bullet bullet))

                //Set Bullet Properties
                bullet.bulletType = _levelManager.dataBase.bulletSaves[i].type;
                bullet.hp = _levelManager.dataBase.bulletSaves[i].hp;
                bullet.pos = _levelManager.dataBase.bulletSaves[i].pos;
                bullet.hitValue = _levelManager.dataBase.bulletSaves[i].hitValue;
                bullet.isGameBullet = true;

                //Select Unbeatable bullet
                if (unbeatableBullet == null)
                    unbeatableBullet = bullet;
                else 
                    unbeatableBullet = bullet.bulletType >= unbeatableBullet.bulletType ? bullet : unbeatableBullet;

                //Set Bullet Transform
                tempBullet.transform.localScale = tempBullet.transform.localScale * 2;
                tempBullet.transform.position = new Vector3(-4, 1, 0) + new Vector3(bullet.pos.x * 2, 0, bullet.pos.y * 2);
                tempBullet.transform.rotation = Quaternion.Euler(90, 0, 0);

                //Start Move
                startBulletsParent.GetComponent<StartBullets>().isMoveForward = true;
            }
            
            unbeatableBullet.isUnbeatable = true;
            _cameraManager.SetTarget(unbeatableBullet.transform);
            
            //Set Wall Length
            return unbeatableBullet.hp + 2;
        }
        
        public static void AddBullet()
        {
            if (_gridManager.emptyGrids.Count > 0 &&
                _levelManager.dataBase.money >= _levelManager.levelEditor.bulletPrice)
            {
                int rndGrid = Random.Range(0, _gridManager.emptyGrids.Count);
                
                var tempBullet = _pool.SpawnObject(Vector3.zero, PoolItemType.Bullets, null,
                    _levelManager.levelEditor.creatingBulletType - 1);
                var bullet = tempBullet.GetComponent<Bullet>();
                var tempGrid = _gridManager.emptyGrids[rndGrid];
                var grid = tempGrid.GetComponent<Grid>();

                //Set grid properties
                grid.bulletType = _levelManager.levelEditor.bulletDatas[_levelManager.levelEditor.creatingBulletType - 1]
                    .type;
                _gridManager.emptyGrids.Remove(tempGrid);
                grid.gridSit = GridSit.Fill;

                //Set bullet properties
                tempBullet.transform.SetParent(tempGrid.transform);
                tempBullet.transform.localPosition = Vector3.zero;
                bullet.gridNum = _gridManager.grids.IndexOf(tempGrid);
                bullet.bulletType = _levelManager.levelEditor.creatingBulletType;
                bullet.hitValue = _levelManager.levelEditor.bulletDatas[_levelManager.levelEditor.creatingBulletType - 1]
                    .hitValue;
                bullet.hp = _levelManager.levelEditor.bulletDatas[_levelManager.levelEditor.creatingBulletType - 1].hp;
                bullet.pos = grid.pos;
                bullet.GetGridController();

                _gameManager.bullets.Add(bullet);
                
                _levelManager.SaveSystem();
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
            mergedBullet.bulletType = _levelManager.levelEditor.bulletDatas[firstBullet.bulletType].type;
            mergedBullet.hitValue = _levelManager.levelEditor.bulletDatas[firstBullet.bulletType].hitValue;
            mergedBullet.hp = _levelManager.levelEditor.bulletDatas[firstBullet.bulletType].hp;
            mergedBullet.GetGridController();
            _gameManager.bullets.Add(mergedBullet);
            
            // Remove the merged bullets from the bullets list
            _gameManager.bullets.Remove(firstBullet);
            _gameManager.bullets.Remove(secondBullet);

            // Deactivate the merged bullets
            _pool.DeactivateObject(firstBullet.gameObject, PoolItemType.Bullets);
            _pool.DeactivateObject(secondBullet.gameObject, PoolItemType.Bullets);
        }
    }
}
