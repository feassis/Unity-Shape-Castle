using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    [SerializeField] private GridDebugObject gridDebugObjectPrefab;
    [SerializeField] private Transform pathFindingLinkHolder;
    [SerializeField] private LayerMask obstacleLayer, floorMask;
    private int width;
    private int height;
    private float cellSize;

    private int floorAmount;
    private List<GridSystemHex<PathNode>> gridSystemList;
    private const int MOVE_STRAIGHT_COST = 10; 

    private List<PathfindingLink> pathfindinglinkList;

    private void Awake()
    {
        if(Instance != null)
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

    public void Setup(int width, int height, float cellSize, int floorAmount)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.floorAmount = floorAmount;

        gridSystemList = new List<GridSystemHex<PathNode>>();

        for (int floor = 0; floor < floorAmount; floor++)
        {
            var gridSystem = new GridSystemHex<PathNode>(width, height, cellSize, floor, LevelGrid.Instance.GetFloorHeight(),
            (GridSystemHex<PathNode> g, GridPosition p) => new PathNode(p));

            gridSystemList.Add(gridSystem);
        }

        

        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    float raycastOffsetDistance = 1f;

                    bool nodeIsBlocked = Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance,
                        Vector3.up, raycastOffsetDistance * 2, obstacleLayer);

                    bool nodeHasFloor = Physics.Raycast(worldPosition + Vector3.up * raycastOffsetDistance,
                        Vector3.down, raycastOffsetDistance * 2, floorMask);

                    if (nodeIsBlocked || !nodeHasFloor)
                    {
                        GetNode(x, z, floor).SetIsWalkable(false);
                    }
                } 
            }
        }

        pathfindinglinkList = new List<PathfindingLink>();

        foreach (Transform pathfindingLinkTransform in pathFindingLinkHolder)
        {
            if (pathfindingLinkTransform.
                TryGetComponent<PathfindingLinkMonoBehavior>(out PathfindingLinkMonoBehavior link))
            {
                pathfindinglinkList.Add(link.GetPathfindingLink());
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startingPosition, 
        GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();

        PathNode startNode = GetGridSystem(startingPosition.floor).GetGridObject(startingPosition);
        PathNode endNode = GetGridSystem(endGridPosition.floor).GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                for (int floor = 0; floor < floorAmount; floor++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    PathNode pathNode = GetGridSystem(floor).GetGridObject(gridPosition);

                    pathNode.SetGcost(int.MaxValue);
                    pathNode.SetHcost(0);
                    pathNode.CalculateFCost();
                    pathNode.ResetCameFromPathNode();
                }
            }
        }

        startNode.SetGcost(0);
        startNode.SetHcost(CalculateHeuristicDistance(startingPosition, endGridPosition));
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowerstFCostPathNode(openList);

            if(currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closeList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.IsWalkable())
                {
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + MOVE_STRAIGHT_COST;

                if(tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGcost(tentativeGCost);
                    neighbourNode.SetHcost(CalculateHeuristicDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();

        pathNodeList.Add(endNode);

        PathNode currentNode = endNode;

        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (var node in pathNodeList)
        {
            gridPositionList.Add(node.GetGridPosition());
        }

        return gridPositionList;
    }

    public int CalculateHeuristicDistance(GridPosition gridPositionA, 
        GridPosition gridPositionB)
    {
        return Mathf.RoundToInt(MOVE_STRAIGHT_COST *
           Vector3.Distance(gridSystemList.Find(g => g.GetFloor() == gridPositionA.floor).GetWorldPosition(gridPositionA),
           gridSystemList.Find(g => g.GetFloor() == gridPositionB.floor).GetWorldPosition(gridPositionB)));

    }

    private PathNode GetLowerstFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCost = pathNodeList[0];

        foreach (var testNode in pathNodeList)
        {
            if(testNode.GetFCost() < lowestFCost.GetFCost())
            {
                lowestFCost = testNode;
            }
        }

        return lowestFCost;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0, gridPosition.floor));
        }

        if (gridPosition.x + 1 < width)
        {
            // Right
            neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0, gridPosition.floor));
        }

        if (gridPosition.z - 1 >= 0)
        {
            // Down
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1, gridPosition.floor));
        }
        if (gridPosition.z + 1 < height)
        {
            // Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1, gridPosition.floor));
        }


        bool oddRow = gridPosition.z % 2 == 1;

        if (oddRow)
        {
            if (gridPosition.x + 1 < width)
            {
                if (gridPosition.z - 1 >= 0)
                {
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1, gridPosition.floor));
                }
                if (gridPosition.z + 1 < height)
                {
                    neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1, gridPosition.floor));
                }
            }
        } else
        {
            if (gridPosition.x - 1 >= 0)
            {
                if (gridPosition.z - 1 >= 0)
                {
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1, gridPosition.floor));
                }
                if (gridPosition.z + 1 < height)
                {
                    neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1, gridPosition.floor));
                }
            }
        }


        List<PathNode> totalNeighbourList = new List<PathNode>();
        totalNeighbourList.AddRange(neighbourList);

        List<GridPosition> pathfindingLinkGridPositionList = GetPathfindingLinkConnectedGridPosition(gridPosition);

        foreach (var gridPos in pathfindingLinkGridPositionList)
        {
            totalNeighbourList.Add(GetNode(gridPos.x, gridPos.z, gridPos.floor));
        }

        return totalNeighbourList;
    }

    private List<GridPosition> GetPathfindingLinkConnectedGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        foreach (PathfindingLink pathfingLink in pathfindinglinkList)
        {
            if(pathfingLink.GridPositionA == gridPosition)
            {
                gridPositionList.Add(pathfingLink.GridPositionB);
                continue;
            }

            if (pathfingLink.GridPositionB == gridPosition)
            {
                gridPositionList.Add(pathfingLink.GridPositionA);
            }
        }

        return gridPositionList;
    }

    private GridSystemHex<PathNode> GetGridSystem(int floor)
    {
        return gridSystemList[floor];
    }

    private PathNode GetNode(int x, int z, int floor)
    {
        return GetGridSystem(floor).GetGridObject(new GridPosition(x, z, floor));
    }

    public bool isWalkableGridPosition(GridPosition gridPosition)
    {
        return GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).IsWalkable();
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        GetGridSystem(gridPosition.floor).GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLegth) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLegth);
        return pathLegth;
    }
}
