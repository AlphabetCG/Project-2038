using UnityEngine;

public class SoundBoss : MonoBehaviour
{
    public GameObject check_box;
    public AudioSource sound;
    private bool isPlaying = false; // ตัวแปรเก็บสถานะเสียง

    void Start()
    {
        // ตรวจสอบว่า AudioSource ถูกกำหนดหรือไม่
        if (sound == null)
        {
            sound = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // ตรวจสอบว่ากล่องถูกเปิดใช้งาน
        if (!check_box.activeSelf)
        {
            if (!isPlaying) // เล่นเสียงครั้งเดียว
            {
                sound.Play();
                isPlaying = true; // ตั้งค่าสถานะเป็นกำลังเล่น
            }
        }
        else
        {
            if (isPlaying) // หยุดเสียงเมื่อยังเล่นอยู่
            {
                sound.Stop();
                isPlaying = false; // ตั้งค่าสถานะเป็นหยุดเล่น
            }
        }
    }
}
