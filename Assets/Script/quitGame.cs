using UnityEngine;

public class quitGame : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quit Game"); // ✅ แสดงใน Console ตอนทดสอบใน Editor
        Application.Quit();      // ✅ ใช้คำสั่งออกจากเกม (จะทำงานเฉพาะตอน Build)
    }
}