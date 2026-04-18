public interface IStoryInteractable : IInteractable
{
    public StoryPhase PhaseToProgress { get; }
    public bool UsedToProgress { get; set;  }
}
