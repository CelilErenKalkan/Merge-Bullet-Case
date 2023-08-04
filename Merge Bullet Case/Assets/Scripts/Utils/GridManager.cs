using System.Collections.Generic;
using Editors;
using Managers;
using UnityEngine;

namespace Utils
{
    public enum GridSit
    {
        Empty,
        Fill
    }

    public class GridManager : MonoSingleton<GridManager>
    {
        private LevelManager levelManager;
        private Pool pool;
        
        private GameObject gridsParent;
        [HideInInspector] public List<GameObject> grids = new List<GameObject>();
        [HideInInspector] public List<GameObject> emptyGrids = new List<GameObject>();


        private void Awake()
        {
            levelManager = LevelManager.Instance;
            pool = Pool.Instance;
            
            gridsParent = new GameObject
            {
                name = "Grids",
                transform = { position = Vector3.zero }
            };

            CreateGrids();
        }
        
        private void OnEnable()
        {
            Actions.LevelStart += DeactivateGrids;
        }
        
        private void OnDisable()
        {
            Actions.LevelStart -= DeactivateGrids;
        }

        private void CreateGrids()
        {
            for (var i = 0; i < levelManager.levelEditor.gridRow; i++)
            {
                for (var j = 0; j < levelManager.levelEditor.gridColumn; j++)
                {
                    //Call Grids
                    var tempGrid = pool.SpawnObject(Vector3.zero, PoolItemType.Grid, gridsParent.transform);
                    tempGrid.transform.position = levelManager.levelEditor.gridStartPoint +
                                                  new Vector3(j, -i, 0) * tempGrid.transform.localScale.x * 2;

                    //Set grid properties
                    grids.Add(tempGrid);
                    tempGrid.GetComponent<Grid>().gridNum = grids.IndexOf(tempGrid);
                    tempGrid.GetComponent<Grid>().pos = new Vector2(j, -i);
                    emptyGrids.Add(tempGrid);
                }
            }
        }

        private void DeactivateGrids()
        {
            foreach (var grid in grids)
            {
                pool.DeactivateObject(grid, PoolItemType.Grid);
            }
        }
    }
}