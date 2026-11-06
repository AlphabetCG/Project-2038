using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// จัดการการเปลี่ยนฉาก / restart / จบเกม
// ผูกปุ่มใน Inspector กับ RestartGame(), BackToMainMenu(), QuitGame()
public class GameFlow : MonoBehaviour
{
    public const string MainMenuScene = "MainMenu";
    public const string GameScene = "SampleScene";

    // true เมื่อเกมจบแล้ว (เวลาหมด / ชนะ) เพื่อไม่ให้ ending อื่นทำงานซ้อน
    public static bool GameEnded { get; private set; }

    public void RestartGame()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        LoadScene(MainMenuScene);
    }

    public void QuitGame()
    {
        Quit();
    }

    public static void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        GameEnded = false;
        UnlockCursor();
        SceneManager.LoadScene(sceneName);
    }

    // โหลดฉากแบบ async ให้ loading screen แสดงผลได้จริง
    public static IEnumerator LoadSceneAsync(string sceneName)
    {
        Time.timeScale = 1;
        GameEnded = false;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    // เรียกตอนเกมจบ (Game Over หรือ Win) คืนค่า false ถ้าเกมจบไปแล้ว
    public static bool EndGame()
    {
        if (GameEnded) return false;
        GameEnded = true;
        UnlockCursor();
        return true;
    }

    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // เกมอยู่ในโหมดเล่นปกติ (ไม่ pause, ไม่ได้เปิด UI ที่ปลดล็อกเมาส์ไว้, เกมยังไม่จบ)
    public static bool IsInGameplay()
    {
        return Time.timeScale > 0 && !GameEnded && Cursor.lockState == CursorLockMode.Locked;
    }

    // กดปุ่ม Interact ได้เฉพาะตอนเล่นปกติ (กันการเปิด UI ซ้อนตอน pause หรือตอนเปิด UI อื่นอยู่)
    public static bool InteractPressed()
    {
        return IsInGameplay() && Input.GetButtonDown("Interact");
    }
}
