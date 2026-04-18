using UnityEngine;

public class StoryProgressionObject : Interactable, IStoryInteractable
{
    [SerializeField] private StoryPhase _phaseToProgress;

    public StoryPhase PhaseToProgress => _phaseToProgress;
    public bool UsedToProgress { get; set; }

    private void Awake()
    {
        UsedToProgress = false;
    }
}
