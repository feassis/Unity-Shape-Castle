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

    public void Setup(BuildingType buildingType, Sprite sprite)
    {
        this.buildingType = buildingType;
        icon.sprite = sprite;
    }

    private void Awake()
    {
        buildButton.onClick.AddListener(OnBuildButtonClicked);
    }

    private void OnBuildButtonClicked()
    {
        BuildingService.Instance.TryBuildOnTile(buildingType);
    }
}
