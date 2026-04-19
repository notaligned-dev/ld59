using UnityEngine;
using VContainer;

public class PlayerController : MonoBehaviour
{
    private const float MoveTresholdValue = 0.3f;
    private const float MaximumGravityStackedValue = 20f;
    private const float GroundedVelocityY = -2f;
    private const float MaxAngleRangeAbsoluteByX = 86;
    private const float sensitivity = 0.8f;

    [Header("Settings")]
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private float _sprintSpeedModifier = 2f;
    [SerializeField] private float _gravity = 9.81f;
    [Header("Dependencies")]
    [SerializeField] private PlayerInputReader _inputReader;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerLook _look;

    private Camera _camera;
    private Vector3Int _inputMove;
    private Vector3 _velocity;
    private Vector2 _cameraRotation;
    private bool _isSprinting;

    [Inject]
    private void Construct(Camera camera)
    {
        _camera = camera;
        _isSprinting = false;
        _velocity = Vector3.zero;
        _cameraRotation = new Vector2(_camera.transform.rotation.y, _camera.transform.rotation.x);
    }

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_inputReader, nameof(_inputReader)),
            (_characterController, nameof(_characterController)),
            (_look, nameof(_look))
        );
    }

    private void OnEnable()
    {
        _inputReader.MovePressed += HandleMove;
        _inputReader.Looked += HandleInputLook;
        _inputReader.Sprinted += HandleSprint;
        _inputReader.Interacted += HandleInteraction;
        _inputReader.Holded += HandleHoldStart;
        _look.InteractableInViewChanged += HandleLookChanged;
    }

    private void OnDisable()
    {
        _inputReader.MovePressed -= HandleMove;
        _inputReader.Looked -= HandleInputLook;
        _inputReader.Sprinted -= HandleSprint;
        _inputReader.Interacted -= HandleInteraction;
        _inputReader.Holded -= HandleHoldStart;
        _look.InteractableInViewChanged -= HandleLookChanged;
    }

    private void FixedUpdate()
    {
        if (_characterController.isGrounded && _velocity.y < 0)
            _velocity.y = GroundedVelocityY;
        else
            _velocity.y = -1 * Mathf.Min(_velocity.y + _gravity, MaximumGravityStackedValue);

        CalculateHorizontalVelocity();
        _characterController.Move(Quaternion.AngleAxis(_cameraRotation.y, Vector3.up) * _velocity * Time.fixedDeltaTime);
        ResetInput();
    }

    private void HandleMove(Vector2 move)
    {
        if (Mathf.Abs(move.x) > MoveTresholdValue)
            _inputMove.x = move.x > 0 ? 1 : -1;

        if (Mathf.Abs(move.y) > MoveTresholdValue)
            _inputMove.z = move.y > 0 ? 1 : -1;
    }

    private void HandleInputLook(Vector2 look)
    {
        _cameraRotation.x -= look.y * sensitivity;
        _cameraRotation.y += look.x * sensitivity;

        _cameraRotation.x = Mathf.Clamp(_cameraRotation.x, -1 * MaxAngleRangeAbsoluteByX, MaxAngleRangeAbsoluteByX);

        _camera.transform.localRotation = Quaternion.Euler(_cameraRotation.x, _cameraRotation.y, 0f);
    }

    private void HandleSprint()
    {
        _isSprinting = true;
    }

    private void HandleLookChanged(IInteractable interactable)
    {
        interactable.TriggerLookAction();
    }

    private void HandleInteraction()
    {
        _look.CurrentInteractable?.TriggerInteraction();
    }

    private void HandleHoldStart()
    {
        Debug.Log("HOOLD!");
    }

    private void CalculateHorizontalVelocity()
    {
        var currentSpeed = _speed;

        if (Mathf.Abs(_inputMove.x) == 1 && Mathf.Abs(_inputMove.z) == 1)
            currentSpeed = Mathf.Sqrt(currentSpeed);

        if (_isSprinting)
            currentSpeed *= _sprintSpeedModifier;

        _velocity.x = currentSpeed * _inputMove.x;
        _velocity.z = currentSpeed * _inputMove.z;
    }

    private void ResetInput()
    {
        _inputMove.x = 0;
        _inputMove.z = 0;
        _isSprinting = false;
    }
}
