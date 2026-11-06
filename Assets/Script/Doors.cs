using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Animator door;
    public GameObject openText;

    public AudioSource doorSound;


    public bool inReach;

    // Animator ของประตูใช้ bool "Open" แบบ pulse: true หนึ่งเฟรมเพื่อสลับ เปิด <-> ปิด
    private bool pulseSent;


    void Start()
    {
        inReach = false;
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
        // รีเซ็ต pulse ในเฟรมถัดไป แทนการสั่ง SetBool ทุกเฟรม
        if (pulseSent)
        {
            door.SetBool("Open", false);
            pulseSent = false;
        }

        if (inReach && GameFlow.InteractPressed())
        {
            ToggleDoor();
        }
    }

    void ToggleDoor()
    {
        door.SetBool("Open", true);
        pulseSent = true;
        doorSound.Play();
    }
}
