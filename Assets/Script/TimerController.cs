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

    void Start()
    {
        currentTime = startTime;
        isTimerRunning = true; // เริ่มจับเวลา
    }

    void Update()
    {
        if (isTimerRunning)
        {
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
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60); // คำนวณนาที
        int seconds = Mathf.FloorToInt(currentTime % 60); // คำนวณวินาที
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // แสดงผลในรูปแบบ mm:ss
    }

    void TimerEnded()
    {
        video.SetActive(true);
        video1.SetActive(true);
        test.SetActive(false);
        button.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Time's up!");
        // ใส่เงื่อนไขเพิ่มเติม เช่น เปลี่ยน Scene หรือแสดงข้อความ
    }
}
