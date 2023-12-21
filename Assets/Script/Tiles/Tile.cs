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
    [SerializeField] private Transform buildHolder;
    [SerializeField] private List<GameObject> hideOnBuild = new List<GameObject>();

    private Building currentBuilding;

    private GameObject modifierObj;
    
    private TerrainModifier terrainModfier;

    [Serializable]
    private struct ModifiersProbabilities
    {
        public float weight;
        public TerrainModifier modifier;
    }

    public List<TileSpreadSeed> GetTileSpreads() { return tileSpreadSeeds; }

    public Terrain GetTerrain() => type;
    public TerrainModifier GetTerrainModifier() => terrainModfier;

    public void ShowBuildButton()
    {
        if(type == Terrain.Spawn)
        {
            return;
        }

        buildCanvas.Show();
    }

    public void HideBuildButton()
    {
        buildCanvas.Hide();
    }

    public void BuildStructure(Building building)
    {
        if(currentBuilding != null)
        {
            Destroy(currentBuilding.gameObject);
        }

        foreach (var obj in hideOnBuild)
        {
            obj.SetActive(false);
        }

        currentBuilding = Instantiate(building, buildHolder);
    }

    private void Start()
    {
        SetupModifier();
        InstantiateGameObject();
        buildCanvas.SubscribeToButtonPress(OpenBuildMenu);
    }

    private void OpenBuildMenu()
    {
        BuildingService.Instance.OpenBuildingMenu(this);
    }

    private void InstantiateGameObject()
    {
        if(modifierObj != null)
        {
            Destroy(modifierObj);
        }

        if (terrainModfier == TerrainModifier.None)
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
