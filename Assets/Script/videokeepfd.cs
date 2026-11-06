using UnityEngine;

public class videokeepfd : MonoBehaviour
{
    public GameObject video1;
    public GameObject video2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartingScreen.Play(video1, video2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
