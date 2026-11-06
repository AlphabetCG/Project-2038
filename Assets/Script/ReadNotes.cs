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
        if(GameFlow.InteractPressed() && inReach)
        {
            Read();
        }
    }

    public void Read()
    {
        if (!inReach || !GameFlow.IsInGameplay()) return;
        if (readnote != null) ChecklistProgress.Complete(ChecklistTask.ReadNote);
        noteUI.SetActive(true);
        pickUpSound.Play();
        line.SetActive(true);
        PlayerUI.Open(player, hud, inv);
    }


    public void ExitButton()
    {   
        if (readnote != null) readnote.SetActive(false);
        if (line != null) line.SetActive(false);
        if (Login != null) Login.SetActive(true);
        noteUI.SetActive(false);
        PlayerUI.Close(player, hud, inv);
    }
}
