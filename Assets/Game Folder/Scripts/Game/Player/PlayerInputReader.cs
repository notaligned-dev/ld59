using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInputReader : MonoBehaviour
{
    private PlayerInputActions _actions;

    public event Action<Vector2> MovePressed;
    public event Action<Vector2> Looked;
    public event Action Sprinted;
    public event Action Interacted;
    public event Action Holded;

    private void Awake()
    {
        _actions = new PlayerInputActions();
        _actions.Enable();
    }

    private void OnEnable()
    {
        _actions.Player.Interact.performed += HandleInteracted;
    }

    private void OnDisable()
    {
        _actions.Player.Interact.performed -= HandleInteracted;
    }

    private void Update()
    {
        if (_actions == null)
            return;

        var move = _actions.Player.Move;
        var look = _actions.Player.Look;
        var sprint = _actions.Player.Sprint;

        if (move.IsPressed())
            MovePressed?.Invoke(move.ReadValue<Vector2>());

        if (look.WasPressedThisFrame())
            Looked?.Invoke(look.ReadValue<Vector2>());

        if (sprint.IsPressed())
            Sprinted?.Invoke();
    }

    private void HandleInteracted(InputAction.CallbackContext context)
    {
        if (context.interaction is TapInteraction)
            Interacted?.Invoke();
        else if (context.interaction is HoldInteraction)
            Holded?.Invoke();
    }
}
