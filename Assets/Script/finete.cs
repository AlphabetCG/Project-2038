using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class finete : MonoBehaviour
{
 public GameObject player;
 public GameObject canvas;
    public GameObject noteUI;
    public GameObject hud;
    public GameObject inv;

    public GameObject pickUpText;
    public GameObject activefinite;

    public bool inReach;
    public bool test;



    void Start()
    {
        activefinite.SetActive(false);
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
            canvas.SetActive(true);
            if(test){
                activefinite.SetActive(true);
            }else{
                noteUI.SetActive(true);
            }
            PlayerUI.Open(player, hud, inv);
        }
        
    }


    public void ExitButton()
    {   
        canvas.SetActive(false);
        PlayerUI.Close(player, hud, inv);
    }
    public void ExitButton_other()
    {   
        canvas.SetActive(false);
        PlayerUI.Close(player, hud, inv);
    }
    public void pressButton(){
        test = true;
        noteUI.SetActive(false);
        activefinite.SetActive(true);
    }
    public void pressButtonOther(){
        test = false;
        noteUI.SetActive(true);
        activefinite.SetActive(false);
    }
}
