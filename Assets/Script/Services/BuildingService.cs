using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingService : MonoBehaviour
{
    public static BuildingService Instance;

    [SerializeField] private BuildingRuleBookConfig buildingRules;
    [SerializeField] private BuildingLibraryConfig buildingLibraryConfig;

    public Building GetBuilding(BuildingType buildingType)
    {
        return buildingLibraryConfig.GetBuilding(buildingType);
    }

    public List<BuildingType> GetBuildingOptions(Terrain terrain, TerrainModifier modifier)
    {
        return buildingRules.GetBuildingOptions(terrain, modifier);
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
