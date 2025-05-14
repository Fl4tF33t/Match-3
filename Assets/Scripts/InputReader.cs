using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour {

    private PlayerInput playerInput;
    private InputAction selectAction;
    private InputAction fireAction;

    public event Action OnFire;
    public Vector2 Selected => selectAction.ReadValue<Vector2>();  

    private void Start () {
        playerInput = GetComponent<PlayerInput>();
        selectAction = playerInput.actions["Select"];
        fireAction = playerInput.actions["Fire"];

        fireAction.performed += TriggerOnFire;
    }

    private void OnDestroy(){
        fireAction.performed -= TriggerOnFire;
    }

    private void TriggerOnFire(InputAction.CallbackContext context) => OnFire?.Invoke();
}