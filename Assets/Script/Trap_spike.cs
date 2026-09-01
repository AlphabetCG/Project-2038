using UnityEngine;

public class Trap_spike : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;
    public GameObject object4;
    public GameObject object5;
    public GameObject object6;
    public GameObject object7;
    public GameObject object8;
    public GameObject object9;
    public GameObject object10;
    public GameObject object11;
    public GameObject object12;
    public GameObject object13;
    public GameObject object14;
    public GameObject object15;
    public GameObject object16;
    public GameObject object17;
    public GameObject object18;
    public GameObject object19;
    public GameObject object20;
    public GameObject object21;
    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าเป็นผู้เล่นหรือไม่
        if (other.CompareTag("Player"))
        {
            object1.SetActive(true);
            object2.SetActive(true);
            object3.SetActive(true);
            object4.SetActive(true);
            object5.SetActive(true);
            object6.SetActive(true);
            object7.SetActive(true);
            object8.SetActive(true);
            object9.SetActive(true);
            object10.SetActive(true);
            object11.SetActive(true);
            object12.SetActive(true);
            object13.SetActive(true);
            object14.SetActive(true);
            object15.SetActive(true);
            object16.SetActive(true);
            object17.SetActive(true);
            object18.SetActive(true);
            object19.SetActive(true);
            object20.SetActive(true);
            object21.SetActive(true);
        }
    }
}
