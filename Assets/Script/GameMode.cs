using UnityEngine;

public enum GameMode { Easy, CPE }

public static class GameModeSettings
{
    const string Preference = "GameMode";
    public static GameMode Current => PlayerPrefs.GetInt(Preference, 0) == 1 ? GameMode.CPE : GameMode.Easy;
    public static bool HasHints => Current == GameMode.Easy;

    public static void Select(GameMode mode)
    {
        PlayerPrefs.SetInt(Preference, (int)mode);
        PlayerPrefs.Save();
    }
}
