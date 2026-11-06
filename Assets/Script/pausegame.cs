using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pausegame : MonoBehaviour
{
    public GameObject menu;
    public GameObject resume;
    public GameObject quit;

    private bool isPaused;


    void Start()
    {
        menu.SetActive(false);
        isPaused = false;
    }


    void Update()
    {
        if (!Input.GetButtonDown("pause")) return;

        if (isPaused)
        {
            Resume();
        }
        // ไม่ให้ pause ซ้อนตอนเปิด UI อื่น (Keypad, Computer ฯลฯ) หรือตอนเกมจบแล้ว
        else if (GameFlow.IsInGameplay())
        {
            Pause();
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
        isPaused = true;
        GameFlow.UnlockCursor();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
        isPaused = false;
        GameFlow.LockCursor();
    }

    public void Restart()
    {
        GameFlow.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        GameFlow.LoadScene(GameFlow.MainMenuScene);
    }

    // ปุ่ม Exit ในเมนู Pause กลับไป Main Menu แทนการปิดเกม
    public void Exit()
    {
        MainMenu();
    }
}
