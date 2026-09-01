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
        Debug.Log("1 second passed - Condition 1 triggered");
    }

    void Condition2()
    {
        time_2.SetActive(true);
        Debug.Log("2 seconds passed - Condition 2 triggered");
    }

    void Condition3()
    {
        time_3.SetActive(true);
        Debug.Log("3 seconds passed - Condition 3 triggered");
    }
    void Condition4()
    {
        time_4.SetActive(true);
        Debug.Log("4 seconds passed - Condition 4 triggered");
    }
    void Condition5()
    {
        time_5.SetActive(true);
        Debug.Log("5 seconds passed - Condition 5 triggered");
    }
    void Condition6()
    {
        time_6.SetActive(true);
        Debug.Log("6 seconds passed - Condition 6 triggered");
    }
    void Condition7()
    {
        time_7.SetActive(true);
        Debug.Log("7 seconds passed - Condition 7 triggered");
    }
    void Condition8()
    {
        time_8.SetActive(true);
        Debug.Log("8 seconds passed - Condition 8 triggered");
    }
    void Condition9()
    {
        time_9.SetActive(true);
        Debug.Log("9 seconds passed - Condition 9 triggered");
    }
    void Condition10()
    {
        time_10.SetActive(true);
        Debug.Log("10 seconds passed - Condition 10 triggered");
    }
    void Condition11()
    {
        time_11.SetActive(true);
        Debug.Log("10 seconds passed - Condition 10 triggered");
    }
    void Condition12()
    {
        time_12.SetActive(true);
        Debug.Log("10 seconds passed - Condition 10 triggered");
    }
    void Condition13()
    {
        save_already.SetActive(true);
        Debug.Log("10 seconds passed - Condition 10 triggered");
    }
    void Condition14()
    {
        save_already.SetActive(false);
        keyfbx.SetActive(false);
        USB.SetActive(true);
    }
}
