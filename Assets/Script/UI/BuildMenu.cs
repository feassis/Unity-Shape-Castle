using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildMenu : MonoBehaviour
{
    private PlayerInputActions playerAction;
    private InputAction uiCancelAction;

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
