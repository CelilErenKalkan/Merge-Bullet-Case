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

    public class GridController : MonoBehaviour
    {
        public int bulletType,gridNum;
        public GridSit gridSit;
        public Vector2 pos;
    }
    
    public class GridCreator : MonoBehaviour
    {
        public LevelEditor levelEditor;
    
        public GameObject gridPrefab;
        private GameObject tempGrid;
        private Transform gridsparent;
        public List<GameObject> grids = new List<GameObject>();
        public List<GameObject> emptyGrids = new List<GameObject>();


        private void Awake()
        {
            ObjectManager.GridCreator = this;
            gridsparent = GameObject.FindGameObjectWithTag("GridsParent").transform;

            CreateGrids();
        }

        private void CreateGrids()
        {
            for (int i = 0; i < levelEditor.gridRow; i++)
            {
                for (int j = 0; j < levelEditor.gridColumn; j++)
                {
                    //Create Grids
                    tempGrid = Instantiate(gridPrefab);
                    tempGrid.transform.SetParent(gridsparent);
                    tempGrid.transform.position = Vector3.zero;
                    tempGrid.transform.position = levelEditor.gridStartPoint + new Vector3(j, -i, 0)*tempGrid.transform.localScale.x*2;

                    //Set grid properties
                    grids.Add(tempGrid);
                    tempGrid.GetComponent<GridController>().gridNum = grids.IndexOf(tempGrid);
                    tempGrid.GetComponent<GridController>().pos = new Vector2(j, -i);
                    emptyGrids.Add(tempGrid);
                }
            }
        }
    }
}