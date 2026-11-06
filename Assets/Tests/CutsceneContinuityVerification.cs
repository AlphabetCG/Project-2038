#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// Run on the real scene in Play Mode. All scene mutations are temporary.
public class CutsceneContinuityVerification : MonoBehaviour
{
    const string Folder = "Temp/CutsceneContinuity/";
    bool originalBackground;
    bool running;

    [MenuItem("Tools/Project2038/Verify Cutscene Continuity (Play Mode)")]
    public static void RunVerification()
    {
        if (!EditorApplication.isPlaying || SceneManager.GetActiveScene().name != GameFlow.GameScene)
            throw new InvalidOperationException("Enter Play Mode in SampleScene first. This verification reloads that scene.");
        if (FindFirstObjectByType<CutsceneContinuityVerification>() != null) return;
        var runner = new GameObject("Cutscene Continuity Verification").AddComponent<CutsceneContinuityVerification>();
        runner.Begin();
    }

    void Begin()
    {
        Directory.CreateDirectory(Folder);
        File.WriteAllText(Folder + "verification.txt", "");
        originalBackground = Application.runInBackground;
        Application.runInBackground = true;
        running = true;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Safely());
    }

    IEnumerator Safely()
    {
        var routine = Run();
        while (true)
        {
            object current;
            try
            {
                if (!EditorApplication.isPlaying) throw new Exception("Play Mode ended unexpectedly");
                if (!routine.MoveNext()) break;
                current = routine.Current;
            }
            catch (Exception error)
            {
                File.AppendAllText(Folder + "verification.txt", "FAIL " + error + "\n");
                Debug.LogException(error);
                Destroy(gameObject);
                yield break;
            }
            yield return current;
        }
        File.AppendAllText(Folder + "verification.txt", "VERIFICATION COMPLETE\n");
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (running) Application.runInBackground = originalBackground;
    }

    T Find<T>() where T : UnityEngine.Object => FindFirstObjectByType<T>(FindObjectsInactive.Include);
    void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        File.AppendAllText(Folder + "verification.txt", "PASS " + message + "\n");
    }
    void ControlsReleased(string context)
    {
        var player = Find<FirstPersonController>();
        Check(!StartingScreen.IsPlaying && player.enabled && !player.GetComponent<Rigidbody>().isKinematic,
            context + ": releases movement and physics");
        Check(Cursor.lockState == CursorLockMode.Locked && GameFlow.IsInGameplay(), context + ": restores gameplay input");
    }

    IEnumerator Run()
    {
        GameFlow.LoadScene(GameFlow.GameScene);
        yield return null; yield return null;
        var player = Find<FirstPersonController>();
        var intro = GameObject.Find("MapLogic").GetComponent<StartingScreen>();
        var ambient = player.GetComponent<AudioSource>();
        Check(StartingScreen.IsPlaying && !player.enabled && player.GetComponent<Rigidbody>().isKinematic,
            "Intro owns player movement and physics");
        Check(!player.transform.Find("HUD").GetComponent<Canvas>().enabled &&
            !player.GetComponentInChildren<UnityEngine.UI.Image>().GetComponentInParent<Canvas>().enabled,
            "HUD and crosshair hidden during intro");
        Check(!ambient.isPlaying, "Looping gameplay audio paused during dialogue");
        float deadline = Time.realtimeSinceStartup + 12;
        while (!intro.video.isPlaying) { if (Time.realtimeSinceStartup > deadline) throw new Exception("Intro prepare"); yield return null; }
        var introCanvas = player.transform.Find("Video").GetComponent<Canvas>();
        Check(introCanvas.enabled && !introCanvas.GetComponent<Animator>().enabled, "Video completion replaces early canvas animator");
        yield return new WaitForSecondsRealtime(1.3f);
        Capture("intro");
        int introFinished = 0;
        intro.onFinished.AddListener(() => introFinished++);
        intro.video.time = intro.video.length - .5;
        deadline = Time.realtimeSinceStartup + 12;
        while (StartingScreen.IsPlaying) { if (Time.realtimeSinceStartup > deadline) throw new Exception("Intro natural completion"); yield return null; }
        ControlsReleased("Intro natural completion");
        Check(introFinished == 1 && !intro.video.isPlaying && !introCanvas.gameObject.activeSelf,
            "Natural completion stops video and hides its actual canvas exactly once");
        Check(ambient.isPlaying && player.transform.Find("HUD").GetComponent<Canvas>().enabled,
            "Intro returns HUD and ambient audio");

        var key = Find<PickUpFinalKey>();
        var keyCutscene = key.video.GetComponent<StartingScreen>();
        int keyFinished = 0;
        keyCutscene.onFinished.AddListener(() => keyFinished++);
        key.inReach = true; key.Collect();
        Check(StartingScreen.IsPlaying && !player.enabled, "Key pickup acquires cutscene control synchronously");
        keyCutscene.Skip();
        Check(StartingScreen.IsPlaying && keyFinished == 0, "Cannot skip in the first second");
        yield return new WaitForSecondsRealtime(3.3f);
        Check(StartingScreen.IsPlaying && !player.enabled && key.sceen.GetComponent<Canvas>().enabled,
            "7.5-second key video remains visible and locked beyond old 3-second timeout");
        Capture("key-after-three-seconds");
        keyCutscene.video.time = keyCutscene.video.length - .5;
        deadline = Time.realtimeSinceStartup + 12;
        while (StartingScreen.IsPlaying) { if (Time.realtimeSinceStartup > deadline) throw new Exception("Key natural completion"); yield return null; }
        ControlsReleased("Key natural completion");
        Check(keyFinished == 1 && !key.sceen.activeSelf && !keyCutscene.video.isPlaying, "Key completion closes picture and audio once");
        StartingScreen.Play(key.video, key.sceen);
        yield return new WaitForSecondsRealtime(1.1f);
        keyCutscene.Skip(); keyCutscene.Skip();
        Check(keyFinished == 2, "Replay and repeated Skip invoke one completion per playback");
        Check(!GameFlow.IsInGameplay(), "Skip input is consumed before returning to gameplay");
        yield return null;
        ControlsReleased("Key skip");

        StartingScreen.Play(intro.gameObject, intro.startingScreen);
        StartingScreen.Play(key.video, key.sceen);
        yield return new WaitForSecondsRealtime(1.1f);
        keyCutscene.Skip();
        Check(StartingScreen.IsPlaying && !player.enabled && player.GetComponent<Rigidbody>().isKinematic && !ambient.isPlaying,
            "Finishing one overlapping cutscene cannot release another's controls or audio");
        intro.Skip();
        yield return null;
        ControlsReleased("Last overlapping cutscene");
        StartingScreen.Play(key.video, key.sceen);
        keyCutscene.enabled = false;
        Check(!StartingScreen.IsPlaying && !key.sceen.activeSelf, "External disable cancels without a stuck cutscene gate");
        keyCutscene.enabled = true;
        Check(StartingScreen.IsPlaying, "Re-enabling a controller can replay it");
        StartingScreen.CancelAll();
        ControlsReleased("Cancelled replay");

        var finalDrive = Find<PickUpflashdrive_END>();
        var boss = Find<BossManager>();
        finalDrive.inReach = true; finalDrive.Collect();
        yield return null; yield return null;
        var keep = boss.startTrigger.GetComponent<videokeepfd>().video1.GetComponent<StartingScreen>();
        Check(StartingScreen.IsPlaying && !boss.IsActive && !boss.bossMusic.isPlaying,
            "Final drive plays story cutscene before starting boss or boss music");
        deadline = Time.realtimeSinceStartup + 12;
        while (!keep.video.isPlaying) { if (Time.realtimeSinceStartup > deadline) throw new Exception("USB prepare"); yield return null; }
        keep.video.time = keep.video.length - .5;
        deadline = Time.realtimeSinceStartup + 12;
        while (StartingScreen.IsPlaying) { if (Time.realtimeSinceStartup > deadline) throw new Exception("USB natural completion"); yield return null; }
        yield return null; yield return null;
        Check(boss.IsActive && boss.bossMusic.isPlaying, "Boss starts after story playback naturally ends");
        ControlsReleased("USB story completion");

        boss.SendMessage("Win");
        var ending = boss.winVideo2.GetComponent<StartingScreen>();
        // WinScreen is a scene root in some scene revisions.
        GameObject win = null;
        foreach (var t in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (t.name == "WinScreen") win = t.gameObject;
        Check(GameFlow.GameEnded && StartingScreen.IsPlaying && !win.activeSelf, "Win screen waits behind ending video");
        yield return new WaitForSecondsRealtime(1.1f);
        ending.Skip();
        Check(win.activeSelf && !StartingScreen.IsPlaying && !player.enabled && player.GetComponent<Rigidbody>().isKinematic,
            "Skipping ending opens WinScreen without restoring player movement");
        Check(Cursor.lockState == CursorLockMode.None, "Ending hands cursor to result screen");

        GameFlow.LoadScene(GameFlow.GameScene);
        yield return null; yield return new WaitForSecondsRealtime(1.1f);
        GameObject.Find("MapLogic").GetComponent<StartingScreen>().Skip();
        var timer = Find<TimerController>();
        timer.gameObject.SetActive(true);
        yield return null;
        timer.gameObject.SetActive(true);
        timer.SendMessage("TimerEnded");
        var loss = timer.video.GetComponent<StartingScreen>();
        Check(loss != null && StartingScreen.IsPlaying && !timer.gameOverScreen.activeSelf,
            "Timer failure starts controlled Game Over video before results");
        deadline = Time.realtimeSinceStartup + 12;
        while (!loss.video.isPlaying) { if (Time.realtimeSinceStartup > deadline) throw new Exception("Loss prepare"); yield return null; }
        loss.video.playbackSpeed = .5f;
        yield return new WaitForSecondsRealtime(3.3f);
        Check(StartingScreen.IsPlaying && !timer.gameOverScreen.activeSelf,
            "Results follow video time rather than old three-second wall clock");
        Capture("game-over-before-results");
        loss.video.time = loss.video.length - .4;
        deadline = Time.realtimeSinceStartup + 12;
        while (StartingScreen.IsPlaying) { if (Time.realtimeSinceStartup > deadline) throw new Exception("Loss natural completion"); yield return null; }
        Check(timer.gameOverScreen.activeSelf && !Find<FirstPersonController>().enabled,
            "Natural Game Over completion opens results with movement locked");

        GameFlow.LoadScene(GameFlow.GameScene);
        yield return null; yield return new WaitForSecondsRealtime(1.1f);
        GameObject.Find("MapLogic").GetComponent<StartingScreen>().Skip();
        var finite = Find<FiniteLogic>();
        finite.s0_active.SetActive(true); finite.s1_active.SetActive(true);
        finite.s2_active.SetActive(true); finite.s3_active.SetActive(false);
        finite.SendMessage("Update");
        Check(GameFlow.GameEnded && StartingScreen.IsPlaying && !finite.gameOverScreen.activeSelf,
            "Sensor failure uses the same video completion path");
        yield return new WaitForSecondsRealtime(1.1f);
        finite.video.GetComponent<StartingScreen>().Skip();
        Check(finite.gameOverScreen.activeSelf && !StartingScreen.IsPlaying, "Sensor failure is skippable into results");

        GameFlow.LoadScene(GameFlow.GameScene);
        yield return null; yield return new WaitForSecondsRealtime(1.1f);
        GameObject.Find("MapLogic").GetComponent<StartingScreen>().Skip();
        var missingVideoObject = new GameObject("Expected missing video QA");
        var missingVideo = missingVideoObject.AddComponent<VideoPlayer>();
        missingVideo.playOnAwake = false;
        missingVideo.source = VideoSource.Url;
        missingVideo.url = Path.GetFullPath(Folder + "missing-video.mp4");
        var errorScreen = new GameObject("Error fallback result"); errorScreen.SetActive(false);
        var cover = new GameObject("Missing video cover");
        GameFlow.PlayEnding(cover, missingVideoObject, 0, errorScreen);
        deadline = Time.realtimeSinceStartup + 18;
        while (StartingScreen.IsPlaying) { if (Time.realtimeSinceStartup > deadline) throw new Exception("Missing video recovery"); yield return null; }
        Check(errorScreen.activeSelf && !cover.activeSelf, "Unreadable video recovers into its completion target");
        yield return null;
        ControlsReleased("Unreadable video recovery");
        Destroy(missingVideoObject); Destroy(errorScreen); Destroy(cover);

        var fallbackObject = new GameObject("Screen-only cutscene QA"); fallbackObject.SetActive(false);
        var fallback = fallbackObject.AddComponent<StartingScreen>();
        fallback.startingScreen = new GameObject("Screen-only cover");
        fallback.waitTime = .25f; fallback.canSkip = false;
        int fallbackFinished = 0; fallback.onFinished.AddListener(() => fallbackFinished++);
        fallbackObject.SetActive(true);
        yield return new WaitForSecondsRealtime(.1f);
        fallback.Skip(); Check(StartingScreen.IsPlaying, "Non-skippable screen cannot be skipped");
        yield return new WaitForSecondsRealtime(.3f);
        Check(!StartingScreen.IsPlaying && fallbackFinished == 1, "Screen-only fallback uses realtime and completes once");
        Destroy(fallback.startingScreen); Destroy(fallbackObject);

        key = Find<PickUpFinalKey>();
        StartingScreen.Play(key.video, key.sceen);
        GameFlow.LoadScene(GameFlow.GameScene);
        yield return null; yield return new WaitForSecondsRealtime(1.1f);
        GameObject.Find("MapLogic").GetComponent<StartingScreen>().Skip();
        yield return null;
        ControlsReleased("Restart during a cutscene");
        Check(!GameFlow.GameEnded && Find<BossManager>().IsActive == false,
            "Restart clears old ending and pending boss state");
    }

    void Capture(string name)
    {
        var go = new GameObject("Cutscene QA Camera");
        var camera = go.AddComponent<Camera>();
        camera.transform.position = new Vector3(-1000, 1000, -1000);
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.black;
        camera.orthographic = true; camera.nearClipPlane = .1f; camera.farClipPlane = 10;
        var target = new RenderTexture(1280, 720, 24); camera.targetTexture = target;
        var canvases = new List<(Canvas canvas, RenderMode mode, Camera camera, float distance)>();
        foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (!canvas.isRootCanvas || canvas.renderMode == RenderMode.WorldSpace || !canvas.enabled) continue;
            canvases.Add((canvas, canvas.renderMode, canvas.worldCamera, canvas.planeDistance));
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
        }
        Canvas.ForceUpdateCanvases(); camera.Render();
        var previous = RenderTexture.active; RenderTexture.active = target;
        var texture = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); texture.Apply();
        File.WriteAllBytes(Folder + name + ".png", texture.EncodeToPNG());
        RenderTexture.active = previous;
        foreach (var state in canvases)
        {
            state.canvas.renderMode = state.mode; state.canvas.worldCamera = state.camera; state.canvas.planeDistance = state.distance;
        }
        camera.targetTexture = null;
        DestroyImmediate(go); DestroyImmediate(texture); target.Release(); DestroyImmediate(target);
        Canvas.ForceUpdateCanvases();
    }
}
#endif
