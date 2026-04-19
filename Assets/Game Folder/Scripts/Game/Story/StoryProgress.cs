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

            case StoryPhase.FirstDoorOpened:
                if (CurrentPhase != StoryPhase.DevilBookTaken)
                    phaseSuccessfullyChanged = false;
                break;

            case StoryPhase.SecretBookshelfOpened:
                if (CurrentPhase != StoryPhase.FirstDoorOpened)
                    phaseSuccessfullyChanged = false;
                break;

            case StoryPhase.AreaWithShredderEntered:
                if (CurrentPhase != StoryPhase.SecretBookshelfOpened)
                    phaseSuccessfullyChanged = false;
                break;

            case StoryPhase.PenTaken:
                if (CurrentPhase != StoryPhase.AreaWithShredderEntered)
                    phaseSuccessfullyChanged = false;
                break;

            case StoryPhase.ContractShown:
                if (CurrentPhase != StoryPhase.PenTaken)
                    phaseSuccessfullyChanged = false;
                break;

            case StoryPhase.ContractTaken:
                if (CurrentPhase != StoryPhase.ContractShown)
                    phaseSuccessfullyChanged = false;
                break;

            case StoryPhase.ContractSigned:
                if (CurrentPhase != StoryPhase.ContractTaken)
                    phaseSuccessfullyChanged = false;
                break;

            case StoryPhase.ContractShredded:
                if (CurrentPhase != StoryPhase.ContractTaken)
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
    Beginning = 0,
    DevilBookTaken = 1,
    FirstDoorOpened = 2,
    SecretBookshelfOpened = 3,
    AreaWithShredderEntered = 4,
    PenTaken = 5,
    ContractShown = 6,
    ContractTaken = 7,
    ContractSigned = 8,
    ContractShredded = 9,
}