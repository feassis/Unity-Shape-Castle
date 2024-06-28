using System;

[Serializable]
public class ResourceAmount
{
    public ResourceType resourceType;
    public float amount;
}

[Serializable]
public class ResourceAmountWallet: ResourceAmount
{
    public float maxAmount;
}