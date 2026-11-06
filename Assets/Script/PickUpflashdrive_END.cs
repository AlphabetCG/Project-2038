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
            // ของในห้องบอสถูกจัดการโดย BossManager แล้ว ช่อง item1-5 ปล่อยว่างได้
            foreach (var item in new[] { item1, item2, item3, item4, item5 })
            {
                if (item != null) item.SetActive(true);
            }
            keyOB.SetActive(false);
            keySound.Play();
            invOB.SetActive(true);
            pickUpText.SetActive(false);
            oldtext.SetActive(false);
            newtext.SetActive(true);
        }

        
    }
}
