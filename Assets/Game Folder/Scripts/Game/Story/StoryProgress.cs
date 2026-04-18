using System;
using VContainer;

public class StoryProgress
{
    private PlayerSaveSystem _saveSystem;

    public StoryPhase CurrentPhase { get; private set; }

    [Inject]
    public StoryProgress(PlayerSaveSystem saveSystem)
    {
        _saveSystem = saveSystem;

        CurrentPhase = _saveSystem.GetStoryPhase();
    }

    public bool TrySetPhase(StoryPhase phase)
    {
        bool phaseSuccessfullyChanged = true;

        switch (phase)
        {
            case StoryPhase.Beginning:
                phaseSuccessfullyChanged = false;
                break;

            case StoryPhase.DevilBookTaken:
                if (CurrentPhase != StoryPhase.Beginning)
                    phaseSuccessfullyChanged = false;
                break;

            default:
                phaseSuccessfullyChanged = false;
                break;
        }

        if (phaseSuccessfullyChanged)
            CurrentPhase = phase;

        return phaseSuccessfullyChanged;
    }
}

public enum StoryPhase {
    Beginning,
    DevilBookTaken
}