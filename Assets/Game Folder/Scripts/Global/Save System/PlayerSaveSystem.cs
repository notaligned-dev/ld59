using UnityEngine;

public class PlayerSaveSystem
{
    public void SetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value  ? 1 : 0);
    }

    public bool GetBoold(string name)
    {
        return PlayerPrefs.GetInt(name) != 0;
    }
}
