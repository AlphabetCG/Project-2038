using UnityEngine;

public class trapchangetext : MonoBehaviour
{
public GameObject cude;
public GameObject oldtext;
public GameObject newtext;
    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        if (other.CompareTag("Player"))
        {
                ChecklistProgress.Complete(ChecklistTask.ReachBossRoom);
                oldtext.SetActive(false);
                newtext.SetActive(true);
                cude.SetActive(false);
        }
    }
}

