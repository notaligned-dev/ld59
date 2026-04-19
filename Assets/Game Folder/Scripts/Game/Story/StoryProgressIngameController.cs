using System;
using System.Diagnostics;
using UnityEngine;
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
            if (nextPhase == StoryPhase.DevilBookTaken)
            {
                _devilBookView.Initialize();
                _playerController.IsWatchingBookFixed = true;
                GameObject myEmptyObj = new("hardcode goal 1");
                myEmptyObj.transform.position = new Vector3(-4f, 2.55f, -2.1f);
                _devilSymbolView.TryShowEye(myEmptyObj.transform, onComplete: UnfixBook);
            }
            else if (nextPhase == StoryPhase.FirstDoorOpened)
            {
                _playerController.IsWatchingBookFixed = true;
                GameObject myEmptyObj = new("hardcode goal 2");
                myEmptyObj.transform.position = new Vector3(0.5f, 1.7f, 16.5f);
                //_devilSymbolView.TryShowHand(onComplete: UnlockBookWatch);
                _devilSymbolView.TryShowEar(myEmptyObj.transform, onComplete: UnfixBook);
            }
        }
    }

    private void UnfixBook()
    {
        _playerController.IsWatchingBookFixed = false;
    }
}
