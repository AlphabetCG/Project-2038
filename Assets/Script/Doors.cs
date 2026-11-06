using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Doors : MonoBehaviour
{
    public Animator door;
    public GameObject openText;

    public AudioSource doorSound;


    public bool inReach;

    // Animator ของประตูใช้ bool "Open" แบบ pulse: true หนึ่งเฟรมเพื่อสลับ เปิด <-> ปิด
    private bool pulseSent;
    private bool isOpen;


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
        isOpen = !isOpen;
        UpdatePrompt();
    }

    void UpdatePrompt()
    {
        TMP_Text label = openText.GetComponentInChildren<TMP_Text>(true);
        if (label != null) label.text = isOpen ? "[E] Close Door" : "[E] Open Door";
    }
}
