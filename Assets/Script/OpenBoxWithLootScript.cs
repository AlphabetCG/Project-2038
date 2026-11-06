using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBoxWithLootScript : MonoBehaviour
{
    public Animator boxOB;
    public GameObject keyOBNeeded;
    public GameObject openText;
    public GameObject KeyMissingText;
    public AudioSource openSound;
    public GameObject drop1;
    public bool inReach;
    public bool isOpen;
    public GameObject oldtext;
    public GameObject new_line;
    void Start()
    {
        inReach = false;
        openText.SetActive(false);
        KeyMissingText.SetActive(false);
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
        if(other.gameObject.tag == "Reach")
        {
            inReach = false;
            openText.SetActive(false);
            KeyMissingText.SetActive(false);
        }
    }
    void Update()
    {
        if (inReach && GameFlow.InteractPressed()) TryOpen();
        if(isOpen)
        {
            boxOB.GetComponent<BoxCollider>().enabled = false;
            boxOB.GetComponent<OpenBoxWithLootScript>().enabled = false;
        }
    }

    public void TryOpen()
    {
        if (isOpen || !inReach || !GameFlow.IsInGameplay()) return;
        if(keyOBNeeded.activeInHierarchy)
        {
            keyOBNeeded.SetActive(false);
            openSound.Play();
            boxOB.SetBool("open",true);
            openText.SetActive(false);
            KeyMissingText.SetActive(false);
            isOpen = true;
            ChecklistProgress.Complete(ChecklistTask.PrintKey);
            drop1.SetActive(true);
            oldtext.SetActive(false);
            new_line.SetActive(true);

        }
        else
        {
            openText.SetActive(false);
            KeyMissingText.SetActive(true);
        }
    }
}
