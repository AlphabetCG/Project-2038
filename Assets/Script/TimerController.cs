using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerController : MonoBehaviour
{
    public TextMeshProUGUI timerText; // สำหรับ TextMeshPro
    // หรือใช้ public Text timerText; // สำหรับ UI Text แบบเก่า

    public float startTime = 60f; // เวลาเริ่มต้น (เช่น 60 วินาที)
    private float currentTime;    // เวลา ณ ขณะนั้น
    private bool isTimerRunning = false; // สถานะจับเวลา
    public GameObject test;
    public GameObject video;
    public GameObject video1;

    public GameObject button;
    public GameObject gameOverScreen; // หน้า Game Over (มีปุ่ม Restart / Main Menu) ไม่บังคับ
    public float screenDelay = 3f;    // รอให้วิดีโอ Game Over เล่นจบก่อนแสดงปุ่ม

    void Start()
    {
        currentTime = startTime;
        isTimerRunning = true; // เริ่มจับเวลา
    }

    void Update()
    {
        if (!isTimerRunning) return;

        // ถ้าเกมจบไปแล้วด้วยทางอื่น (เช่น ชนะบอส) ให้หยุดจับเวลา
        if (GameFlow.GameEnded)
        {
            isTimerRunning = false;
            return;
        }

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // ลดเวลาตามเฟรมเรต
            UpdateTimerUI(); // อัปเดต UI
        }
        else
        {
            currentTime = 0;
            isTimerRunning = false; // หยุดจับเวลา
            TimerEnded(); // เรียกฟังก์ชันเมื่อเวลาหมด
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60); // คำนวณนาที
        int seconds = Mathf.FloorToInt(currentTime % 60); // คำนวณวินาที
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // แสดงผลในรูปแบบ mm:ss
    }

    void TimerEnded()
    {
        if (!GameFlow.EndGame()) return;

        video.SetActive(true);
        video1.SetActive(true);
        test.SetActive(false);
        // test คือ GameObject ที่มีสคริปต์นี้อยู่ จึงให้ GameFlow เป็นคนหน่วงเวลาแทน
        GameFlow.ShowAfterDelay(screenDelay, button, gameOverScreen);
    }
}
