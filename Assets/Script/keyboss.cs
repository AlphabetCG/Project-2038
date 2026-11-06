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
    private bool pickedUp; // กันการกดเก็บซ้ำ


    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);
        invOB.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !pickedUp)
        {   
            
            inReach = true;
            pickUpText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !pickedUp)
        {
            inReach = false;
            pickUpText.SetActive(false);

        }
    }


    void Update()
    {
        if (!pickedUp && inReach && GameFlow.InteractPressed())
        {
            pickedUp = true;
            inReach = false;
            keyOB.SetActive(false);
            keySound.Play();
            oldtext.SetActive(false);
            newtext.SetActive(true);
            invOB.SetActive(true);
            pickUpText.SetActive(false);
        }
    }
}
