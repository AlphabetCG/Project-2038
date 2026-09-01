using UnityEngine;

public class Bosslogic : MonoBehaviour
{
    public GameObject havekey;
    public GameObject canvas;
    public GameObject checkitem1;
    public GameObject checkitem2;
    public GameObject checkitem3;
    public GameObject checkitem4;
    public GameObject checkitem5;
    public GameObject video1;
    public GameObject video2;
    public AudioSource sound; 
    public GameObject heart1,heart2,heart3,heart4,heart5;
    public bool test = true;
    public bool check1 = true;
    public bool check2 = true;
    public bool check3 = true;
    public bool check4 = true;
    public bool check5 = true;
    private int sum = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (havekey.activeSelf && test)
        {
            canvas.SetActive(true);
            test = false;
        }
        else if (!havekey.activeSelf && !test)
        {
            canvas.SetActive(false);
            test = true; // Reset to allow reactivation
        }
        if(checkitem1.activeSelf && check1){
            check1 = false;
            sum++;
        }
        if(checkitem2.activeSelf && check2){
            check2 = false;
            sum++;
        }
        if(checkitem3.activeSelf && check3){
            check3 = false;
            sum++;
        }
        if(checkitem4.activeSelf && check4){
            check4 = false;
            sum++;
        }
        if(checkitem5.activeSelf && check5){
            check5 = false;
            sum++;
        }
        if(sum == 1){
            heart1.SetActive(false);
        }else if(sum == 2){
            heart2.SetActive(false);
        }else if (sum ==3){
            heart3.SetActive(false);
        }else if(sum ==4){
            heart4.SetActive(false);
        }else if(sum==5){
            video1.SetActive(true);
            video2.SetActive(true);
            heart5.SetActive(false);
            sound.Stop();
        }
    }
}
