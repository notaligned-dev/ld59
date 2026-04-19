using System;
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
        _objectsProvider.GroupFinished += HandleGroupFinished;
    }

    private void UnsubscribeFromEvents()
    {
        _objectsProvider.GroupFinished -= HandleGroupFinished;
    }

    private void HandleGroupFinished(StoryPhase nextPhase)
    {
        if (_service.TryToChangePhase(nextPhase))
        {
            if (nextPhase == StoryPhase.DevilBookTaken)
                _devilBookView.Initialize();


        }
    }
}
