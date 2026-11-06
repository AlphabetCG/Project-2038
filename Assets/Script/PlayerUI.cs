using UnityEngine;

// จุดเดียวสำหรับเปิด/ปิด UI ที่ต้องหยุดผู้เล่น (Keypad, Computer, Note ฯลฯ)
public static class PlayerUI
{
    public static void Open(GameObject player, GameObject hud, GameObject inv)
    {
        if (hud != null) hud.SetActive(false);
        if (inv != null) inv.SetActive(false);
        SetPlayerEnabled(player, false);
        GameFlow.UnlockCursor();
    }

    public static void Close(GameObject player, GameObject hud, GameObject inv)
    {
        if (hud != null) hud.SetActive(true);
        if (inv != null) inv.SetActive(true);
        SetPlayerEnabled(player, !GameFlow.GameEnded && !StartingScreen.IsPlaying);
        if (GameFlow.GameEnded && !StartingScreen.IsPlaying) GameFlow.UnlockCursor();
        else GameFlow.LockCursor();
    }

    static void SetPlayerEnabled(GameObject player, bool enabled)
    {
        if (player == null) return;
        FirstPersonController fpc = player.GetComponent<FirstPersonController>();
        if (fpc != null) fpc.enabled = enabled;
    }
}
