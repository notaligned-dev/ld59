using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VContainer;

public class DevilSymbolView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.25f;
    [SerializeField] private float _animationSwitchDelay = 0.1f;
    [SerializeField] private float _maxHearingDistance = 12.5f;
    [Header("References")]
    [SerializeField] private DecalProjector _symbolBaseProjector;
    [SerializeField] private DecalProjector _symbolEditableProjector;
    [Header("Materials")]
    [SerializeField] private Material _emptyMaterial;
    [SerializeField] private Material _devilEyeBase;
    [SerializeField] private Material _devilEyePupil;
    [SerializeField] private Material _stopHandBase;
    [SerializeField] private Material _earEditable;

    private Camera _playerCamera;
    private Coroutine _animation;
    private Coroutine _eyeWatching;
    private Coroutine _earHearing;
    private DevilSymbols _currentSymbol;
    private Vector3 _initialEditableProjectorPosition;
    private Vector2 _initialEditableProjectorSize;
    private Vector2 _maxEditableProjectorSize;
    private Vector2 _minEditableProjectorSize;

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
            (_stopHandBase, nameof(_stopHandBase)),
            (_earEditable, nameof(_earEditable))
        );
    }

    private void Awake()
    {
        _eyeWatching = null;
        _earHearing = null;
        _initialEditableProjectorPosition = _symbolEditableProjector.transform.localPosition;
        _symbolBaseProjector.fadeFactor = 0f;
        _symbolEditableProjector.fadeFactor = 0f;
        _initialEditableProjectorSize.x = _symbolEditableProjector.size.x;
        _initialEditableProjectorSize.y = _symbolEditableProjector.size.y;
        SetSymbol(DevilSymbols.None);
        _minEditableProjectorSize = _initialEditableProjectorSize * 0.5f;
        _maxEditableProjectorSize = _initialEditableProjectorSize * 2f;
    }

    public bool TryShowEye(Transform eyeDirectionToWatch = null, Action onComplete = null)
    {
        if (_animation != null)
            return false;

        if (_currentSymbol == DevilSymbols.DirectionEye)
        {
            SetEyeTarget(eyeDirectionToWatch);
        }
        else
        {
            _currentSymbol = DevilSymbols.DirectionEye;
            _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.DirectionEye, onShowed: () => SetEyeTarget(eyeDirectionToWatch), onShowed2: onComplete));
        }

        return true;
    }

    public bool TryShowHand(Action onComplete = null)
    {
        if (_animation != null)
            return false;

        _currentSymbol = DevilSymbols.StopHand;

        if (_currentSymbol != DevilSymbols.StopHand)
            _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.StopHand, onShowed2: onComplete));
        
        return true;
    }

    public bool TryShowEar(Transform earDestination = null, Action onComplete = null)
    {
        if (_animation != null)
            return false;

        if (_currentSymbol == DevilSymbols.Ear)
        {
            SetEarTarget(earDestination);
        }
        else
        {
            _currentSymbol = DevilSymbols.Ear;
            _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.Ear, onShowed: () => SetEarTarget(earDestination), onShowed2: onComplete));
        }

        return true;
    }

    private void SetEarTarget(Transform earDestination = null)
    {
        StopAllDevilTracking();

        if (earDestination != null)
            _earHearing = StartCoroutine(HearDestination(earDestination));
    }

    private void SetEyeTarget(Transform eyeDirectionToWatch = null)
    {
        StopAllDevilTracking();

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

    private IEnumerator HearDestination(Transform destination)
    {
        var await = new WaitForEndOfFrame();
        var initialSize = _initialEditableProjectorSize;

        while (destination != null)
        {
            float distance = Vector3.Distance(transform.position, destination.position);

            initialSize.x = Mathf.Lerp(_maxEditableProjectorSize.x, _minEditableProjectorSize.x, distance / _maxHearingDistance);
            initialSize.y = Mathf.Lerp(_maxEditableProjectorSize.y, _minEditableProjectorSize.y, distance / _maxHearingDistance);

            _symbolEditableProjector.size = new Vector3(initialSize.x, initialSize.y, _symbolEditableProjector.size.z);

            yield return await;
        }

        _symbolEditableProjector.size = new Vector3(_initialEditableProjectorSize.x, _initialEditableProjectorSize.y, _symbolEditableProjector.size.z);
        _earHearing = null;
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

        _animation = null;
        onShowed?.Invoke();
        onShowed2?.Invoke();
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

            case DevilSymbols.Ear:
                _symbolBaseProjector.material = _emptyMaterial;
                _symbolEditableProjector.material = _earEditable;
                break;
            default:
                _symbolBaseProjector.material = _emptyMaterial;
                _symbolEditableProjector.material = _emptyMaterial;
                break;
        }
    }

    private void StopAllDevilTracking()
    {
        if (_eyeWatching != null)
        {
            StopCoroutine(_eyeWatching);
            _eyeWatching = null;
        }

        if (_earHearing != null)
        {
            StopCoroutine(_earHearing);
            _earHearing = null;
        }
    }
}
