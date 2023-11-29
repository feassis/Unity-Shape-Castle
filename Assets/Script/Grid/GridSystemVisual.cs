using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [SerializeField] private GridSystemVisualSingle gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    private GridSystemVisualSingle[,,] gridSystemVisualsArray;
    private GridSystemVisualSingle lastSelected;

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType Type;
        public Material Material;
    }

    public enum GridVisualType
    {
        White = 0,
        Blue = 1,
        Red = 2,
        Yellow = 3,
        RedSoft = 4,
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        Vector3 mousePos = MouseWorld.GetMousePosition();
        GridPosition gridPos = LevelGrid.Instance.GetGridPosition(mousePos);
        if (lastSelected != null)
        {
            lastSelected.HideSelected();
        }

        if (LevelGrid.Instance.IsValidGridPosition(gridPos))
        {
            lastSelected = gridSystemVisualsArray[gridPos.x, gridPos.z, gridPos.floor];
        }

        if(lastSelected != null)
        {
            lastSelected.ShowSelected();
        }


    }

    private void Start()
    {
        var gridSize = LevelGrid.Instance.GetLevelGridSize();
        int floorAmount = LevelGrid.Instance.GetFloorAmount();
        gridSystemVisualsArray = new GridSystemVisualSingle[gridSize.width, gridSize.height, floorAmount];
        
        for (int x = 0; x < gridSize.width; x++)
        {
            for (int z = 0; z < gridSize.height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    var gridVisual = Instantiate(gridSystemVisualSinglePrefab,
                    LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z, floor)), Quaternion.identity);
                    gridVisual.transform.parent = transform;
                    gridVisual.Hide();
                    gridSystemVisualsArray[x, z, floor] = gridVisual;
                }    
            }
        }

        for(int x = 0; x < gridSize.width; x++)
        {
            for(int z = 0;z < gridSize.height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    if (floor != 0)
                    {
                        continue;
                    }
                    gridSystemVisualsArray[x, z, floor].Show(GetGridVisualTypeMaterial(GridVisualType.White));
                }
            }
        }
    }

    public void HideAllGridVisuals()
    {
        var gridSize = LevelGrid.Instance.GetLevelGridSize();
        int floorAmount = LevelGrid.Instance.GetFloorAmount();

        for (int x = 0; x < gridSize.width; x++)
        {
            for (int z = 0; z < gridSize.height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    gridSystemVisualsArray[x, z, floor].Hide();
                }    
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType type)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, type);
    }

    private void ShowGridPositionRangeSquared(GridPosition gridPosition, int range, GridVisualType type)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z, 0);
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, type);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType type)
    {
        Material gridMaterial = GetGridVisualTypeMaterial(type);
        foreach (var pos in gridPositions)
        {
            gridSystemVisualsArray[pos.x, pos.z, pos.floor].Show(gridMaterial);
        }
    }

    private Material GetGridVisualTypeMaterial(GridVisualType type)
    {
        return gridVisualTypeMaterialList.Find(g => g.Type == type).Material;
    }
}
