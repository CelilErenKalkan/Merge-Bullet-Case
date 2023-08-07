using System.Collections.Generic;
using Editors;
using UnityEngine;
using Grid = Utils.Grid;

namespace Managers
{
    public enum GridSit
    {
        Empty,
        Fill
    }

    public class GridManager : MonoSingleton<GridManager>
    {
        // References to managers
        private LevelManager levelManager;
        private Pool pool;

        // Parent for organizing grid objects
        private GameObject gridsParent;

        // Lists of grid objects
        [HideInInspector] public List<GameObject> grids = new List<GameObject>();
        [HideInInspector] public List<GameObject> emptyGrids = new List<GameObject>();

        private void Awake()
        {
            // Initialize references to managers
            levelManager = LevelManager.Instance;
            pool = Pool.Instance;

            // Create parent for grid objects
            gridsParent = CreateParentTransform("Grids");

            // Create and set up grid objects
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

        private GameObject CreateParentTransform(string name)
        {
            return new GameObject
            {
                name = name,
                transform = { position = Vector3.zero }
            };
        }

        private void CreateGrids()
        {
            for (var i = 0; i < levelManager.levelEditor.gridRow; i++)
            {
                for (var j = 0; j < levelManager.levelEditor.gridColumn; j++)
                {
                    // Spawn Grids
                    var tempGrid = pool.SpawnObject(Vector3.zero, PoolItemType.Grid, gridsParent.transform);
                    tempGrid.transform.position = levelManager.levelEditor.gridStartPoint +
                                                  new Vector3(j, -i, 0) * tempGrid.transform.localScale.x * 2;

                    // Set grid properties
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