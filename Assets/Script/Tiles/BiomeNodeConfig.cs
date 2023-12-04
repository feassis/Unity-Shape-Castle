using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Biome Node", menuName = "Configs/Biome Node")]

public class BiomeNodeConfig : ScriptableObject
{
    [SerializeField] private List<BiomeConfig> biomeConfigEven = new List<BiomeConfig>();
    [SerializeField] private List<BiomeConfig> biomeConfigOdd = new List<BiomeConfig>();

    public List<BiomeConfig> GetBiomeConfig(int zCollun)
    {
        return zCollun % 2 == 0 ? biomeConfigEven : biomeConfigOdd;
    }
}

[Serializable]
public struct BiomeConfig
{
    public Terrain Terrain;
    public Vector2Int IncrementalPos;
}