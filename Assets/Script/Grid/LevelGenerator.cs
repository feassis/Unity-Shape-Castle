using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Tile spawnTime;
    [SerializeField] private List<TileNode> tiles;

    [Serializable]
    private struct TileNode 
    {
        public Terrain Type;
        public Tile Tile;
    }

    public void GenerateLevel(GridSystemHex<GridObject> gridSystem)
    {
        Vector2 spawnGridPosition = new Vector2(gridSystem.GetGridSize().width / 2, gridSystem.GetGridSize().height / 2);

        GridObject gridObjectSpawn = gridSystem.GetGridObject(new GridPosition(Mathf.RoundToInt(spawnGridPosition.x), Mathf.RoundToInt(spawnGridPosition.y), gridSystem.GetFloor()));
        gridObjectSpawn.SetTile(spawnTime);

        List<GridPosition> visitedNodes = new List<GridPosition>();
        visitedNodes.Add(gridObjectSpawn.GetGridPosition());

        List<GridPosition> nodesToVisit = new List<GridPosition>();

        var neighbours = gridSystem.GetNeighbours(gridObjectSpawn.GetGridPosition());

        foreach (GridPosition node in neighbours)
        {
            if(gridSystem.IsValidGridPosition(node)) 
            {
                nodesToVisit.Add(node);
            }
        }

        while(nodesToVisit.Count > 0)
        {
            GridPosition visitingNode = nodesToVisit[0];

            List<GridPosition> neighbourList = gridSystem.GetNeighbours(visitingNode);
            List<TileSpreadSeed> spread = new List<TileSpreadSeed>();

            foreach (GridPosition neighbour in neighbourList)
            {
                if (!gridSystem.IsValidGridPosition(neighbour))
                {
                    continue;
                }

                if(!visitedNodes.Contains(neighbour) && !nodesToVisit.Contains(neighbour))
                {
                    nodesToVisit.Add(neighbour);
                }

                GridObject neighborNode = gridSystem.GetGridObject(neighbour);

                if(neighborNode.GetTile() == null)
                {
                    continue;
                }

                Tile tileInStudy = neighborNode.GetTile();

                List<TileSpreadSeed> seeds = tileInStudy.GetTileSpreads();

                foreach (TileSpreadSeed seed in seeds)
                {
                    if (spread.Find(s => s.terrainType == seed.terrainType) == null)
                    {
                        spread.Add(seed);
                        continue;
                    }

                    spread.Find(s => s.terrainType == seed.terrainType).weight = seed.weight;
                }
            }

            Terrain desiredTerrain = GetChoosenSeed(spread);

            gridSystem.GetGridObject(visitingNode).SetTile(tiles.Find(t => t.Type == desiredTerrain).Tile);
            visitedNodes.Add(visitingNode);
            nodesToVisit.Remove(visitingNode);

        }

    }

    public Terrain GetChoosenSeed(List<TileSpreadSeed> spread)
    {
        float weightSun = 0f;

        foreach(TileSpreadSeed seed in spread)
        {
            weightSun += seed.weight;
        }

        float randomSeed = Random.Range(0, weightSun);

        foreach(TileSpreadSeed seed in spread)
        {
            if(randomSeed < seed.weight)
            {
                return seed.terrainType;
            }

            randomSeed -= seed.weight;
        }

        return Terrain.None;
    }
}
