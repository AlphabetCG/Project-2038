using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
        else if (locked)
        {
            lockedSound.Play();
        }
    }
}
