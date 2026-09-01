using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ReadNotes : MonoBehaviour
{
    public GameObject player;
    public GameObject noteUI;
    public GameObject hud;
    public GameObject inv;

    public GameObject pickUpText;
    public GameObject line;
    public GameObject Login;
    public GameObject readnote;

    public AudioSource pickUpSound;

    public bool inReach;



    void Start()
    {
        noteUI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        pickUpText.SetActive(false);

        inReach = false;
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            pickUpText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            pickUpText.SetActive(false);
        }
    }




    void Update()
    {
        if(Input.GetButtonDown("Interact") && inReach)
        {
            noteUI.SetActive(true);
            pickUpSound.Play();
            line.SetActive(true);
            hud.SetActive(false);
            inv.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
    }


    public void ExitButton()
    {   
        readnote.SetActive(false);
        line.SetActive(false);
        Login.SetActive(true);
        noteUI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // ล็อคเคอร์เซอร์ไว้กลางจอ
    }
}
