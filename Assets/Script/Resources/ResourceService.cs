using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceService : MonoBehaviour
{
    [SerializeField] private List<ResourceAmountWallet> resourceAmounts = new List<ResourceAmountWallet>();
    [SerializeField] private GameObject resourceUIPrefab;
    [SerializeField] private RectTransform canvas;


    public static ResourceService Instance { get; private set; }

    private GameObject resourceUI;

    public event Action<ResourceType, float> OnResourceAdded;
    public event Action<ResourceType, float> OnResourceRemoved;

    public float GetResourceAmount(ResourceType type) => resourceAmounts.Find(r => r.resourceType == type).amount;
    public float GetResourceMaxAmount(ResourceType type) => resourceAmounts.Find(r => r.resourceType == type).maxAmount;

    public void AddResource(ResourceType type, float amount)
    {
        var resource = resourceAmounts.Find(r => r.resourceType == type);
        resource.amount = Mathf.Clamp(resource.amount + amount, 0, resource.maxAmount);
        OnResourceAdded?.Invoke(type, amount);
    }

    public void SubtractResource(ResourceType type, float amount)
    {
        var resource = resourceAmounts.Find(r => r.resourceType == type);
        resource.amount = Mathf.Clamp(resource.amount - amount, 0, resource.maxAmount);
        OnResourceRemoved?.Invoke(type, amount);
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
