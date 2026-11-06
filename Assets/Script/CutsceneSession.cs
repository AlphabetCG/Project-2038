using System.Collections.Generic;
using UnityEngine;

// Overlapping cutscenes share one control/audio snapshot. Only the last one releases it.
internal static class CutsceneSession
{
    static FirstPersonController player;
    static Rigidbody body;
    static bool controllerEnabled, wasKinematic;
    static Vector3 velocity, angularVelocity;
    static CursorLockMode cursorLock;
    static bool cursorVisible;
    static readonly Dictionary<Canvas, bool> hiddenCanvases = new Dictionary<Canvas, bool>();
    static readonly List<AudioSource> pausedAudio = new List<AudioSource>();

    public static void Reset()
    {
        player = null; body = null;
        hiddenCanvases.Clear();
        pausedAudio.Clear();
    }

    public static void Acquire()
    {
        cursorLock = Cursor.lockState; cursorVisible = Cursor.visible;
        player = Object.FindFirstObjectByType<FirstPersonController>();
        if (player != null)
        {
            OptionsMenu.ApplyToPlayer(player.gameObject);
            controllerEnabled = player.enabled;
            player.enabled = false;
            body = player.GetComponent<Rigidbody>();
            if (body != null)
            {
                wasKinematic = body.isKinematic;
                velocity = body.linearVelocity; angularVelocity = body.angularVelocity;
                if (!wasKinematic)
                {
                    body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero;
                    body.isKinematic = true;
                }
            }
            var hudTransform = player.transform.Find("HUD");
            if (hudTransform != null) Hide(hudTransform.GetComponent<Canvas>());
            var reticle = player.GetComponentInChildren<UnityEngine.UI.Image>();
            if (reticle != null) Hide(reticle.GetComponentInParent<Canvas>());
        }
        if (ChecklistUI.Instance != null) Hide(ChecklistUI.Instance.GetComponentInChildren<Canvas>(true));
        pausedAudio.Clear();
        foreach (var source in Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (!source.loop || !source.isPlaying) continue;
            source.Pause(); pausedAudio.Add(source);
        }
    }

    public static void Release()
    {
        if (GameFlow.GameEnded) GameFlow.UnlockCursor();
        else
        {
            foreach (var canvas in hiddenCanvases) if (canvas.Key != null) canvas.Key.enabled = canvas.Value;
            if (player != null) player.enabled = controllerEnabled;
            if (body != null)
            {
                body.isKinematic = wasKinematic;
                if (!wasKinematic) { body.linearVelocity = velocity; body.angularVelocity = angularVelocity; }
            }
            Cursor.lockState = cursorLock; Cursor.visible = cursorVisible;
            foreach (var source in pausedAudio) if (source != null && source.isActiveAndEnabled) source.UnPause();
        }
        Reset();
    }

    static void Hide(Canvas canvas)
    {
        if (canvas == null || hiddenCanvases.ContainsKey(canvas)) return;
        hiddenCanvases.Add(canvas, canvas.enabled);
        canvas.enabled = false;
    }
}
