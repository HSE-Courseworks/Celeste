using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    [HideInInspector]
    public float horizontalInput = 0;
    [HideInInspector]
    public float verticalInput = 0;
    [HideInInspector]
    public float climbInput = 0;

    [HideInInspector]
    public bool jumpPressed;
    [HideInInspector]
    public bool dashPressed;
    [HideInInspector]
    public bool grabPressed;

    public void Move(InputAction.CallbackContext context) {
        Vector2 axis = context.ReadValue<Vector2>();

        horizontalInput = Mathf.Round(axis.x);
        verticalInput = Mathf.Round(axis.y);
    }

    public void Jump(InputAction.CallbackContext context) {
        jumpPressed = context.performed;
    }

    public void Dash(InputAction.CallbackContext context) {
        dashPressed = context.performed;
    }

    public void Grab(InputAction.CallbackContext context) {
        grabPressed = context.performed;
    }
}
