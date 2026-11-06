using UnityEngine;

public class Time_Keyfbx : MonoBehaviour
{
    public GameObject time_1;
    public GameObject time_2;
    public GameObject time_3;
    public GameObject time_4;
    public GameObject time_5;
    public GameObject time_6;
    public GameObject time_7;
    public GameObject time_8;
    public GameObject time_9;
    public GameObject time_10;
    public GameObject time_11;
    public GameObject time_12;
    public GameObject save_already;
    public GameObject keyfbx;
    public GameObject USB;
    void Start()
    {
        Invoke("Condition1", 0.5f);
        Invoke("Condition2", 1f);
        Invoke("Condition3", 1.5f);
        Invoke("Condition4", 2f);
        Invoke("Condition5", 2.5f);
        Invoke("Condition6", 3f);
        Invoke("Condition7", 3.5f);
        Invoke("Condition8", 4f);
        Invoke("Condition9", 4.5f);
        Invoke("Condition10", 5f);
        Invoke("Condition11", 5.5f);
        Invoke("Condition12", 6f);
        Invoke("Condition13", 6.5f);
        Invoke("Condition14", 7f);
    }

    void Condition1()
    {
        time_1.SetActive(true);
    }

    void Condition2()
    {
        time_2.SetActive(true);
    }

    void Condition3()
    {
        time_3.SetActive(true);
    }
    void Condition4()
    {
        time_4.SetActive(true);
    }
    void Condition5()
    {
        time_5.SetActive(true);
    }
    void Condition6()
    {
        time_6.SetActive(true);
    }
    void Condition7()
    {
        time_7.SetActive(true);
    }
    void Condition8()
    {
        time_8.SetActive(true);
    }
    void Condition9()
    {
        time_9.SetActive(true);
    }
    void Condition10()
    {
        time_10.SetActive(true);
    }
    void Condition11()
    {
        time_11.SetActive(true);
    }
    void Condition12()
    {
        time_12.SetActive(true);
    }
    void Condition13()
    {
        save_already.SetActive(true);
    }
    void Condition14()
    {
        save_already.SetActive(false);
        keyfbx.SetActive(false);
        USB.SetActive(true);
    }
}
