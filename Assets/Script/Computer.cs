using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Computer_Jomo สืบทอดจากคลาสนี้
public class Computer : MonoBehaviour
{
    public GameObject player;
    public GameObject noteUI;
    public GameObject hud;
    public GameObject inv;

    public GameObject pickUpText;

    public AudioSource pickUpSound;

    public bool inReach;



    protected virtual void Start()
    {
        noteUI.SetActive(false);
        hud.SetActive(true);
        inv.SetActive(true);
        pickUpText.SetActive(false);

        inReach = false;

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            pickUpText.SetActive(true);

        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            pickUpText.SetActive(false);
        }
    }




    protected virtual void Update()
    {
        if(GameFlow.InteractPressed() && inReach)
        {
            noteUI.SetActive(true);
            pickUpSound.Play();
            PlayerUI.Open(player, hud, inv);
        }

    }


    public void ExitButton()
    {
        noteUI.SetActive(false);
        PlayerUI.Close(player, hud, inv);
    }
}
