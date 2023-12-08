using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float speedWhileBuilding = 1;
    [SerializeField] private float rotationSpeed = 360;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerBody body;

    private PlayerInputActions playerAction;
    private InputAction moveAction;
    Vector2 moveDirection = Vector2.zero;

    private void Awake()
    {
        playerAction = new PlayerInputActions();
        playerAction.Enable();
        moveAction = playerAction.FindAction("Move");

        moveAction.performed += OnMoveAction;
        moveAction.canceled += OnMoveAction;
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
        rb.velocity = new Vector3(moveDirection.x, 0, moveDirection.y) * speed;

        body.AnimateMovement(moveDirection);
        if (moveDirection == Vector2.zero)
        {
            return;
        }


        Quaternion toRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.y), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);

    }
}
