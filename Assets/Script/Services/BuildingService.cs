using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingService : MonoBehaviour
{
    public static BuildingService Instance;

    [SerializeField] private BuildingRuleBookConfig buildingRules;
    [SerializeField] private BuildingLibraryConfig buildingLibraryConfig;
    [SerializeField] private BuildMenu buildMenuPrefab;
    [SerializeField] private Transform canvas;

    private BuildMenu buildMenu;

    public Building GetBuilding(BuildingType buildingType)
    {
        return buildingLibraryConfig.GetBuilding(buildingType);
    }

    public List<BuildingType> GetBuildingOptions(Terrain terrain, TerrainModifier modifier)
    {
        return buildingRules.GetBuildingOptions(terrain, modifier);
    }

    public void OpenBuildingMenu(Tile tile)
    {
        if(buildMenu != null)
        {
            CloseBuildingMenu();
        }

        buildMenu = Instantiate(buildMenuPrefab, canvas);
    }

    public void CloseBuildingMenu()
    {
        Destroy(buildMenu.gameObject);
        buildMenu = null;
    }

    private void Awake()
    {
        if (buildMenu != null)
        {
            CloseBuildingMenu();
        }

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