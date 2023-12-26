using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button buildButton;

    private BuildingType buildingType;

    public Action<BuildingType> buildingTypeAction;

    public void Setup(BuildingType buildingType, Sprite sprite, Action<BuildingType> onClick)
    {
        this.buildingType = buildingType;
        icon.sprite = sprite;
        buildingTypeAction = onClick;
    }

    private void Awake()
    {
        buildButton.onClick.AddListener(OnBuildButtonClicked);
    }

    private void OnBuildButtonClicked()
    {
        buildingTypeAction?.Invoke(buildingType);
    }
}
