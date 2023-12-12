using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float speedWhileBuilding = 1;
    [SerializeField] private float rotationSpeed = 360;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerBody body;

    private PlayerInputActions playerAction;
    private InputAction moveAction;
    private InputAction buildAction;
    Vector2 moveDirection = Vector2.zero;
    private bool isBuilding = false;
    private List<GridPosition> activeBuildTiles = new List<GridPosition>();

    private void Awake()
    {
        playerAction = new PlayerInputActions();
        playerAction.Enable();
        moveAction = playerAction.FindAction("Move");
        buildAction = playerAction.FindAction("Build");

        moveAction.performed += OnMoveAction;
        moveAction.canceled += OnMoveAction;

        buildAction.performed += OnBuildButtonPressed;
    }

    private void OnBuildButtonPressed(InputAction.CallbackContext context)
    {
        isBuilding = !isBuilding;

        if (!isBuilding)
        {
            GridSystemHex<GridObject> grid = LevelGrid.Instance.GetGridSystem(0);
            GridPosition gridPos = grid.GetGridPosition(transform.position);

            var auxActive = new List<GridPosition>();
            auxActive.AddRange(activeBuildTiles);

            foreach (GridPosition pos in auxActive)
            {
                grid.GetGridObject(pos).GetTile().HideBuildButton();
                activeBuildTiles.Remove(pos);
            }
        }
    }

    private void OnMoveAction(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>().normalized;
    }

    private void OnDisable()
    {
        playerAction.Disable();
    }

    private void OnEnable()
    {
        if(playerAction == null) 
        {
            return;
        }
        playerAction.Enable();
    }

    void Start()
    {
        CameraControler.Instance.GoToFollowPlayerMode(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(isBuilding)
        {
            GridSystemHex<GridObject> grid = LevelGrid.Instance.GetGridSystem(0);
            GridPosition gridPos = grid.GetGridPosition(transform.position);
            List<GridPosition> closeEnoughToBuild = grid.GetNeighbours(gridPos);

            closeEnoughToBuild.Add(gridPos);

            var auxActive = new List<GridPosition>();
            auxActive.AddRange(activeBuildTiles);

            foreach (GridPosition pos in auxActive)
            {
                if (!closeEnoughToBuild.Contains(pos))
                {
                    grid.GetGridObject(pos).GetTile().HideBuildButton();
                    activeBuildTiles.Remove(pos);
                }
            }


            foreach (var pos in closeEnoughToBuild)
            {
                grid.GetGridObject(pos).GetTile().ShowBuildButton();
                activeBuildTiles.Add(pos);
            }
        }


        rb.velocity = new Vector3(moveDirection.x, 0, moveDirection.y) * (isBuilding ? speedWhileBuilding : speed);

        body.AnimateMovement(moveDirection);
        if (moveDirection == Vector2.zero)
        {
            return;
        }


        Quaternion toRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.y), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);

    }
}
