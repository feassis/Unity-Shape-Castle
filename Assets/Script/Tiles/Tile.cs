using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Terrain type;
    [SerializeField] private Transform modfierHolder;
    [SerializeField] private List<TileSpreadSeed> tileSpreadSeeds = new List<TileSpreadSeed>();
    [SerializeField] private List<ModifiersProbabilities> modifiersProbabilities;
    [SerializeField] private BuildCanvas buildCanvas;

    private GameObject modifierObj;
    
    private TerrainModfier terrainModfier;

    [Serializable]
    private struct ModifiersProbabilities
    {
        public float weight;
        public TerrainModfier modifier;
    }

    public List<TileSpreadSeed> GetTileSpreads() { return tileSpreadSeeds; }

    public void ShowBuildButton()
    {
        buildCanvas.Show();
    }

    public void HideBuildButton()
    {
        buildCanvas.Hide();
    }

    private void Start()
    {
        SetupModifier();
        InstantiateGameObject();
    }

    private void InstantiateGameObject()
    {
        if(modifierObj != null)
        {
            Destroy(modifierObj);
        }

        if (terrainModfier == TerrainModfier.None)
        {
            return;
        }

        var mod = TerrainService.Instance.GetTerrain(terrainModfier);
        Instantiate(mod, modfierHolder);
    }

    private void SetupModifier()
    {
        float totalWeight = 0;

        foreach (var prob in modifiersProbabilities)
        {
            totalWeight += prob.weight;
        }

        float sortedNumber = UnityEngine.Random.Range(0, totalWeight);

        foreach (var prob in modifiersProbabilities)
        {
            if (sortedNumber < prob.weight)
            {
                terrainModfier = prob.modifier;
                break;
            }

            sortedNumber -= prob.weight;
        }
    }
}
