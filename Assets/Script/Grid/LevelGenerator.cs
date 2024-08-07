using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform levelHolder;
    [SerializeField] private List<TileNode> tiles;
    [SerializeField] private List<BiomesSetup> biomes;

    [Serializable]
    private struct TileNode 
    {
        public Terrain Type;
        public Tile Tile;
    }

    [Serializable]
    private class BiomesSetup
    {
        public Terrain terrain;
        public int min;
        public int max;
        public int priority;

        public int BiomeAmount() => Random.Range(min, max + 1);
    }

    public IEnumerator SlowGenerateLevel(GridSystemHex<GridObject> gridSystem)
    {
        List<BiomesSetup> biomesToSetup = new List<BiomesSetup>();
        biomesToSetup.AddRange(biomes);
        biomesToSetup.Sort((a, b) => a.priority > b.priority ? 1 : 0);

        List<GridPosition> visitedNodes = new List<GridPosition>();
        List<GridPosition> nodesToVisit = new List<GridPosition>();

        List<Vector2Int> sortedBiomePos = new List<Vector2Int>();

        foreach (BiomesSetup biome in biomesToSetup)
        {
            int amount = biome.BiomeAmount();

            for (int i = 0; i < amount; i++)
            {
                (List<BiomeConfig> biomeConfig, float size) sortedBiome = (new List<BiomeConfig>(), 0);

                Vector2Int pos = Vector2Int.zero;
                if (biome.terrain == Terrain.Spawn)
                {
                    pos = new Vector2Int(gridSystem.GetGridSize().width / 2, gridSystem.GetGridSize().height / 2);
                    sortedBiome = TerrainService.Instance.GetBiome(biome.terrain, pos.y);
                }
                else
                {
                    pos = new Vector2Int(Random.Range(0, gridSystem.GetGridSize().width), Random.Range(0, gridSystem.GetGridSize().height));
                    int count = 0;
                    bool validPosition = false;

                    while (!validPosition && count < 3)
                    {
                        sortedBiome = TerrainService.Instance.GetBiome(biome.terrain, pos.y);
                        if (sortedBiomePos.Count <= 0)
                        {
                            validPosition = true;
                            continue;
                        }

                        foreach (var position in sortedBiomePos)
                        {
                            if (Vector2Int.Distance(pos, position) < sortedBiome.size)
                            {
                                pos = new Vector2Int(Random.Range(0, gridSystem.GetGridSize().width), Random.Range(0, gridSystem.GetGridSize().height));
                                count++;
                                continue;
                            }
                        }
                        validPosition = true;
                        break;
                    }

                }

                sortedBiomePos.Add(pos);

                var biomeConfig = sortedBiome.biomeConfig;


                foreach (var item in biomeConfig)
                {
                    GridPosition gridPos = new GridPosition(pos.x + item.IncrementalPos.x,
                        pos.y + item.IncrementalPos.y, gridSystem.GetFloor());

                    if (!gridSystem.IsValidGridPosition(gridPos))
                    {
                        continue;
                    }

                    GridObject obj = gridSystem.GetGridObject(gridPos);
                    obj.SetTile(tiles.Find(t => t.Type == item.Terrain).Tile, levelHolder);
                    visitedNodes.Add(obj.GetGridPosition());
                }
            }
        }

        foreach (GridPosition node in visitedNodes)
        {
            var neighbours = gridSystem.GetNeighbours(node);

            foreach (GridPosition neighbour in neighbours)
            {
                if (gridSystem.IsValidGridPosition(neighbour) && !visitedNodes.Contains(neighbour))
                {
                    nodesToVisit.Add(neighbour);
                }
            }
        }


        yield return GenerateCellBaseSlow(nodesToVisit, gridSystem, visitedNodes);
    }

    private IEnumerator GenerateCellBaseSlow(List<GridPosition> nodesToVisit, GridSystemHex<GridObject> gridSystem, List<GridPosition> visitedNodes)
    {
        while (nodesToVisit.Count > 0)
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

                if (!visitedNodes.Contains(neighbour) && !nodesToVisit.Contains(neighbour))
                {
                    nodesToVisit.Add(neighbour);
                }

                GridObject neighborNode = gridSystem.GetGridObject(neighbour);

                if (neighborNode.GetTile() == null)
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

            gridSystem.GetGridObject(visitingNode).SetTile(tiles.Find(t => t.Type == desiredTerrain).Tile, levelHolder);
            visitedNodes.Add(visitingNode);
            nodesToVisit.Remove(visitingNode);
            yield return null;
        }
    }

    public void GenerateLevel(GridSystemHex<GridObject> gridSystem)
    {
        List<BiomesSetup> biomesToSetup = new List<BiomesSetup>();
        biomesToSetup.AddRange(biomes);
        biomesToSetup.Sort((a, b) => a.priority > b.priority ? 1 : 0);

        List<GridPosition> visitedNodes = new List<GridPosition>();
        List<GridPosition> nodesToVisit = new List<GridPosition>();

        List<Vector2Int> sortedBiomePos = new List<Vector2Int>();

        foreach (BiomesSetup biome in biomesToSetup)
        {
            int amount = biome.BiomeAmount();

            

            for (int i = 0; i < amount; i++)
            {
                (List<BiomeConfig> biomeConfig, float size) sortedBiome = (new List<BiomeConfig>(), 0); 

                Vector2Int pos = Vector2Int.zero;
                if (biome.terrain == Terrain.Spawn)
                {
                    pos = new Vector2Int(gridSystem.GetGridSize().width / 2, gridSystem.GetGridSize().height / 2);
                    sortedBiome = TerrainService.Instance.GetBiome(biome.terrain, pos.y);
                }
                else
                {
                    pos = new Vector2Int(Random.Range(0, gridSystem.GetGridSize().width), Random.Range(0, gridSystem.GetGridSize().height));
                    int count = 0;
                    bool validPosition = false;

                    

                    while (!validPosition && count < 3)
                    {
                        sortedBiome = TerrainService.Instance.GetBiome(biome.terrain, pos.y);
                        if (sortedBiomePos.Count <= 0)
                        {
                            validPosition = true;
                            continue;
                        }

                        foreach (var position in sortedBiomePos)
                        {
                            if (Vector2Int.Distance(pos, position) < sortedBiome.size)
                            {
                                pos = new Vector2Int(Random.Range(0, gridSystem.GetGridSize().width), Random.Range(0, gridSystem.GetGridSize().height));
                                count++;
                                continue;
                            }
                        }
                        validPosition = true; 
                        break;
                    }

                }

                sortedBiomePos.Add(pos);

                var biomeConfig = sortedBiome.biomeConfig;
                

                foreach (var item in biomeConfig)
                {
                    GridPosition gridPos = new GridPosition(pos.x + item.IncrementalPos.x,
                        pos.y + item.IncrementalPos.y, gridSystem.GetFloor());

                    if (!gridSystem.IsValidGridPosition(gridPos))
                    {
                        continue;
                    }

                    GridObject obj = gridSystem.GetGridObject(gridPos);
                    obj.SetTile(tiles.Find(t => t.Type == item.Terrain).Tile, levelHolder);
                    visitedNodes.Add(obj.GetGridPosition());
                }
            }
        }

        foreach (GridPosition node in visitedNodes)
        {
            var neighbours = gridSystem.GetNeighbours(node);

            foreach (GridPosition neighbour in neighbours)
            {
                if (gridSystem.IsValidGridPosition(neighbour) && !visitedNodes.Contains(neighbour))
                {
                    nodesToVisit.Add(neighbour);
                }
            }
        }


        GenerateCellBase(nodesToVisit, gridSystem, visitedNodes);
    }

    public void GenerateCellBase(List<GridPosition> nodesToVisit, GridSystemHex<GridObject> gridSystem, List<GridPosition> visitedNodes)
    {
        while (nodesToVisit.Count > 0)
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

                if (!visitedNodes.Contains(neighbour) && !nodesToVisit.Contains(neighbour))
                {
                    nodesToVisit.Add(neighbour);
                }

                GridObject neighborNode = gridSystem.GetGridObject(neighbour);

                if (neighborNode.GetTile() == null)
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

            gridSystem.GetGridObject(visitingNode).SetTile(tiles.Find(t => t.Type == desiredTerrain).Tile, levelHolder);
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
