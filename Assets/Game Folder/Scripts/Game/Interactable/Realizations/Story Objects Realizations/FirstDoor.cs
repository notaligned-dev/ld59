using UnityEngine;

public class FirstDoor : StoryProgressionObject
{
    [SerializeField] private Transform _doorPivot;

    private void OnValidate()
    {
        UtilitiesDD.RequireNotNull(
            (_doorPivot, nameof(_doorPivot))
        );
    }

    public override void TriggerStoryLookAction()
    {
        var newRotation = _doorPivot.eulerAngles;
        newRotation.y = 90;

        _doorPivot.eulerAngles = newRotation;
        base.TriggerStoryLookAction();
    }
}
