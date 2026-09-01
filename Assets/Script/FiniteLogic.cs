using UnityEngine;

public class FiniteLogic : MonoBehaviour
{
    public GameObject item;
    public GameObject s0_active;
    public GameObject s1_active;
    public GameObject s2_active;
    public GameObject s3_active;
    public GameObject video;
    public GameObject video1;

    private bool condition1Done = false; // Flag for first condition
    private bool condition2Done = false; // Flag for second condition

    void Update()
    {
        // Check first condition and ensure it runs only once
        if (!condition1Done && s0_active != null && s0_active.activeSelf &&
            s1_active != null && s1_active.activeSelf &&
            s2_active != null && s2_active.activeSelf && s3_active.activeSelf)
        {
            item.SetActive(true);
            condition1Done = true; // Mark condition as completed
        }

        // Check second condition and ensure it runs only once
        if (!condition2Done && s0_active != null && s0_active.activeSelf &&
            s1_active != null && s1_active.activeSelf &&
            s2_active != null && s2_active.activeSelf && !s3_active.activeSelf)
        {
            video.SetActive(true);
            video1.SetActive(true);
            item.SetActive(true);
            condition2Done = true; // Mark condition as completed
        }
    }
}
