using System;
using System.Collections.Generic;
using UnityEngine;

public class StoryObjectsProvider : MonoBehaviour
{
    [SerializeField] private List<StoryProgressionObject> _storyProgressionObjects;

    public event Action<IStoryInteractable> Interacted;

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_storyProgressionObjects, nameof(_storyProgressionObjects))    
        );

        if (_storyProgressionObjects.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(_storyProgressionObjects));
    }

    private void OnEnable()
    {
        foreach (var obj in _storyProgressionObjects)
            obj.Interacted += HandleInteraction;
    }

    private void OnDisable()
    {
        foreach (var obj in _storyProgressionObjects)
            obj.Interacted -= HandleInteraction;
    }

    private void HandleInteraction(IInteractable interactable)
    {
        if (interactable is IStoryInteractable storyInteractable)
            Interacted?.Invoke(storyInteractable);
    }
}
