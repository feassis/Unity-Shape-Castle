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

        public List<BiomeConfig> GetBiome(int zCollun)
        {
            return BiomeNodeConfigs[UnityEngine.Random.Range(0, BiomeNodeConfigs.Count)].GetBiomeConfig(zCollun);
        } 
    }

    public List<BiomeConfig> GetBiome(Terrain terrain, int zCollun)
    {
        return biomeNodeOptions.Find(b => b.Terrain == terrain).GetBiome(zCollun);
    }

    public GameObject GetTerrain(TerrainModfier terrain)
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
