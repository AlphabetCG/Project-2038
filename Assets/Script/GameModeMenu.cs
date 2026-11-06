using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameModeMenu : MonoBehaviour
{
    Action startGame;
    GameMode selected;
    Button easy, cpe;
    TMP_Text selection;
    bool starting;

    public static void Open(Action startGame)
    {
        if (FindFirstObjectByType<GameModeMenu>() != null) return;
        var menu = new GameObject("Game Mode Selection").AddComponent<GameModeMenu>();
        menu.startGame = startGame;
        menu.Build();
    }

    void Build()
    {
        var canvas = ChecklistUIStyle.Canvas(transform, "Mode Canvas");
        ChecklistUIStyle.Box(canvas, "Backdrop", Vector2.zero, Vector2.one, new Color(0, 0, 0, .94f));
        var panel = ChecklistUIStyle.Box(canvas, "Mode Panel", new Vector2(.12f, .13f), new Vector2(.88f, .87f), ChecklistUIStyle.Ink);
        ChecklistUIStyle.Text(panel, "Title", "CHOOSE YOUR MODE", 36, new Vector2(.06f, .81f), new Vector2(.94f, .95f), ChecklistUIStyle.Accent);
        ChecklistUIStyle.Text(panel, "Subtitle", "Same puzzles. Choose how much guidance you want.", 22, new Vector2(.06f, .69f), new Vector2(.94f, .81f), ChecklistUIStyle.Muted);
        easy = ChecklistUIStyle.Button(panel, "Easy", "EASY", new Vector2(.06f, .49f), new Vector2(.47f, .64f), () => Select(GameMode.Easy));
        cpe = ChecklistUIStyle.Button(panel, "CPE", "CPE - CLASSIC", new Vector2(.53f, .49f), new Vector2(.94f, .64f), () => Select(GameMode.CPE));
        ChecklistUIStyle.Text(panel, "Easy Description", "Conceptual hints in your Checklist.", 22, new Vector2(.06f, .29f), new Vector2(.47f, .47f));
        ChecklistUIStyle.Text(panel, "CPE Description", "Objectives only. No Checklist hints.", 22, new Vector2(.53f, .29f), new Vector2(.94f, .47f));
        selection = ChecklistUIStyle.Text(panel, "Selection", "", 20, new Vector2(.06f, .19f), new Vector2(.94f, .28f), ChecklistUIStyle.Accent);
        ChecklistUIStyle.Button(panel, "Back", "BACK", new Vector2(.06f, .04f), new Vector2(.3f, .16f), () => Destroy(gameObject));
        ChecklistUIStyle.Button(panel, "Start", "START GAME", new Vector2(.61f, .04f), new Vector2(.94f, .16f), StartSelectedGame);
        Select(GameModeSettings.Current);
    }

    public void Select(GameMode mode)
    {
        selected = mode;
        easy.GetComponent<Image>().color = mode == GameMode.Easy ? new Color(.3f, .24f, .13f) : ChecklistUIStyle.Panel;
        cpe.GetComponent<Image>().color = mode == GameMode.CPE ? new Color(.3f, .24f, .13f) : ChecklistUIStyle.Panel;
        selection.text = "Selected: " + (mode == GameMode.Easy ? "EASY - Checklist hints enabled" : "CPE - Classic, no Checklist hints");
    }

    public void StartSelectedGame()
    {
        if (starting) return;
        starting = true;
        GameModeSettings.Select(selected);
        startGame?.Invoke();
        Destroy(gameObject);
    }

    void Update() { if (Input.GetKeyDown(KeyCode.Escape)) Destroy(gameObject); }
}
