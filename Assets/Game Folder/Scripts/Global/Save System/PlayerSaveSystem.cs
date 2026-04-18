using UnityEngine;

public class PlayerSaveSystem
{
    public const string StoryPhaseString = "currentStoryPhase";

    public void SetStoryPhase(StoryPhase phase)
    {
        PlayerPrefs.SetString(StoryPhaseString, phase.ToString());
    }

    public StoryPhase GetStoryPhase()
    {
        string storyPhase = PlayerPrefs.GetString(StoryPhaseString, "");

        if (System.Enum.TryParse<StoryPhase>(storyPhase, out StoryPhase result))
            return result;

        return StoryPhase.Beginning;
    }

    public void SetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
    }

    public bool GetBoold(string name)
    {
        return PlayerPrefs.GetInt(name) != 0;
    }
}
