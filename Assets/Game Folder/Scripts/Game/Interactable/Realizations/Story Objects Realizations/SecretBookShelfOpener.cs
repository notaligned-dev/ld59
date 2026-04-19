using UnityEngine;

public class SecretBookShelfOpener : StoryProgressionObject
{
    [SerializeField] private Transform _bookshelfPivot;

    public override void TriggerStoryInteraction()
    {
        var newPosition = _bookshelfPivot.localPosition;
        newPosition.z -= 5;

        _bookshelfPivot.localPosition = newPosition;
        base.TriggerStoryInteraction();
    }
}
