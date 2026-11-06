using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

// Video completion owns the transition; waitTime is only for screens without a video.
public class StartingScreen : MonoBehaviour
{
    public GameObject startingScreen;
    public float waitTime;
    public VideoPlayer video;
    public GameObject skipHint;
    public GameObject[] hideOnFinish;
    public bool canSkip = true;
    public float skipDelay = 1f;
    public UnityEvent onFinished = new UnityEvent();

    public static bool IsPlaying => active.Count > 0;
    public static bool BlocksGameplayInput => IsPlaying || Time.frameCount == lastSkipFrame;
    static int lastSkipFrame = -1;
    static readonly HashSet<StartingScreen> active = new HashSet<StartingScreen>();
    readonly List<GameObject> videoScreens = new List<GameObject>();
    bool initialized, registered, finished;
    float startTime;
    const float PlaybackTimeout = 15f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetState()
    {
        active.Clear();
        lastSkipFrame = -1;
        CutsceneSession.Reset();
    }

    // Prepare synchronously when a quest activates its video, before the next render.
    public static StartingScreen Play(GameObject videoObject, GameObject screen)
    {
        if (videoObject == null) return null;
        var controller = videoObject.GetComponent<StartingScreen>();
        if (controller == null) controller = videoObject.AddComponent<StartingScreen>();
        controller.startingScreen = screen;
        if (controller.video == null) controller.video = videoObject.GetComponent<VideoPlayer>();
        controller.enabled = true;
        videoObject.SetActive(true);
        controller.Begin();
        return controller;
    }

    public static void CancelAll()
    {
        foreach (var cutscene in new List<StartingScreen>(active))
            if (cutscene != null) cutscene.Finish(false);
    }

    void Start() { if (!initialized) Begin(); }
    void OnEnable() { if (initialized) Begin(); }
    void OnDisable() { if (registered) Finish(false); }
    void OnDestroy() { if (registered) Finish(false); }

    void Begin()
    {
        if (registered) return;
        initialized = true;
        finished = false;
        startTime = Time.unscaledTime;
        if (video == null) video = GetComponent<VideoPlayer>();
        if (active.Count == 0) CutsceneSession.Acquire();
        active.Add(this);
        registered = true;
        GameFlow.LockCursor();
        BindVideoScreens();
        if (startingScreen != null) startingScreen.SetActive(true);
        if (skipHint == null && canSkip) CreateSkipHint();
        if (skipHint != null) skipHint.SetActive(false);
        if (video != null)
        {
            video.Stop();
            ClearTexture(video.targetTexture);
            video.playOnAwake = false;
            video.isLooping = false;
            video.waitForFirstFrame = true;
            video.loopPointReached += VideoEnded;
            video.errorReceived += VideoError;
            video.Prepare();
        }
        StartCoroutine(Playback());
    }

    void BindVideoScreens()
    {
        videoScreens.Clear();
        if (video == null || video.targetTexture == null) return;
        foreach (var image in FindObjectsByType<RawImage>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (image.texture != video.targetTexture) continue;
            var canvas = image.GetComponentInParent<Canvas>(true);
            if (canvas == null) continue;
            // Old animations hid these canvases using a different clock than the video.
            var animator = canvas.GetComponent<Animator>();
            if (animator != null) animator.enabled = false;
            canvas.gameObject.SetActive(true);
            canvas.enabled = true;
            if (!videoScreens.Contains(canvas.gameObject)) videoScreens.Add(canvas.gameObject);
        }
    }

    static void ClearTexture(RenderTexture texture)
    {
        if (texture == null) return;
        var previous = RenderTexture.active;
        if (!texture.IsCreated()) texture.Create();
        RenderTexture.active = texture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = previous;
    }

    IEnumerator Playback()
    {
        if (video == null)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0, waitTime));
            Finish(true);
            yield break;
        }
        float deadline = Time.realtimeSinceStartup + PlaybackTimeout;
        while (!finished && !video.isPrepared && Time.realtimeSinceStartup < deadline) yield return null;
        if (finished) yield break;
        if (!video.isPrepared)
        {
            Debug.LogWarning("[Cutscene] Video preparation timed out: " + name);
            Finish(true);
            yield break;
        }
        video.Play();
        long frame = video.frame;
        float lastProgress = Time.realtimeSinceStartup;
        while (!finished)
        {
            if (video.frame != frame) { frame = video.frame; lastProgress = Time.realtimeSinceStartup; }
            if (Time.realtimeSinceStartup - lastProgress > PlaybackTimeout)
            {
                Debug.LogWarning("[Cutscene] Video playback stalled: " + name);
                Finish(true);
                yield break;
            }
            yield return null;
        }
    }

    void VideoEnded(VideoPlayer source) => Finish(true);
    void VideoError(VideoPlayer source, string message)
    {
        Debug.LogWarning("[Cutscene] " + name + ": " + message);
        Finish(true);
    }

    void Update()
    {
        if (!registered || finished) return;
        bool ready = canSkip && Time.unscaledTime - startTime >= skipDelay;
        if (skipHint != null) skipHint.SetActive(ready);
        if (ready && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) Skip();
    }

    public void Skip()
    {
        if (!registered || finished || !canSkip || Time.unscaledTime - startTime < skipDelay) return;
        lastSkipFrame = Time.frameCount;
        Finish(true);
    }

    void Finish(bool notify)
    {
        if (!registered || finished) return;
        finished = true;
        registered = false;
        StopAllCoroutines();
        if (video != null)
        {
            video.loopPointReached -= VideoEnded;
            video.errorReceived -= VideoError;
            video.Stop();
        }
        if (startingScreen != null) startingScreen.SetActive(false);
        foreach (var screen in videoScreens) if (screen != null) screen.SetActive(false);
        if (skipHint != null) skipHint.SetActive(false);
        if (hideOnFinish != null)
            foreach (var screen in hideOnFinish) if (screen != null) screen.SetActive(false);
        active.Remove(this);
        if (active.Count == 0) CutsceneSession.Release();
        if (notify) onFinished.Invoke();
    }

    void CreateSkipHint()
    {
        var canvasObject = new GameObject("Cutscene Skip Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 300;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        var textObject = new GameObject("SkipHint", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvasObject.transform, false);
        var rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(.65f, .03f); rect.anchorMax = new Vector2(.97f, .1f);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var text = textObject.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset; text.fontSize = 22;
        text.text = "[Space / Enter] Skip"; text.alignment = TextAlignmentOptions.Right;
        text.raycastTarget = false;
        skipHint = textObject;
    }
}
