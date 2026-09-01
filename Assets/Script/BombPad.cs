using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BombPad : MonoBehaviour
{
public GameObject player;
    public GameObject keypadOB;
    public GameObject hud;
    public GameObject inv;
    public GameObject Oldtext;
    public GameObject newtext;

    public Animator ANI;
    public AudioSource audioSource; 

    public TextMeshProUGUI textOB;
    public GameObject doortoilet;
    public GameObject budisiam;
    public string answer = "12345";

    public AudioSource button;
    public AudioSource correct;
    public AudioSource wrong;

    public bool animate;

    void Start()
    {
        keypadOB.SetActive(false);
    }

    public void Number(int number)
    {
    if (textOB.text.Length < 5) // Example: Maximum 5 digits
    {
        textOB.text += number.ToString();
        button.Play();
    }
    }

    public void Execute()
    {
        if (textOB.text == answer)
        {
            budisiam.SetActive(true);
            Debug.Log("its open True");
            correct.Play();
            doortoilet.SetActive(true);
            keypadOB.SetActive(false);
            inv.SetActive(true);
            hud.SetActive(true);
            player.GetComponent<FirstPersonController>().enabled = true;
            Cursor.visible = false;
            audioSource.Stop();
            textOB.text = "Right";
            Oldtext.SetActive(false);
            newtext.SetActive(true);
        }
        else
        {
            wrong.Play();
            textOB.text = "Wrong";
            textOB.text = "";
        }
    }

    public void Clear()
    {
        textOB.text = "";
        button.Play();
    }

    public void Exit()
    {
        keypadOB.SetActive(false);
        inv.SetActive(true);
        hud.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;

        Cursor.visible = false;
    }
    
    public void Update()
    {
        if (textOB.text == "Right" && animate)
        {
            
            keypadOB.SetActive(false);
            ANI.SetTrigger("Open"); // ใช้ Trigger แทน Bool
            Debug.Log("its open na kubbb");
            animate = false; // ป้องกันการรันซ้ำ
        }

        if (keypadOB.activeInHierarchy)
        {
            hud.SetActive(false);
            inv.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
