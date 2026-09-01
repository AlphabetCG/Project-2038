using UnityEngine;

public class Sound : MonoBehaviour
{
public AudioSource audioSource; // AudioSource ที่จะเล่นเสียง
public GameObject cude;
    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        if (other.CompareTag("Player"))
        {
            // เล่นเสียง
            if (audioSource != null)
            {   
                audioSource.Play();
                cude.SetActive(false);
            }
        }
    }
}
