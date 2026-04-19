using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;

public class DevilSymbolView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.25f;
    [SerializeField] private float _animationSwitchDelay = 0.1f;
    [Header("References")]
    [SerializeField] private DecalProjector _symbolBaseProjector;
    [SerializeField] private DecalProjector _symbolEditableProjector;
    [Header("Materials")]
    [SerializeField] private Material _emptyMaterial;
    [SerializeField] private Material _devilEyeBase;
    [SerializeField] private Material _devilEyePupil;
    [SerializeField] private Material _stopHandBase;

    private Camera _playerCamera;
    private Coroutine _animation;
    private Coroutine _eyeWatching;
    private DevilSymbols _currentSymbol;
    private Vector3 _initialEditableProjectorPosition;

    [Inject]
    private void Construct(Camera playerCamera)
    {
        _playerCamera = playerCamera;
    }

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_symbolBaseProjector, nameof(_symbolBaseProjector)),
            (_symbolEditableProjector, nameof(_symbolEditableProjector)),
            (_emptyMaterial, nameof(_emptyMaterial)),
            (_devilEyeBase, nameof(_devilEyeBase)),
            (_devilEyePupil, nameof(_devilEyePupil)),
            (_stopHandBase, nameof(_stopHandBase))
        );
    }

    private void Awake()
    {
        _initialEditableProjectorPosition = _symbolEditableProjector.transform.localPosition;
        _symbolBaseProjector.fadeFactor = 0f;
        _symbolEditableProjector.fadeFactor = 0f;
        SetSymbol(DevilSymbols.None);
    }

    public bool TryShowEye(Transform eyeDirectionToWatch = null, Action onComplete = null)
    {
        if (_animation != null)
            return false;

        if (_currentSymbol == DevilSymbols.DirectionEye)
            SetEyeTarget(eyeDirectionToWatch);
        else 
            _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.DirectionEye, onShowed: () => SetEyeTarget(eyeDirectionToWatch), onShowed2: onComplete));

        return true;
    }

    public bool TryShowHand(Action onComplete = null)
    {
        if (_animation != null)
            return false;

        _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.StopHand, onShowed2: onComplete));
        return true;
    }

    private void SetEyeTarget(Transform eyeDirectionToWatch = null)
    {
        if (_eyeWatching != null)
        {
            StopCoroutine(TrackDestination(eyeDirectionToWatch));
            _eyeWatching = null;
        }

        if (eyeDirectionToWatch != null)
            _eyeWatching = StartCoroutine(TrackDestination(eyeDirectionToWatch));
    }

    private IEnumerator TrackDestination(Transform destination)
    {
        var await = new WaitForEndOfFrame();
        var initialPosition = _initialEditableProjectorPosition;

        while (destination != null)
        {
            Vector3 cameraDirection = _playerCamera.transform.forward;
            cameraDirection.y = 0f;
            Vector3 destinationDirection = destination.transform.position - transform.position;
            destinationDirection.y = 0f;

            var angle = Vector3.SignedAngle(cameraDirection, destinationDirection, Vector3.up);
            var projectorPosition = initialPosition;
            projectorPosition.y += Mathf.Cos(angle * Mathf.Deg2Rad) * 0.1f;
            projectorPosition.x += Mathf.Sin(angle * Mathf.Deg2Rad) * 0.1f;
            _symbolEditableProjector.transform.localPosition = projectorPosition;

            yield return await;
        }

        _symbolEditableProjector.transform.localPosition = _initialEditableProjectorPosition;
        _eyeWatching = null;
        yield return null;
    }

    private IEnumerator ChangeShowingSymbol(DevilSymbols symbol, Action onHide = null, Action onShowed = null, Action onShowed2 = null)
    {
        var await = new WaitForEndOfFrame();
        float timePassed = 0f;
        float startTime = Time.time;
        float startFadeFactorBase = _symbolBaseProjector.fadeFactor;
        float startFadeFactorEditable = _symbolEditableProjector.fadeFactor;

        while (timePassed < _animationDuration)
        {
            _symbolBaseProjector.fadeFactor = Mathf.Lerp(startFadeFactorBase, 0f, timePassed / _animationDuration);
            _symbolEditableProjector.fadeFactor = Mathf.Lerp(startFadeFactorEditable, 0f, timePassed / _animationDuration);
            timePassed = Time.time - startTime;
            yield return await;
        }

        onHide?.Invoke();
        SetSymbol(symbol);

        timePassed = 0f;
        startTime = Time.time;

        while (timePassed < _animationSwitchDelay)
        {
            timePassed = Time.time - startTime;
            yield return await;
        }

        timePassed = 0f;
        startTime = Time.time;

        while (timePassed < _animationDuration)
        {
            _symbolBaseProjector.fadeFactor = Mathf.Lerp(startFadeFactorBase, 1f, timePassed / _animationDuration);
            _symbolEditableProjector.fadeFactor = Mathf.Lerp(startFadeFactorEditable, 1f, timePassed / _animationDuration);
            timePassed = Time.time - startTime;
            yield return await;
        }

        onShowed?.Invoke();
        onShowed2?.Invoke();
        _animation = null;
        yield return null;
    }

    private void SetSymbol(DevilSymbols symbol)
    {
        switch (symbol)
        {
            case DevilSymbols.DirectionEye:
                _symbolBaseProjector.material = _devilEyeBase;
                _symbolEditableProjector.material = _devilEyePupil;
                break;

            case DevilSymbols.StopHand:
                _symbolBaseProjector.material = _stopHandBase;
                _symbolEditableProjector.material = _emptyMaterial;
                break;

            default:
                _symbolBaseProjector.material = _emptyMaterial;
                _symbolEditableProjector.material = _emptyMaterial;
                break;
        }
    }
}
