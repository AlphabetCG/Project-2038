using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    public GameObject pickUpText;
    public GameObject cude;

    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        if (other.CompareTag("Player"))
        {
            // แสดงข้อความ
            pickUpText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // เมื่อผู้เล่นออกจากพื้นที่ ให้ซ่อนข้อความอีกครั้ง
        if (other.CompareTag("Player"))
        {
            pickUpText.SetActive(false);
            cude.SetActive(false);
        }
    }
}
