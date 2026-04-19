using System.Collections;
using TMPro;
using UnityEngine;

public class DevilBookView : MonoBehaviour
{
    private const float HighestBookPositionY = 0.3f;
    private const float DefaultBookPosition = 0f;

    [SerializeField] private float _timeToRaiseInitally = 0.4f;
    [SerializeField] private Transform _book;
    [SerializeField] private TMP_Text _symbol;

    private bool _isInitialzied;
    private Coroutine _raiseBookCoroutine;

    private void Awake()
    {
        _symbol.text = "";
        _raiseBookCoroutine = null;
        _isInitialzied = false;
    }

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_book, nameof(_book)),
            (_symbol, nameof(_symbol))
        );
    }

    public void Initialize()
    {
        if (_isInitialzied == false && _raiseBookCoroutine == null)
            StartCoroutine(RaiseBook());
    }

    public void ChangeSymbol(DevilSymbols symbol)
    {
        switch (symbol)
        {
            case DevilSymbols.DirectionEye:
                _symbol.text = "Eye";
                break;

            default:
                ClearSymbol();
                break;
        }
    }

    public void WatchBook()
    {
        if (_isInitialzied == false)
            return;

        var newPosition = _book.localPosition;
        newPosition.y = Mathf.Clamp(newPosition.y + (HighestBookPositionY - DefaultBookPosition) / 10, DefaultBookPosition, HighestBookPositionY);
        _book.localPosition = newPosition;
    }

    public void ReleaseBook()
    {
        if (_isInitialzied == false)
            return;

        var newPosition = _book.localPosition;
        newPosition.y = Mathf.Clamp(newPosition.y - (HighestBookPositionY - DefaultBookPosition) / 10, DefaultBookPosition, HighestBookPositionY);
        _book.localPosition = newPosition;
    }

    private IEnumerator RaiseBook()
    {
        var await = new WaitForEndOfFrame();
        Vector3 currentPosition = _book.localPosition;
        float startPositionY = currentPosition.y;
        float finalPositionY = DefaultBookPosition;
        float startTime = Time.time;
        float timePassed = 0f;

        while (timePassed < _timeToRaiseInitally)
        {
            currentPosition.y = Mathf.Lerp(startPositionY, finalPositionY, timePassed / _timeToRaiseInitally);
            _book.localPosition = currentPosition;
            timePassed = Time.time - startTime;
            yield return await;
        }

        _isInitialzied = true;
        yield return null;
    }

    private void ClearSymbol()
    {
        _symbol.text = "";
    }
}

public enum DevilSymbols
{
    None = 0,
    DirectionEye = 1
}
