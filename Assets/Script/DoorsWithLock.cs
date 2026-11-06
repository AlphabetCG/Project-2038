using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorsWithLock : MonoBehaviour
{
    public Animator door;
    public GameObject openText;
    public GameObject KeyINV;

    public AudioSource doorSound;
    public AudioSource lockedSound;


    public bool inReach;
    public bool unlocked;
    public bool locked;
    public bool hasKey;

    // Animator ของประตูใช้ bool "Open" แบบ pulse: true หนึ่งเฟรมเพื่อสลับ เปิด <-> ปิด
    private bool pulseSent;
    private bool isOpen;


    void Start()
    {
        inReach = false;
        hasKey = false;
        unlocked = false;
        locked = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            openText.SetActive(true);
            UpdatePrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            openText.SetActive(false);
        }
    }


    void Update()
    {
        if (pulseSent)
        {
            door.SetBool("Open", false);
            pulseSent = false;
        }

        if(KeyINV.activeInHierarchy)
        {
            locked = false;
            hasKey = true;
        }
        else
        {
            hasKey = false;
        }

        if (!inReach || !GameFlow.InteractPressed()) return;

        if (hasKey)
        {
            unlocked = true;
            door.SetBool("Open", true);
            pulseSent = true;
            doorSound.Play();
            isOpen = !isOpen;
            UpdatePrompt();
        }
        else if (locked)
        {
            lockedSound.Play();
        }
    }

    void UpdatePrompt()
    {
        TMP_Text label = openText.GetComponentInChildren<TMP_Text>(true);
        if (label == null) return;
        if (!KeyINV.activeInHierarchy) label.text = "Locked - Need a Key";
        else label.text = isOpen ? "[E] Close Door" : "[E] Open Door";
    }
}
