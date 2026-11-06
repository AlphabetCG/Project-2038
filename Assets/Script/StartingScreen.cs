using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

// แสดงคัตซีน (หรือหน้าจอเริ่ม) ตามเวลา waitTime ระหว่างนั้นหยุดผู้เล่นไว้
// กด Space / Enter เพื่อข้าม
public class StartingScreen : MonoBehaviour
{
    private GameObject player;
    public GameObject startingScreen;

    public float waitTime;

    public VideoPlayer video;        // วิดีโอที่ต้องหยุดตอนข้าม (ว่างไว้ = ใช้ VideoPlayer บน GameObject นี้)
    public GameObject skipHint;      // ข้อความ "[Space] Skip" ไม่บังคับ
    public GameObject[] hideOnFinish; // ซ่อนเพิ่มตอนจบ/ข้าม เช่น canvas วิดีโอที่ Animator คุมอยู่
    public bool canSkip = true;
    public float skipDelay = 1f;     // กันกดข้ามโดยไม่ตั้งใจในช่วงแรก
    public UnityEvent onFinished;    // เรียกตอนคัตซีนจบหรือถูกข้าม

    // มีคัตซีนเล่นอยู่ไหม (ใช้กันเปิด Pause ระหว่างคัตซีน)
    public static bool IsPlaying => playingCount > 0;
    private static int playingCount;

    private bool started;
    private bool finished;
    private float startTime;

    void Start()
    {
        if (video == null) video = GetComponent<VideoPlayer>();
        startingScreen.SetActive(true);
        if (skipHint != null) skipHint.SetActive(canSkip);
        player = GameObject.FindWithTag("Player");
        OptionsMenu.ApplyToPlayer(player);
        player.GetComponent<FirstPersonController>().enabled = false;

        startTime = Time.unscaledTime;
        started = true;
        playingCount++;
        StartCoroutine(Starting());
    }

    IEnumerator Starting()
    {
        yield return new WaitForSeconds(waitTime);
        Finish();
    }

    void Update()
    {
        if (finished || !canSkip) return;
        if (Time.unscaledTime - startTime < skipDelay) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Skip();
        }
    }

    public void Skip()
    {
        // ข้ามได้เฉพาะคัตซีนที่เริ่มเล่นแล้ว (Start ทำงานแล้ว)
        if (!started || finished) return;
        if (video != null) video.Stop();
        Finish();
    }

    void Finish()
    {
        if (finished) return;
        finished = true;
        playingCount--;
        StopAllCoroutines();
        startingScreen.SetActive(false);
        if (skipHint != null) skipHint.SetActive(false);
        if (hideOnFinish != null)
        {
            foreach (var go in hideOnFinish)
            {
                if (go != null) go.SetActive(false);
            }
        }
        // ถ้าเกมจบแล้ว (ฉากจบ) ไม่ต้องคืนการควบคุมผู้เล่น
        if (!GameFlow.GameEnded)
        {
            player.GetComponent<FirstPersonController>().enabled = true;
            GameFlow.LockCursor();
        }
        onFinished.Invoke();
    }

    void OnDestroy()
    {
        if (started && !finished) playingCount--;
    }
}
