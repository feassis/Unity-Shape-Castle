using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildMenu : MonoBehaviour
{
    [SerializeField] private BuildButton buttonPrefab;
    [SerializeField] private Transform menuHolder;
    private PlayerInputActions playerAction;
    private InputAction uiCancelAction;

    private List<BuildButton> buildButtons = new List<BuildButton>();

    private void Awake()
    {
        playerAction = new PlayerInputActions();
        playerAction.Enable();
        uiCancelAction = playerAction.FindAction("UICancel");

        uiCancelAction.performed += OnUICancel;
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
            button.Setup(option, BuildingService.Instance.GetBuilding(option).GetBuildingIcon());
            buildButtons.Add(button);
        }
        buildOptions.Clear();


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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
