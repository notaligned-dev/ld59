using System;
using UnityEngine;
using VContainer;

public class PlayerLook : MonoBehaviour
{
    private Camera _camera;
    private LayerMask _interactableLayer;
    private IInteractable _lastInteractableLookedAt;
    private float _interactRange;

    public IInteractable CurrentInteractable => _lastInteractableLookedAt;

    public event Action<IInteractable> InteractableInViewChanged;

    [Inject]
    private void Construct(IPlayerConfigurable playerConfig, Camera camera)
    {
        _interactRange = playerConfig.InteractRange;
        _interactableLayer = playerConfig.InteractLayers;
        _lastInteractableLookedAt = null;
        _camera = camera;
    }

    private void FixedUpdate()
    {
        var viewRay = new Ray(_camera.transform.position, _camera.transform.forward);

        IInteractable foundInteractable = Physics.Raycast(viewRay, out RaycastHit hit, _interactRange, _interactableLayer) &&  hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable) ? interactable : null;

        if (foundInteractable != _lastInteractableLookedAt && foundInteractable != null)
            InteractableInViewChanged?.Invoke(foundInteractable);

        _lastInteractableLookedAt = foundInteractable;
    }
}
