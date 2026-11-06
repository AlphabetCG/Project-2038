using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battery : MonoBehaviour
{
    private bool inReach;
    private bool collected;

    public GameObject pickUpText;
    private GameObject flashlight;

    public AudioSource pickUpSound;

    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);
        flashlight = GameObject.Find("flashlight");
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
            Collect();
        }
        
    }

    public void Collect()
    {
        if (collected || !inReach || !GameFlow.IsInGameplay()) return;
        collected = true;
        flashlight.GetComponent<FlashlightAdvanced>().batteries += 1;
        pickUpSound.Play();
        inReach = false;
        pickUpText.SetActive(false);
        Destroy(gameObject);
    }
}
