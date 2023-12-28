using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] private Sprite buildButtonIcon;
    [SerializeField] private string buildName;
    [SerializeField] private string buildDescription;
    [SerializeField] private List<ResourceAmount> resourcesCost = new List<ResourceAmount>();
    [SerializeField] private List<ResourceAmount> resourcesGeneration = new List<ResourceAmount>();
    [SerializeField] private float generationTime = 0.0f;

    [SerializeField] private GameObject generationResourceCanvas;
    [SerializeField] private Image fillBar;
    

    private float generationTimer;

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


    private void Awake()
    {
        if(generationTime <= 0)
        {
            generationResourceCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if(generationTime <= 0.0f)
        {
            return;
        }

        fillBar.fillAmount = generationTimer/generationTime;

        generationTimer += Time.deltaTime;

        if(generationTimer > generationTime)
        {
            generationTimer = 0;

            foreach (var resource in resourcesGeneration)
            {
                ResourceService.Instance.AddResource(resource.resourceType, resource.amount);
            }
        }
    }
}