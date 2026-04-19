using Mono.Cecil;
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
    [SerializeField] private float _maxDestinationDistance = 30f;
    [Header("References")]
    [SerializeField] private DecalProjector _symbolBaseProjector;
    [SerializeField] private DecalProjector _symbolEditableProjector;
    [SerializeField] private DecalProjector _symbolEditableProjector2;
    [Header("Materials")]
    [SerializeField] private Material _emptyMaterial;
    [SerializeField] private Material _devilEyeBase;
    [SerializeField] private Material _devilEyePupil;
    [SerializeField] private Material _stopHandBase;
    [SerializeField] private Material _earEditable;
    [SerializeField] private Material _leftStepEditable;
    [SerializeField] private Material _rightStepEditable;
    [SerializeField] private Material _takeHandEditable;

    private Camera _playerCamera;
    private Coroutine _animation;
    private Coroutine _eyeWatching;
    private Coroutine _stepsTracking;
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
            (_symbolEditableProjector2, nameof(_symbolEditableProjector2)),
            (_emptyMaterial, nameof(_emptyMaterial)),
            (_devilEyeBase, nameof(_devilEyeBase)),
            (_devilEyePupil, nameof(_devilEyePupil)),
            (_stopHandBase, nameof(_stopHandBase)),
            (_earEditable, nameof(_earEditable)),
            (_leftStepEditable, nameof(_leftStepEditable)),
            (_rightStepEditable, nameof(_rightStepEditable)),
            (_takeHandEditable, nameof(_takeHandEditable))
        );
    }

    private void Awake()
    {
        _eyeWatching = null;
        _stepsTracking = null;
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

        StopAllDevilTracking();

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

        StopAllDevilTracking();

        if (_currentSymbol != DevilSymbols.StopHand)
            _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.StopHand, onShowed2: onComplete));
        else
            onComplete?.Invoke();

        _currentSymbol = DevilSymbols.StopHand;
        
        return true;
    }

    public bool TryShowHandToTake(Action onComplete = null)
    {
        if (_animation != null)
            return false;

        StopAllDevilTracking();

        if (_currentSymbol != DevilSymbols.TakeHand)
            _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.TakeHand, onShowed2: onComplete));
        else
            onComplete?.Invoke();

        _currentSymbol = DevilSymbols.TakeHand;

        return true;
    }

    public bool TryShowEar(Action onComplete = null)
    {
        if (_animation != null)
            return false;

        StopAllDevilTracking();

        if (_currentSymbol != DevilSymbols.Ear)
            _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.Ear, onShowed2: onComplete));
        else
            onComplete?.Invoke();

        _currentSymbol = DevilSymbols.Ear;

        return true;
    }

    public bool TryShowSteps(Transform stepsDestination = null, Action onComplete = null)
    {
        if (_animation != null)
            return false;

        StopAllDevilTracking();

        if (_currentSymbol == DevilSymbols.Steps)
        {
            SetStepsTarget(stepsDestination);
        }
        else
        {
            _currentSymbol = DevilSymbols.Steps;
            _animation = StartCoroutine(ChangeShowingSymbol(DevilSymbols.Steps, onShowed: () => SetStepsTarget(stepsDestination), onShowed2: onComplete));
        }

        return true;
    }

    private void SetStepsTarget(Transform earDestination = null)
    {
        if (earDestination != null)
            _stepsTracking = StartCoroutine(TrackStepsDestination(earDestination));
    }

    private void SetEyeTarget(Transform eyeDirectionToWatch = null)
    {
        if (eyeDirectionToWatch != null)
            _eyeWatching = StartCoroutine(TrackEyeDestination(eyeDirectionToWatch));
        else
            _symbolEditableProjector.transform.localPosition = _initialEditableProjectorPosition;
    }

    private IEnumerator TrackEyeDestination(Transform destination)
    {
        var await = new WaitForEndOfFrame();
        _symbolEditableProjector.transform.localPosition = _initialEditableProjectorPosition;
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

    private IEnumerator TrackStepsDestination(Transform destination)
    {
        var await = new WaitForEndOfFrame();
        var initialSizeEditable1 = _initialEditableProjectorSize;
        var initialSizeEditable2 = _initialEditableProjectorSize;
        float currentFormulaX = 0;
        int initialFormulaXChange = 1;
        float speedCoefficient = 3.0f;
        float minSpeedCoefficient = 3.0f;
        float maxSpeedCoefficient = 10.0f;
        float formulaShrinkCoefficient = 0.25f;
        int formulaShift = 1;

        while (destination != null)
        {
            var distance = Vector3.Distance(destination.position, transform.position);

            speedCoefficient = Mathf.Lerp(minSpeedCoefficient, maxSpeedCoefficient, distance / _maxDestinationDistance);

            var sizeCoefficient1 = (Mathf.Sin(currentFormulaX * Mathf.Deg2Rad) * formulaShrinkCoefficient + formulaShift);

            initialSizeEditable1.x = _initialEditableProjectorSize.x * sizeCoefficient1;
            initialSizeEditable1.y = _initialEditableProjectorSize.y * sizeCoefficient1;

            var sizeCoefficient2 = (Mathf.Cos(currentFormulaX * Mathf.Deg2Rad) * formulaShrinkCoefficient + formulaShift);

            initialSizeEditable2.x = _initialEditableProjectorSize.x * sizeCoefficient2;
            initialSizeEditable2.y = _initialEditableProjectorSize.y * sizeCoefficient2;

            _symbolEditableProjector.size = new Vector3(initialSizeEditable1.x, initialSizeEditable1.y, _symbolEditableProjector.size.z);
            _symbolEditableProjector2.size = new Vector3(initialSizeEditable2.x, initialSizeEditable2.y, _symbolEditableProjector.size.z);

            currentFormulaX += initialFormulaXChange * Mathf.Clamp(speedCoefficient, minSpeedCoefficient, maxSpeedCoefficient);

            yield return await;
        }

        _symbolEditableProjector.size = new Vector3(_initialEditableProjectorSize.x, _initialEditableProjectorSize.y, _symbolEditableProjector.size.z);
        _symbolEditableProjector2.size = new Vector3(_initialEditableProjectorSize.x, _initialEditableProjectorSize.y, _symbolEditableProjector.size.z);
        _stepsTracking = null;
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
                _symbolEditableProjector2.material = _emptyMaterial;
                break;

            case DevilSymbols.StopHand:
                _symbolBaseProjector.material = _stopHandBase;
                _symbolEditableProjector.material = _emptyMaterial;
                _symbolEditableProjector2.material = _emptyMaterial;
                break;

            case DevilSymbols.Ear:
                _symbolBaseProjector.material = _emptyMaterial;
                _symbolEditableProjector.material = _earEditable;
                _symbolEditableProjector2.material = _emptyMaterial;
                break;

            case DevilSymbols.Steps:
                _symbolBaseProjector.material = _emptyMaterial;
                _symbolEditableProjector.material = _leftStepEditable;
                _symbolEditableProjector2.material = _rightStepEditable;
                break;

            case DevilSymbols.TakeHand:
                _symbolBaseProjector.material = _emptyMaterial;
                _symbolEditableProjector.material = _takeHandEditable;
                _symbolEditableProjector2.material = _emptyMaterial;
                break;

            default:
                _symbolBaseProjector.material = _emptyMaterial;
                _symbolEditableProjector.material = _emptyMaterial;
                _symbolEditableProjector2.material = _emptyMaterial;
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

        if (_stepsTracking != null)
        {
            StopCoroutine(_stepsTracking);
            _stepsTracking = null;
        }
    }
}
