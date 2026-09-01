using UnityEngine;

public class Bomb : MonoBehaviour
{
public GameObject keypadOB;
    public GameObject keypadText;
    public GameObject door;
    public GameObject bomb;

    public bool inReach;


    void Start()
    {
        inReach = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            keypadText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            keypadText.SetActive(false);

        }
    }




    void Update()
    {
        if(Input.GetButtonDown("Interact") && inReach)
        {
            keypadOB.SetActive(true);
        }
        if (bomb != null && bomb.activeSelf)
        {
            door.SetActive(true);
        }
        

    }
}
