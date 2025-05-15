using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : Singleton<PlayerController> {
    private PlayerInput playerInput;
    private InputAction pointerLocationAction;
    private InputAction clickAction;
    private Vector2 PointerLocation => pointerLocationAction.ReadValue<Vector2>();  

    public event Action<Vector2> OnClick;

    private void Start () {
        playerInput = GetComponent<PlayerInput>();
        pointerLocationAction = playerInput.actions["PointerLocation"];
        clickAction = playerInput.actions["Click"];

        clickAction.performed += context => OnClick?.Invoke(PointerLocation);
    }
}
