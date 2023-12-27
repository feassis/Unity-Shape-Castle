using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Sprite buildButtonIcon;
    [SerializeField] private string buildName;
    [SerializeField] private string buildDescription;
    [SerializeField] private List<ResourceAmount> resourcesCost = new List<ResourceAmount>();
    [SerializeField] private List<ResourceAmount> resourcesGeneration = new List<ResourceAmount>();
    [SerializeField] private float generationTime = 0.0f;
    
    public float GetReourceCostAmount(ResourceType type)
    {
        var cost = resourcesCost.Find(r => r.resourceType == type);

        if(cost == null)
        {
            return 0;
        }

        return cost.amount;
    }

    public float GetReourceGenerationAmount(ResourceType type)
    {
        var generation = resourcesGeneration.Find(r => r.resourceType == type);

        if (generation == null)
        {
            return 0;
        }

        return generation.amount;
    }

    public string GetBuildingName() => buildName;

    public string GetBuildDescription() => buildDescription;

    public Sprite GetBuildingIcon() { return buildButtonIcon; }

    public float GetGenerationTime() { return generationTime; }
}
