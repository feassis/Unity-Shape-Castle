using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Terrain Modifier Config", menuName = "Configs/Terrain Modifier Config")]
public class TerrainModifierConfig : ScriptableObject
{
    [SerializeField] private List<TerrainModifierLibraryEntry> terraingModEntry;

    [Serializable]
    private struct TerrainModifierLibraryEntry
    {
        public TerrainModifier TerrainMod;
        public GameObject ModifierObj;
    }

    public GameObject GetTarreingModfierObj(TerrainModifier terrainMod)
    {
        return terraingModEntry.Find(t => t.TerrainMod == terrainMod).ModifierObj;
    }
}
