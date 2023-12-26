using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class BuildMenu : MonoBehaviour
{
    [SerializeField] private BuildButton buttonPrefab;
    [SerializeField] private Transform menuHolder;
    [SerializeField] private GameObject buildingDescriptionMenu;
    [SerializeField] private Button buildStructureButton;
    [SerializeField] private Button closeDescriptionButton;
    [SerializeField] private Image descriptionIcon;
    [SerializeField] private TextMeshProUGUI buildNameText;
    [SerializeField] private TextMeshProUGUI buildDescriptionText;
    [SerializeField] private TextMeshProUGUI generationTimeText;
    [SerializeField] private List<ResourceText> costText;
    [SerializeField] private List<ResourceText> generationText;

    [Serializable]
    private struct ResourceText
    {
        public ResourceType type;
        public TextMeshProUGUI amountText;
    } 

    private BuildingType currentBuildingType;

    private PlayerInputActions playerAction;
    private InputAction uiCancelAction;

    private List<BuildButton> buildButtons = new List<BuildButton>();

    private void Awake()
    {
        playerAction = new PlayerInputActions();
        playerAction.Enable();
        uiCancelAction = playerAction.FindAction("UICancel");

        uiCancelAction.performed += OnUICancel;

        closeDescriptionButton.onClick.AddListener(OnCloseDescriptionClicked);
        buildStructureButton.onClick.AddListener(OnBuildStructureClicked);
    }

    private void SetupBuildingDescription(BuildingType type)
    {
        Building building = BuildingService.Instance.GetBuilding(type);

        buildNameText.text = building.GetBuildingName();
        buildDescriptionText.text = building.GetBuildDescription();
        generationTimeText.text = building.GetGenerationTime().ToString();
        descriptionIcon.sprite = building.GetBuildingIcon();

        foreach (var cost in costText)
        {
            cost.amountText.text = building.GetReourceCostAmount(cost.type).ToString();
        }


        foreach (var generation in generationText)
        {
            generation.amountText.text = building.GetReourceGenerationAmount(generation.type).ToString();
        }
    }

    private void OnBuildStructureClicked()
    {
        BuildingService.Instance.TryBuildOnTile(currentBuildingType);
        OnCloseDescriptionClicked();
        BuildingService.Instance.CloseBuildingMenu();
    }

    private void OnCloseDescriptionClicked()
    {
        currentBuildingType = BuildingType.None;
        buildingDescriptionMenu.SetActive(false);
    }

    private void OnUICancel(InputAction.CallbackContext context)
    {
        BuildingService.Instance.CloseBuildingMenu();
    }

    public void Setup(Tile tile)
    {
        if (buildButtons.Count > 0)
        {
            foreach (BuildButton b in buildButtons)
            {
                Destroy(b.gameObject);
            }

            buildButtons.Clear();
        }
        var buildOptions = BuildingService.Instance.GetBuildingOptions(tile.GetTerrain(), tile.GetTerrainModifier());

        foreach (var option in buildOptions)
        {
            BuildButton button = Instantiate(buttonPrefab, menuHolder);
            button.Setup(option, BuildingService.Instance.GetBuilding(option).GetBuildingIcon(), OpenDescriptionMenu);
            buildButtons.Add(button);
        }
        buildOptions.Clear();


    }


    private void OpenDescriptionMenu(BuildingType type)
    {
        currentBuildingType = type;
        buildingDescriptionMenu.SetActive(true);

        SetupBuildingDescription(type);
    }

    private void OnDisable()
    {
        playerAction.Disable();
    }

    private void OnEnable()
    {
        if (playerAction == null)
        {
            return;
        }
        playerAction.Enable();
    }
}
