using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDisplay : MonoBehaviour
{
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Action _onComplete;

    private void Awake()
    {
        UtilitiesDD.HideCanvasGroup(_canvasGroup);
    }

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_confirmButton, nameof(_confirmButton)),
            (_cancelButton, nameof(_cancelButton)),
            (_canvasGroup, nameof(_canvasGroup))
        );
    }

    private void OnEnable()
    {
        _confirmButton.onClick.AddListener(HandleConfirm);
        _cancelButton.onClick.AddListener(HandleCancel);
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(HandleConfirm);
        _cancelButton.onClick.RemoveListener(HandleCancel);
    }

    public void Show(Action onComplete)
    {
        UtilitiesDD.ShowCanvasGroup(_canvasGroup);

        if (onComplete != null)
            _onComplete = onComplete;
    }

    public void Hide()
    {
        UtilitiesDD.HideCanvasGroup(_canvasGroup);
        _onComplete?.Invoke();
        _onComplete = null;
    }

    private void HandleConfirm()
    {
        var onComplete = _onComplete;
        Hide();
        onComplete?.Invoke();
    }

    private void HandleCancel()
    {
        Hide();
    }
}
