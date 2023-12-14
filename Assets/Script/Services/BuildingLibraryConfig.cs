using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingLibrary", menuName = "Configs/Building Library")]
public class BuildingLibraryConfig : ScriptableObject
{
    [SerializeField] private List<BuildingLibraryEntry> entries = new List<BuildingLibraryEntry>();

    public Building GetBuilding(BuildingType buildingType) => entries.Find(b => b.Type == buildingType).Building;
}
