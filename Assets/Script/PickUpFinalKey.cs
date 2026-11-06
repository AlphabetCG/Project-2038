using UnityEngine;

public class PickUpFinalKey : MonoBehaviour
{
   public GameObject keyOB;
    public GameObject invOB;
    public GameObject pickUpText;
    public AudioSource keySound;
    public GameObject sceen;
    public GameObject video;
    public GameObject Opendoor;
    public GameObject footprint1;
    public GameObject footprint2;
    
    public GameObject footprint3;
    public GameObject footprint4;
    public GameObject footprint5;
    public GameObject footprint6;
    public GameObject footprint7;
    public GameObject footprint8;
    
    public GameObject footprint9;
    public GameObject footprint10;
    public GameObject footprint11;
    public GameObject footprint12;
    public GameObject footprint13;
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
            Collect();
        }
    }

    public void Collect()
    {
        if (pickedUp || !inReach || !GameFlow.IsInGameplay()) return;
        pickedUp = true;
        ChecklistProgress.Complete(ChecklistTask.CollectKey);
        inReach = false;
        Opendoor.SetActive(true);
        sceen.SetActive(true);
        video.SetActive(true);
        keyOB.SetActive(false);
        keySound.Play();
        invOB.SetActive(true);
        pickUpText.SetActive(false);
        footprint1.SetActive(true);
        footprint2.SetActive(true);
        footprint3.SetActive(true);
        footprint4.SetActive(true);
        footprint5.SetActive(true);
        footprint6.SetActive(true);
        footprint7.SetActive(true);
        footprint8.SetActive(true);
        footprint9.SetActive(true);
        footprint10.SetActive(true);
        footprint11.SetActive(true);
        footprint12.SetActive(true);
        footprint13.SetActive(true);
        oldtext.SetActive(false);
        newtext.SetActive(true);
    }
}
