using UnityEngine;

public class keyboss : MonoBehaviour
{
 public GameObject keyOB;
    public GameObject invOB;
    public GameObject pickUpText;
    public AudioSource keySound;
    public GameObject oldtext;
    public GameObject newtext;
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
            keyOB.SetActive(false);
            keySound.Play();
            oldtext.SetActive(false);
            newtext.SetActive(true);
            invOB.SetActive(true);
            pickUpText.SetActive(false);
        }
    }
}
