using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChecklistUI : MonoBehaviour
{
    public static ChecklistUI Instance { get; private set; }
    public static bool IsOpen => Instance != null && Instance.window != null && Instance.window.activeSelf;
    public static bool BlocksPauseInput => IsOpen || Time.frameCount == lastClosedFrame;
    static int lastClosedFrame = -1;
    GameObject window, quickButton, pauseButton;
    TMP_Text progressLabel, quickLabel, detailTitle, detailBody, hintTitle, hintBody;
    readonly TMP_Text[] rows = new TMP_Text[14];
    readonly Image[] backgrounds = new Image[14];
    pausegame pause;
    FirstPersonController player;
    bool fromPause, controllerWasEnabled;
    int selected;
    GameObject previousSelection;
    ScrollRect objectiveScroll;

    void Awake() => Instance = this;

    void Start()
    {
        player = FindFirstObjectByType<FirstPersonController>();
        pause = FindFirstObjectByType<pausegame>();
        // Legacy scripts can keep their serialized references; the new page owns presentation.
        var legacy = player != null ? player.transform.Find("Check_list") : null;
        if (legacy != null) legacy.gameObject.SetActive(false);
        Build();
        ChecklistProgress.Instance.Changed += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (ChecklistProgress.Instance != null) ChecklistProgress.Instance.Changed -= Refresh;
        if (Instance == this) Instance = null;
    }

    void Build()
    {
        var canvas = ChecklistUIStyle.Canvas(transform, "Checklist Canvas");
        var quick = ChecklistUIStyle.Button(canvas, "Checklist Shortcut", "", new Vector2(.025f, .915f), new Vector2(.28f, .98f), Open);
        quickButton = quick.gameObject;
        quickLabel = quick.GetComponentInChildren<TMP_Text>();
        pauseButton = ChecklistUIStyle.Button(canvas, "Pause Checklist Button", "CHECKLIST  [Tab]", new Vector2(.71f, .91f), new Vector2(.97f, .98f), OpenFromPause).gameObject;
        var backdrop = ChecklistUIStyle.Box(canvas, "Checklist Window", Vector2.zero, Vector2.one, new Color(0, 0, 0, .94f));
        window = backdrop.gameObject;
        var panel = ChecklistUIStyle.Box(backdrop, "Panel", new Vector2(.035f, .045f), new Vector2(.965f, .955f), ChecklistUIStyle.Ink);
        ChecklistUIStyle.Text(panel, "Title", "CHECKLIST", 36, new Vector2(.035f, .87f), new Vector2(.6f, .98f), ChecklistUIStyle.Accent);
        progressLabel = ChecklistUIStyle.Text(panel, "Progress", "", 20, new Vector2(.035f, .8f), new Vector2(.75f, .88f), ChecklistUIStyle.Muted);
        ChecklistUIStyle.Button(panel, "Close", "CLOSE  [Tab / Esc]", new Vector2(.72f, .89f), new Vector2(.97f, .97f), Close);
        ChecklistUIStyle.Text(panel, "Footer", "Select an objective to view details. Scroll to see more.", 18, new Vector2(.035f, .015f), new Vector2(.97f, .075f), ChecklistUIStyle.Muted);

        var list = ChecklistUIStyle.Box(panel, "Objective List", new Vector2(.035f, .09f), new Vector2(.49f, .79f), ChecklistUIStyle.Panel);
        var scroll = list.gameObject.AddComponent<ScrollRect>();
        objectiveScroll = scroll;
        scroll.horizontal = false; scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 30;
        var viewport = ChecklistUIStyle.Box(list, "Viewport", Vector2.zero, Vector2.one);
        viewport.gameObject.AddComponent<RectMask2D>();
        var content = ChecklistUIStyle.Box(viewport, "Content", new Vector2(0, 1), Vector2.one);
        content.pivot = new Vector2(.5f, 1);
        content.sizeDelta = new Vector2(0, rows.Length * 48 + 8);
        scroll.viewport = viewport; scroll.content = content;
        for (int i = 0; i < rows.Length; i++)
        {
            int index = i;
            var button = ChecklistUIStyle.Button(content, "Objective " + i, "", new Vector2(0, 1), Vector2.one, () => Select(index));
            var rect = (RectTransform)button.transform;
            rect.pivot = new Vector2(.5f, 1); rect.anchoredPosition = new Vector2(0, -4 - i * 48);
            rect.sizeDelta = new Vector2(0, 46);
            rows[i] = button.GetComponentInChildren<TMP_Text>();
            rows[i].fontSize = 21; rows[i].alignment = TextAlignmentOptions.MidlineLeft;
            backgrounds[i] = button.GetComponent<Image>();
        }

        ChecklistUIStyle.Box(panel, "Details Background", new Vector2(.515f, .09f), new Vector2(.97f, .79f), ChecklistUIStyle.Panel);
        detailTitle = ChecklistUIStyle.Text(panel, "Objective Title", "", 28, new Vector2(.54f, .65f), new Vector2(.945f, .77f), ChecklistUIStyle.Accent);
        detailBody = ChecklistUIStyle.Text(panel, "Objective Details", "", 22, new Vector2(.54f, .48f), new Vector2(.945f, .65f));
        hintTitle = ChecklistUIStyle.Text(panel, "Hint Title", "EASY HINT", 21, new Vector2(.54f, .4f), new Vector2(.945f, .47f), ChecklistUIStyle.Accent);
        hintBody = ChecklistUIStyle.Text(panel, "Hint Body", "", 22, new Vector2(.54f, .12f), new Vector2(.945f, .4f));
        hintBody.verticalAlignment = VerticalAlignmentOptions.Top;
        hintBody.enableAutoSizing = true;
        hintBody.fontSizeMin = 18; hintBody.fontSizeMax = 22;
        detailBody.enableAutoSizing = true;
        detailBody.fontSizeMin = 18; detailBody.fontSizeMax = 22;
        window.SetActive(false);
        pauseButton.SetActive(false);
    }

    public void Select(int index)
    {
        if (index < 0 || index >= rows.Length) return;
        selected = index;
        Refresh();
    }

    void Refresh()
    {
        var progress = ChecklistProgress.Instance;
        if (progress == null || progressLabel == null) return;
        string mode = GameModeSettings.Current == GameMode.Easy ? "EASY" : "CPE - CLASSIC";
        progressLabel.text = mode + "   |   " + progress.Count + " / " + rows.Length + " complete";
        quickLabel.text = "[Tab] Checklist   " + progress.Count + "/" + rows.Length;
        for (int i = 0; i < rows.Length; i++)
        {
            bool done = progress.IsComplete((ChecklistTask)i);
            rows[i].text = (done ? "[x] " : i == progress.CurrentIndex ? ">   " : "[ ] ") + ChecklistProgress.Titles[i];
            rows[i].color = done ? new Color(.58f, .84f, .68f) : Color.white;
            backgrounds[i].color = i == selected ? new Color(.25f, .23f, .16f) : ChecklistUIStyle.Panel;
        }
        bool complete = progress.IsComplete((ChecklistTask)selected);
        detailTitle.text = ChecklistProgress.Titles[selected];
        detailBody.text = (complete ? "COMPLETED\n\n" : selected == progress.CurrentIndex ? "CURRENT OBJECTIVE\n\n" : "NOT COMPLETED\n\n") + ChecklistProgress.Descriptions[selected];
        hintTitle.gameObject.SetActive(GameModeSettings.HasHints);
        hintBody.gameObject.SetActive(GameModeSettings.HasHints);
        // Future hints stay hidden until the player reaches that objective.
        hintBody.text = GameModeSettings.HasHints
            ? (complete || selected <= progress.CurrentIndex ? ChecklistProgress.Hints[selected] : "Complete the earlier objectives to unlock this hint.")
            : "";
    }

    public void Open()
    {
        if (!GameFlow.IsInGameplay() || IsOpen) return;
        fromPause = false;
        Time.timeScale = 0;
        Show();
    }

    public void OpenFromPause()
    {
        if (pause == null || !pause.IsPaused || GameFlow.GameEnded || IsOpen) return;
        fromPause = true;
        pause.menu.SetActive(false);
        Show();
    }

    void Show()
    {
        controllerWasEnabled = player != null && player.enabled;
        if (player != null) player.enabled = false;
        previousSelection = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        window.SetActive(true);
        quickButton.SetActive(false); pauseButton.SetActive(false);
        GameFlow.UnlockCursor();
        Select(ChecklistProgress.Instance.CurrentIndex);
        Canvas.ForceUpdateCanvases();
        float overflow = objectiveScroll.content.rect.height - objectiveScroll.viewport.rect.height;
        objectiveScroll.verticalNormalizedPosition = overflow > 0 ? Mathf.Clamp01(1 - selected * 48f / overflow) : 1;
    }

    public void Close()
    {
        if (!IsOpen) return;
        lastClosedFrame = Time.frameCount;
        window.SetActive(false);
        if (GameFlow.GameEnded) { Time.timeScale = 1; GameFlow.UnlockCursor(); return; }
        if (player != null) player.enabled = controllerWasEnabled;
        if (fromPause)
        {
            pause.menu.SetActive(true);
            Time.timeScale = 0;
            GameFlow.UnlockCursor();
        }
        else
        {
            Time.timeScale = 1;
            GameFlow.LockCursor();
        }
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(previousSelection);
    }

    void Update()
    {
        if (window == null) return;
        if (IsOpen)
        {
            if (GameFlow.GameEnded || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)) Close();
            return;
        }
        quickButton.SetActive(GameFlow.IsInGameplay());
        bool paused = pause != null && pause.IsPaused && pause.menu.activeInHierarchy && !GameFlow.GameEnded;
        pauseButton.SetActive(paused);
        if (!Input.GetKeyDown(KeyCode.Tab)) return;
        if (paused) OpenFromPause(); else Open();
    }
}
