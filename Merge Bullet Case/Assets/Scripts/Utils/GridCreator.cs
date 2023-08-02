using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Utils
{
    public enum GridSit
    {
        Empty,
        Fill
    }

    public class GridCreator : MonoSingleton<GridCreator>
    {
        public GameObject gridPrefab;
        
        private LevelManager levelManager;
        private GameObject tempGrid;
        private GameObject gridsParent;
        [HideInInspector] public List<GameObject> grids = new List<GameObject>();
        [HideInInspector] public List<GameObject> emptyGrids = new List<GameObject>();


        private void Awake()
        {
            levelManager = LevelManager.Instance;
            
            gridsParent = new GameObject
            {
                name = "Grids",
                transform = { position = Vector3.zero }
            };

            CreateGrids();
        }

        private void CreateGrids()
        {
            for (int i = 0; i < levelManager.levelEditor.gridRow; i++)
            {
                for (int j = 0; j < levelManager.levelEditor.gridColumn; j++)
                {
                    //Create Grids
                    tempGrid = Instantiate(gridPrefab);
                    tempGrid.transform.SetParent(gridsParent.transform);
                    tempGrid.transform.position = Vector3.zero;
                    tempGrid.transform.position = levelManager.levelEditor.gridStartPoint + new Vector3(j, -i, 0)*tempGrid.transform.localScale.x*2;

                    //Set grid properties
                    grids.Add(tempGrid);
                    tempGrid.GetComponent<Grid>().gridNum = grids.IndexOf(tempGrid);
                    tempGrid.GetComponent<Grid>().pos = new Vector2(j, -i);
                    emptyGrids.Add(tempGrid);
                }
            }
        }
    }
}