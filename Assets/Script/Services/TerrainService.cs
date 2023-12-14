using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainService : MonoBehaviour
{
    [SerializeField] private TerrainModifierConfig terrainModifierConfig;
    [SerializeField] private List<BiomeNodeOptions> biomeNodeOptions;

    public static TerrainService Instance;

    [Serializable]
    private struct BiomeNodeOptions
    {
        public Terrain Terrain;
        public List<BiomeNodeConfig> BiomeNodeConfigs;

        public (List<BiomeConfig> biome, float size) GetBiome(int zCollun)
        {
            int sorted = UnityEngine.Random.Range(0, BiomeNodeConfigs.Count);
            return (BiomeNodeConfigs[sorted].GetBiomeConfig(zCollun), BiomeNodeConfigs[sorted].GetMinDistance());
        } 
    }

    public (List<BiomeConfig> biome, float size) GetBiome(Terrain terrain, int zCollun)
    {
        return biomeNodeOptions.Find(b => b.Terrain == terrain).GetBiome(zCollun);
    }

    public GameObject GetTerrain(TerrainModifier terrain)
    {
        return terrainModifierConfig.GetTarreingModfierObj(terrain);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
