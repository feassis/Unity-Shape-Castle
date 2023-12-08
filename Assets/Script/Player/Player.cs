using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody rb;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(moveDirection);
        if(moveDirection == Vector2.zero)
        {
            return;
        }
        rb.MovePosition(transform.position + new Vector3( moveDirection.x * speed * Time.deltaTime,  0, moveDirection.y * speed * Time.deltaTime));
    }
}
