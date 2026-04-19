using System;
using System.Diagnostics;
using VContainer;
using VContainer.Unity;

public class StoryProgressIngameController : IInitializable, IDisposable
{
    private StoryObjectsProvider _objectsProvider;
    private StoryService _service;
    private DevilBookView _devilBookView;
    private DevilSymbolView _devilSymbolView;
    private PlayerController _playerController;

    [Inject]
    public StoryProgressIngameController(StoryObjectsProvider objectsProvider, StoryService storyService, DevilBookView devilBookView, DevilSymbolView devilSymbolView, PlayerController playerController)
    {
        _objectsProvider = objectsProvider;
        _service = storyService;
        _devilBookView = devilBookView;
        _devilSymbolView = devilSymbolView;
        _playerController = playerController;
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
            UnityEngine.Debug.Log($"{nextPhase}");

            if (nextPhase == StoryPhase.DevilBookTaken)
            {
                _devilBookView.Initialize();
                _playerController.IsWatchingBookFixed = true;
                _devilSymbolView.TryShowEye(onComplete: UnlockBookWatch);
            }
            else if (nextPhase == StoryPhase.FirstDoorOpened)
            {
                _playerController.IsWatchingBookFixed = true;
                _devilSymbolView.TryShowHand(onComplete: UnlockBookWatch);
            }
        }
    }

    private void UnlockBookWatch()
    {
        _playerController.IsWatchingBookFixed = false;
    }
}
