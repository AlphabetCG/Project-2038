using UnityEngine;

public class keytextboss : MonoBehaviour
{
    public GameObject oldtext;
    public GameObject newtext;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        oldtext.SetActive(false);
        newtext.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
