using UnityEngine;

public class PickUpflashdrive_END : MonoBehaviour
{
public GameObject keyOB;
    public GameObject invOB;
    public GameObject pickUpText;
    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject item4;
    public GameObject item5;
    public GameObject oldtext;
    public GameObject newtext;
    public AudioSource keySound;

    public bool inReach;


    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);
        invOB.SetActive(false);
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
        if (inReach && Input.GetButtonDown("Interact"))
        {
            item1.SetActive(true);
            item2.SetActive(true);
            item3.SetActive(true);
            item4.SetActive(true);
            item5.SetActive(true);
            keyOB.SetActive(false);
            keySound.Play();
            invOB.SetActive(true);
            pickUpText.SetActive(false);
            oldtext.SetActive(false);
            newtext.SetActive(true);
        }

        
    }
}
