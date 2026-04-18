using System.Collections;
using UnityEngine;

public class DevilBookView : MonoBehaviour
{
    [SerializeField] private float _timeToRaise = 0.4f;
    [SerializeField] private Transform _book;

    private bool _isInitialzied;
    private Coroutine _raiseBookCoroutine;

    private void Awake()
    {
        _raiseBookCoroutine = null;
        _isInitialzied = false;
    }

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_book, nameof(_book))    
        );
    }

    public void Initialize()
    {
        if (_isInitialzied == false && _raiseBookCoroutine == null)
            StartCoroutine(RaiseBook());
    }

    private IEnumerator RaiseBook()
    {
        var await = new WaitForEndOfFrame();
        Vector3 currentPosition = _book.localPosition;
        float startPositionY = currentPosition.y;
        float finalPositionY = 0f;
        float startTime = Time.time;
        float timePassed = 0f;

        while (timePassed < _timeToRaise)
        {
            currentPosition.y = Mathf.Lerp(startPositionY, finalPositionY, timePassed / _timeToRaise);
            _book.localPosition = currentPosition;
            timePassed = Time.time - startTime;
            yield return await;
        }

        _isInitialzied = true;
        yield return null;
    }
}
