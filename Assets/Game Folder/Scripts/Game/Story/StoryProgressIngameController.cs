using System;
using System.Diagnostics;
using VContainer;
using VContainer.Unity;

public class StoryProgressIngameController : IInitializable, IDisposable
{
    private StoryObjectsProvider _objectsProvider;
    private StoryService _service;
    private DevilBookView _devilBookView;

    [Inject]
    public StoryProgressIngameController(StoryObjectsProvider objectsProvider, StoryService storyService, DevilBookView devilBookView)
    {
        _objectsProvider = objectsProvider;
        _service = storyService;
        _devilBookView = devilBookView;

    }

    public void Initialize()
    {
        SubscribeToEvents();
    }

    public void Dispose()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        _objectsProvider.Interacted += HandleInteraction;
    }

    private void UnsubscribeFromEvents()
    {
        _objectsProvider.Interacted -= HandleInteraction;
    }

    private void HandleInteraction(IStoryInteractable interactable)
    {
        if (interactable.UsedToProgress == false && _service.TryToChangePhase(interactable.PhaseToProgress))
        {
            interactable.UsedToProgress = true;
            ProcessPhaseChange(interactable.PhaseToProgress);
        }
    }

    private void ProcessPhaseChange(StoryPhase phase)
    {
        switch (phase)
        {
            case StoryPhase.DevilBookTaken:
                StartDevilBookTakenPhase();
                break;

            default:
                break;
        }
    }

    private void StartDevilBookTakenPhase()
    {
        UnityEngine.Debug.Log("Handle Devil view!");
        _devilBookView.Initialize();
    }
}
