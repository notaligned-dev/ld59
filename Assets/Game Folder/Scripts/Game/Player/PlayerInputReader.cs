using System;
using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    private PlayerInputActions _actions;

    public event Action<Vector2> MovePressed;
    public event Action<Vector2> Looked;
    public event Action Sprinted;

    private void Awake()
    {
        _actions = new PlayerInputActions();
        _actions.Enable();
    }

    private void Update()
    {
        if (_actions == null)
            return;

        var move = _actions.Player.Move;
        var look = _actions.Player.Look;
        var sprint = _actions.Player.Sprint;

        if (move.IsPressed())
            MovePressed.Invoke(move.ReadValue<Vector2>());

        if (look.WasPressedThisFrame())
            Looked?.Invoke(look.ReadValue<Vector2>());

        if (sprint.IsPressed())
            Sprinted?.Invoke();
    }
}
