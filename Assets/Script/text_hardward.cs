using UnityEngine;

public class text_hardward : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;
    public GameObject object4;
    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        if (other.CompareTag("Player"))
        {
            ChecklistProgress.Complete(ChecklistTask.ReachHardware);
            object1.SetActive(false);
            object2.SetActive(true);
            object3.SetActive(true);
            object4.SetActive(true);
        }
    }
}
