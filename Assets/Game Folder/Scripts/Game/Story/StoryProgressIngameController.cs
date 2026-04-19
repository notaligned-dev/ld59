using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StoryProgressIngameController : MonoBehaviour
{
    private const float BookFixTime = 2f;

    [SerializeField] private Transform _zonePosition1;
    [SerializeField] private Transform _zonePosition2;
    [SerializeField] private Transform _zonePosition3;
    [SerializeField] private StoryProgressionObject _devilEndingFinalObject;
    [SerializeField] private StoryProgressionObject _shredder;

    private StoryObjectsProvider _objectsProvider;
    private StoryService _service;
    private DevilBookView _devilBookView;
    private DevilSymbolView _devilSymbolView;
    private PlayerController _playerController;

    [Inject]
    private void Construct(StoryObjectsProvider objectsProvider, StoryService storyService, DevilBookView devilBookView, DevilSymbolView devilSymbolView, PlayerController playerController)
    {
        _objectsProvider = objectsProvider;
        _service = storyService;
        _devilBookView = devilBookView;
        _devilSymbolView = devilSymbolView;
        _playerController = playerController;
    }

    private void OnEnable()
    {
        _objectsProvider.GroupFinished += HandleGroupFinished;
    }

    private void OnDisable()
    {
        _objectsProvider.GroupFinished -= HandleGroupFinished;
        _shredder.Interacted -= HandleGoodEnding;
        _devilEndingFinalObject.Interacted -= HandleBadEnding;
    }

    private void HandleGroupFinished(StoryPhase nextPhase)
    {
        if (_service.TryToChangePhase(nextPhase))
        {
            if (nextPhase == StoryPhase.DevilBookTaken)
            {
                _devilBookView.Initialize();
                _devilSymbolView.TryShowEye(_zonePosition1.transform);
                _playerController.FixBookForSeconds(BookFixTime);
            }
            else if (nextPhase == StoryPhase.FirstDoorOpened)
            {
                _devilSymbolView.TryShowEar();
                _playerController.FixBookForSeconds(BookFixTime);
            }
            else if (nextPhase == StoryPhase.SecretBookshelfOpened)
            {
                _devilSymbolView.TryShowSteps(_zonePosition2.transform);
                _playerController.FixBookForSeconds(BookFixTime);
            }
            else if (nextPhase == StoryPhase.AreaWithShredderEntered)
            {
                _devilSymbolView.TryShowEye(null);
                _playerController.FixBookForSeconds(BookFixTime);
            }
            else if (nextPhase == StoryPhase.PenTaken)
            {
                _devilSymbolView.TryShowSteps(_zonePosition3.transform);
                _playerController.FixBookForSeconds(BookFixTime);
            }
            else if (nextPhase == StoryPhase.ContractShown)
            {
                _devilSymbolView.TryShowHandToTake();
                _playerController.FixBookForSeconds(BookFixTime);
            }
            else if (nextPhase == StoryPhase.ContractTaken)
            {
                StartFinalPhase();
            }
        }
    }

    private void StartFinalPhase()
    {
        _shredder.Interacted += HandleGoodEnding;
        _devilEndingFinalObject.Interacted += HandleBadEnding;

        _devilSymbolView.TryShowSteps(_devilEndingFinalObject.transform);
        _playerController.FixBookForSeconds(BookFixTime);
    }

    private void HandleGoodEnding(IInteractable interactable)
    {
        Debug.Log("Good Ending!");
    }

    private void HandleBadEnding(IInteractable interactable)
    {
        Debug.Log("Bad Ending...");
    }
}
