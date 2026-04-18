using System;
using VContainer;

public class StoryService
{
    private StoryProgress _progress;

    [Inject]
    public StoryService(StoryProgress progress)
    {
        _progress = progress;
    }

    public bool CanContinueStory()
    {
        if (_progress.CurrentPhase == StoryPhase.Beginning)
            return false;

        return true;
    }

    public bool TryToChangePhase(StoryPhase newPhase)
    {
        if (_progress.TrySetPhase(newPhase) == false)
            throw new InvalidOperationException(nameof(TryToChangePhase));

        return true;
    }
}
