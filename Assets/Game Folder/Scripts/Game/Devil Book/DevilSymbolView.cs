using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    private Coroutine _animation;
    private DevilSymbols _currentSymbol;

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
