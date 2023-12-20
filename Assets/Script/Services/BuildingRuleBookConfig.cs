using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingRuleBook", menuName ="Configs/Building Rule Book")]
public class BuildingRuleBookConfig : ScriptableObject
{
    [SerializeField] private List<BuildingType> CommomBuildings = new List<BuildingType>();
    [SerializeField] private List<BuildingRuleTerrain> buildingRuleTerrains = new List<BuildingRuleTerrain>();
    [SerializeField] private List<BuilingRuleModifier> builingRuleModifiers = new List<BuilingRuleModifier>();


    public List<BuildingType> GetBuildingOptions(Terrain terrain, TerrainModifier modfier)
    {
        List<BuildingType> buildingList = buildingRuleTerrains.Find(b => b.Type == terrain).Buildings;
        if(modfier != TerrainModifier.None)
        {
            buildingList.AddRange(builingRuleModifiers.Find(b => b.Type == modfier).Buildings);
        }
        
        buildingList.AddRange(CommomBuildings);

        return buildingList;
    }
}

[Serializable]
public class BuildingRuleTerrain
{
    public Terrain Type;
    public List<BuildingType> Buildings;
}

[Serializable]
public class BuilingRuleModifier
{
    public TerrainModifier Type;
    public List<BuildingType> Buildings;
}
