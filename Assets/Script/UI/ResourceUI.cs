using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private List<ResourceText> resourceTexts = new List<ResourceText>();

    [Serializable]
    private struct ResourceText
    {
        public ResourceType type;
        public TextMeshProUGUI amountText;
        public TextMeshProUGUI maxAmountText;
    }

    private void Start()
    {
        ResourceService.Instance.OnResourceAdded += ResourceService_OnResourceChanged;
        ResourceService.Instance.OnResourceRemoved += ResourceService_OnResourceChanged;
        
        foreach (var resource in resourceTexts)
        {
            resource.amountText.text = ResourceService.Instance.GetResourceAmount(resource.type).ToString();
            resource.maxAmountText.text = ResourceService.Instance.GetResourceMaxAmount(resource.type).ToString();
        }
    }

    private void ResourceService_OnResourceChanged(ResourceType type, float amount)
    {
        var resourceText = resourceTexts.Find(r => r.type == type);
        resourceText.amountText.text = amount.ToString();
    }

    private void OnDestroy()
    {
        ResourceService.Instance.OnResourceAdded -= ResourceService_OnResourceChanged;
        ResourceService.Instance.OnResourceRemoved -= ResourceService_OnResourceChanged;
    }
}
