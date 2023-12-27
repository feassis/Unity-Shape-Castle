using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceService : MonoBehaviour
{
    [SerializeField] private List<ResourceAmount> resourceAmounts = new List<ResourceAmount>();
    [SerializeField] private GameObject resourceUIPrefab;
    [SerializeField] private RectTransform canvas;

    public static ResourceService Instance { get; private set; }

    private GameObject resourceUI;

    public event Action<float> OnResourceAdded;
    public event Action<float> OnResourceRemoved;

    public float GetResourceAmount(ResourceType type) => resourceAmounts.Find(r => r.resourceType == type).amount;

    public void AddResource(ResourceType type, float amount)
    {
        var resource = resourceAmounts.Find(r => r.resourceType == type);
        resource.amount += amount;
        OnResourceAdded?.Invoke(amount);
    }

    public void SubtractResource(ResourceType type, float amount)
    {
        var resource = resourceAmounts.Find(r => r.resourceType == type);
        resource.amount -= amount;
        OnResourceRemoved?.Invoke(amount);
    }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        resourceUI = Instantiate(resourceUIPrefab, canvas);
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }
}
