using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int floorAmount = 1;
    [SerializeField] private float cellSize;
    [SerializeField] private float floorHeight = 3f;
    [SerializeField] private LevelGenerator levelGenerator;
    [SerializeField] private bool slowGeneration;
    private List<GridSystemHex<GridObject>> gridSystemList;


    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gridSystemList = new List<GridSystemHex<GridObject>>();

        for (int floor = 0; floor < floorAmount; floor++)
        {
            var gridSystem = new GridSystemHex<GridObject>(width, height, cellSize, floor, floorHeight,
            (GridSystemHex<GridObject> g, GridPosition p) => new GridObject(g, p));

            gridSystemList.Add(gridSystem);
        }
    }


    public float GetFloorHeight()
    {
        return floorHeight;
    }

    private void Start()
    {
        if (slowGeneration)
        {
            StartCoroutine(levelGenerator.SlowGenerateLevel(gridSystemList[0]));
        }
        else
        {
            levelGenerator.GenerateLevel(gridSystemList[0]);
        }
        
        //Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
    }

    public GridSystemHex<GridObject> GetGridSystem(int floor) => gridSystemList[floor];

    public int GetFloor(Vector3 worldPosition) => Mathf.RoundToInt(worldPosition.y / floorHeight);

    public GridPosition GetGridPosition(Vector3 worldPosition) 
        => GetGridSystem(GetFloor(worldPosition)).GetGridPosition(worldPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) 
    {
        if(gridPosition.floor < 0 || gridPosition.floor >= floorAmount)
        {
            return false;
        }

        return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

    public (int width, int height) GetLevelGridSize() => GetGridSystem(0).GetGridSize();
    public int GetFloorAmount() => floorAmount;
}
