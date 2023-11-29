using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Terrain type;
    [SerializeField] private Transform hexHolder;
    [SerializeField] private List<TileSpreadSeed> tileSpreadSeeds = new List<TileSpreadSeed>();

    public List<TileSpreadSeed> GetTileSpreads() { return tileSpreadSeeds; }
}
