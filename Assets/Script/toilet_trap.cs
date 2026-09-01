using UnityEngine;

public class toilet_trap : MonoBehaviour
{
public Animator door;
public AudioSource audioSource; // AudioSource ที่จะเล่นเสียง
public GameObject cude;
public GameObject oldtext;
public GameObject newtext;
public string closeTriggerName = "close";
public GameObject door_lock;
    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        if (other.CompareTag("Player"))
        {
            // เล่นเสียง
            if (audioSource != null)
            {   
                oldtext.SetActive(false);
                newtext.SetActive(true);
                door.SetTrigger(closeTriggerName);
                door_lock.SetActive(false);
                audioSource.Play();
                cude.SetActive(false);
            }
        }
    }
}
